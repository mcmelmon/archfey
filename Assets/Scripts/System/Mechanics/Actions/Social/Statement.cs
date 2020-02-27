using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statement : MonoBehaviour
{
    // Inspector
    [SerializeField] string text = "This is my statement.";
    [SerializeField] string non_response = "I've got nothing to say to you.";
    [SerializeField] List<StatementSkillChallenge> potential_responses = new List<StatementSkillChallenge>(); // e.g. Insight, success, "You're hiding something."
    [SerializeField] Statement answer_to_response = null;  // e.g. "Talk to my lawyer!"
    [SerializeField] int minimum_faction_reputation = 0;
    [SerializeField] int minimum_individual_reputation = 0;
    [SerializeField] int minimum_plot_reputation = 0;

    public struct StatementSkillChallenge {
        public Proficiencies.Skill skill; // the default response will have a skill of None
        public int challenge_rating;
        public Statement response_for_success;
        public Statement response_for_failure;
    }

    // properties

    public Actor Me { get; set; }
    public bool SeenByPlayer { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    // public

    public Statement AnswerToResponse(Statement _response)
    {
        return _response.answer_to_response;
    }

    public string GetStatementToPlayer()
    {
        // pick one of the npc's statements based on reputations

        if (MeetsReputationRequirements()) {
            return text;
        }

        return non_response;
    }

    public List<Statement> PresentResponses()
    {
        List<Statement> selected_responses = new List<Statement>();

        foreach (var skill_challenge in potential_responses) {
            if (skill_challenge.skill == Proficiencies.Skill.None) {
                selected_responses.Add(skill_challenge.response_for_success);
                continue;
            }

            if (!Player.Instance.Me.Stats.Skills.Contains(skill_challenge.skill)) continue;

            if (Player.Instance.Me.Actions.SkillCheck(true, skill_challenge.skill) >= skill_challenge.challenge_rating) {
                selected_responses.Add(skill_challenge.response_for_success);
            } else {
                if (skill_challenge.response_for_failure.text != "") selected_responses.Add(skill_challenge.response_for_failure);
            }
        }

        return selected_responses;
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
