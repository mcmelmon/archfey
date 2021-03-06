﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Response : MonoBehaviour
{
    // Inspector
    [SerializeField] Proficiencies.Skill skill_challenge_vs_player = Proficiencies.Skill.None;
    [SerializeField] string text_for_player_success = ""; // "I have seen through your deception."
    [SerializeField] Proficiencies.Skill skill_challenge_vs_npc = Proficiencies.Skill.None;
    [SerializeField] Statement answer_for_player_failure = null;
    [SerializeField] Statement answer_for_player_success = null;

    // properties

    public Actor Me { get; set; }
    public Proficiencies.Skill PlayerChallengingNPC { get; set; }
    public Proficiencies.Skill NPCChallengingPlayer { get; set; }
    public Actor Target { get; set; }
    public string TextForSuccess { get; set; }

    // Unity

    private void Start() {
        SetComponents();
    }

    // public

    public Statement Answer(Actor _target, bool _advantage = false, bool _disadvantage = false)
    {
        Statement answer = null;
        Target = _target;

        if (PlayerChallengingNPC == Proficiencies.Skill.None) return answer_for_player_success;

        switch(PlayerChallengingNPC) {  // the player is making an opposed skill check against the npc
            case Proficiencies.Skill.Deception:
                answer = Me.Actions.OpposedSkillCheck(Proficiencies.Skill.Deception, Target, _advantage, _disadvantage) ? answer_for_player_success : answer_for_player_failure;
                break;
            case Proficiencies.Skill.Insight:
                answer = Me.Actions.OpposedSkillCheck(Proficiencies.Skill.Insight, Target, _advantage, _disadvantage) ? answer_for_player_success : answer_for_player_failure;
                break;
            case Proficiencies.Skill.Intimidation:
                answer = Me.Actions.OpposedSkillCheck(Proficiencies.Skill.Intimidation, Target, _advantage, _disadvantage) ? answer_for_player_success : answer_for_player_failure;
                break;
            case Proficiencies.Skill.Investigation:
                answer = Me.Senses.InvestigationCheck(true, Mathf.Max(15, Target.Actions.DeceptionCheck(true, Player.Instance.Me)), _advantage, _disadvantage) ? answer_for_player_success : answer_for_player_failure;
                break;
            case Proficiencies.Skill.Perception:
                answer = Me.Senses.PerceptionCheck(true, Mathf.Max(15, Target.Actions.DeceptionCheck(true, Player.Instance.Me)), _advantage, _disadvantage) ? answer_for_player_success : answer_for_player_failure;
                break;
            case Proficiencies.Skill.Persuasion:
                answer = Me.Actions.OpposedSkillCheck(Proficiencies.Skill.Persuasion, Target, _advantage, _disadvantage) ? answer_for_player_success : answer_for_player_failure;
                break;
        }
        return answer;
    }

    // private

    private void SetComponents()
    {
        Me = Player.Instance.Me;
        PlayerChallengingNPC = skill_challenge_vs_npc;
        NPCChallengingPlayer = skill_challenge_vs_player;
        TextForSuccess = text_for_player_success;
    }

}
