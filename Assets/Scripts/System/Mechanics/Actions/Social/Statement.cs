using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statement : MonoBehaviour
{
    // Inspector
    [SerializeField] string statement = "This is my statement.";
    [SerializeField] string non_statement = "I've got nothing to say to you.";
    [SerializeField] List<Statement> potential_responses = new List<Statement>(); // e.g. Insight, success, "You're hiding something."
    [SerializeField] int minimum_faction_reputation = 0;
    [SerializeField] int minimum_individual_reputation = 0;
    [SerializeField] int minimum_plot_reputation = 0;
    [SerializeField] bool player_making_challenge = false;
    [SerializeField] Proficiencies.Skill applicable_skill = Proficiencies.Skill.None;
    [SerializeField] Statement response_for_player_success = null; // in case of player_making_challenge, the "success" is having the applicable skill; the npc resists after the option is chosen (affects answer)
    [SerializeField] Statement response_for_player_failure = null;
    [SerializeField] Statement answer_for_player_success = null; // the npc answer if the player challenged and was successful (or there was no challenge)
    [SerializeField] Statement answer_for_player_failure = null; // the npc answer if the player failed


    // properties

    public Actor Me { get; set; }
    public bool SeenByPlayer { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    // public

    public Statement Answer(bool _advantage = false, bool _disadvantage = false)
    {
        Statement answer = null;

        switch(applicable_skill) {
            case Proficiencies.Skill.Deception:
                answer = Player.Instance.Me.Actions.OpposedSkillCheck(Proficiencies.Skill.Deception, Me, _advantage, _disadvantage) ? answer_for_player_success : answer_for_player_failure;
                break;
            case Proficiencies.Skill.Insight:
                answer = Player.Instance.Me.Actions.OpposedSkillCheck(Proficiencies.Skill.Insight, Me, _advantage, _disadvantage) ? answer_for_player_success : answer_for_player_failure;
                break;
            case Proficiencies.Skill.Intimidation:
                answer = Player.Instance.Me.Actions.OpposedSkillCheck(Proficiencies.Skill.Intimidation, Me, _advantage, _disadvantage) ? answer_for_player_success : answer_for_player_failure;
                break;
            case Proficiencies.Skill.Investigation:
                answer = Player.Instance.Me.Senses.InvestigationCheck(true, Mathf.Max(15, Me.Actions.DeceptionCheck(true, Player.Instance.Me)), _advantage, _disadvantage) ? answer_for_player_success : answer_for_player_failure;
                break;
            case Proficiencies.Skill.Perception:
                answer = Player.Instance.Me.Senses.PerceptionCheck(true, Mathf.Max(15, Me.Actions.DeceptionCheck(true, Player.Instance.Me)), _advantage, _disadvantage) ? answer_for_player_success : answer_for_player_failure;
                break;
            case Proficiencies.Skill.Persuasion:
                answer = Player.Instance.Me.Actions.OpposedSkillCheck(Proficiencies.Skill.Persuasion, Me, _advantage, _disadvantage) ? answer_for_player_success : answer_for_player_failure;
                break;
        }
        return answer;
    }

    public string GetStatementToPlayer()
    {
        // pick one of the npc's statements based on reputations

        if (MeetsReputationRequirements()) {
            return statement;
        }

        return non_statement;
    }

    public List<Statement> PresentResponses()
    {
        List<Statement> presentable_responses = new List<Statement>();

        foreach (var statement in potential_responses) {
            if (statement.applicable_skill == Proficiencies.Skill.None) {
                presentable_responses.Add(statement.response_for_player_success);
                continue;
            }

            if (!Player.Instance.Me.Stats.Skills.Contains(statement.applicable_skill)) continue; // we don't have the needed skill (e.g. Insight) to choose this response

            if (statement.player_making_challenge) {
                // If the player is challenging and has gotten here, they can pick this response (e.g. an intimidating line)
                presentable_responses.Add(statement.response_for_player_success);
                continue;
            }

            // From here, it is the NPC who is challenging the PC to see the response 

            switch(statement.applicable_skill) {
                case Proficiencies.Skill.Deception:
                    if (Me.Actions.OpposedSkillCheck(Proficiencies.Skill.Deception, Player.Instance.Me)) {
                        presentable_responses.Add(statement.response_for_player_failure);
                    } else {
                        presentable_responses.Add(statement.response_for_player_success);
                    }
                    break;
                case Proficiencies.Skill.Intimidation:
                    if (Me.Actions.OpposedSkillCheck(Proficiencies.Skill.Intimidation, Player.Instance.Me)) {
                        presentable_responses.Add(statement.response_for_player_failure);
                    } else {
                        presentable_responses.Add(statement.response_for_player_success);
                    }
                    break;
                default:
                    Debug.Log("NPC isn't useing a known conversation skill: " + statement.applicable_skill);
                    break;               
            }
        }

        return presentable_responses;
    }

    // private

    private bool MeetsReputationRequirements()
    {
        if (Player.Instance.Me.Reputations.GetReputationFor(Me.CurrentFaction) >= minimum_faction_reputation
            && Player.Instance.Me.Reputations.GetReputationFor(Me) >= minimum_individual_reputation
            && MeetsPlotReputationRequirements()) {
            return true;
        }

        return false;
    }

    private bool MeetsPlotReputationRequirements()
    {
        foreach (var plot in Me.Plots) {
            if (Player.Instance.Me.Reputations.GetReputationFor(plot) < minimum_plot_reputation) return false;
        }

        return true;
    }

    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
    }
}
