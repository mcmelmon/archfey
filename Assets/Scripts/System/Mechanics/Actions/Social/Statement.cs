﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Statement : MonoBehaviour
{
    // Inspector
    [SerializeField] bool is_opening_statement = false;
    [SerializeField] bool is_final_statement = false;
    [SerializeField] string statement = "This is my statement.";
    [SerializeField] string non_statement = "I've got nothing to say to you.";
    [SerializeField] int minimum_faction_reputation = 0;
    [SerializeField] int minimum_individual_reputation = 0;
    [SerializeField] int minimum_plot_reputation = 0;
    [SerializeField] List<Response> potential_responses = new List<Response>(); // e.g. Insight, success, "You're hiding something."

    // properties

    public Actor Me { get; set; }
    public List<Response> ChallengeFailed { get; set; }
    public List<Response> ChallengeSucceeded { get; set; }
    public string FinalText { get; set; }

    public bool IsFinalStatement { get; set; }
    public bool IsOpeningStatement { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    // public

    public string GetStatementToPlayer()
    {
        // pick one of the npc's statements based on reputations

        if (MeetsReputationRequirements()) {
            return statement;
        }

        return non_statement;
    }

    public List<Response> PresentResponses()
    {
        List<Response> presentable_responses = new List<Response>();

        if (potential_responses.Count == 0) return presentable_responses;

        foreach (var response in potential_responses) {
            if (presentable_responses.Count > 3) break;

            if (ChallengeFailed.Contains(response)) continue;  // only one bite at the apple!

            if (ChallengeSucceeded.Contains(response)) {
                presentable_responses.Add(response);
                continue;
            }

            if (response.NPCChallengingPlayer == Proficiencies.Skill.None) {
                presentable_responses.Add(response);
                continue;
            }

            if (response.PlayerChallengingNPC != Proficiencies.Skill.None) {
                // If the player is challenging and has gotten here, they can pick this response (e.g. an intimidating line) if they have not already tried
                if (!Me.Dialog.ResponsesChosen.Contains(response)) presentable_responses.Add(response);
                continue;
            }

            // From here, it is the NPC who is challenging the PC to see the response 

            switch(response.NPCChallengingPlayer) {
                case Proficiencies.Skill.Deception:
                    if (!Me.Actions.OpposedSkillCheck(Proficiencies.Skill.Deception, Player.Instance.Me)) {
                        presentable_responses.Add(response);
                        ChallengeSucceeded.Add(response);
                    } else {
                        ChallengeFailed.Add(response);
                    }
                    break;
                case Proficiencies.Skill.Intimidation:
                    if (!Me.Actions.OpposedSkillCheck(Proficiencies.Skill.Intimidation, Player.Instance.Me)) {
                        presentable_responses.Add(response);
                        ChallengeSucceeded.Add(response);
                    } else {
                        ChallengeFailed.Add(response);
                    }
                    break;
                default:
                    Debug.Log("NPC isn't useing a known conversation skill: " + response.NPCChallengingPlayer);
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
        ChallengeFailed = new List<Response>();
        ChallengeSucceeded = new List<Response>();
        IsFinalStatement = is_final_statement; // multiple branches can lead to dead ends

        if (is_opening_statement) {
            if (Me.GetComponentsInChildren<Statement>().Where(s => s.is_opening_statement).ToList().Count == 1) {
                IsOpeningStatement = is_opening_statement;
            } else {
                Debug.Log(Me + " has more than one opening statement: " + statement);
            }
        }
    }
}
