using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealth : MonoBehaviour {

    // properties

    public int ChallengeRatting { get; set; }
    public Actor Me { get; set; }
    public bool Hiding { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void Appear()
    {
        Hiding = false;
        ChallengeRatting = 0;
    }


    public void Hide()
    {
        Hiding = true;
        ChallengeRatting = Me.Actions.RollDie(20, 1) + StealthRating();
    }


    // private


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
    }


    private int StealthRating()
    {
        bool proficient = Me.Stats.Skills.Contains(Proficiencies.Skill.Stealth);
        int dexterity_bonus = Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Dexterity);
        return (proficient) ? Me.Stats.ProficiencyBonus + dexterity_bonus : dexterity_bonus;
    }
}
