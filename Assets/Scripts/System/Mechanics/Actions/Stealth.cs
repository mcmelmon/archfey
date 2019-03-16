using System.Collections;
using System.Linq;
using UnityEngine;

public class Stealth : MonoBehaviour {

    // properties

    public int ChallengeRatting { get; set; }
    public Actor Me { get; set; }
    public bool IsHiding { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void Appear()
    {
        IsHiding = false;
        ChallengeRatting = 0;
    }


    public void Hide()
    {
        ChallengeRatting = Me.Actions.RollDie(20, 1) + StealthRating();
        if (!IsHiding) StartCoroutine(Obscure());
    }


    public void PickLock()
    {
        if (Mouse.SelectedObjects.Count == 1) {
            Item target = Mouse.SelectedObjects.First().GetComponent<Item>();

            if (target != null && target.is_locked) {
                bool proficient = Me.Stats.Tools.Contains(Proficiencies.Tool.Thief);
                bool expertise = Me.Stats.ExpertiseInTools.Contains(Proficiencies.Tool.Thief);
                int proficiency_bonus = expertise ? Me.Stats.ProficiencyBonus * 2 : Me.Stats.ProficiencyBonus;
                int dexterity_bonus = Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Dexterity);

                int bonus = proficient ? proficiency_bonus + dexterity_bonus : dexterity_bonus;
                int roll = Me.Actions.RollDie(20, 1);
                if (roll + bonus > target.unlock_challenge_rating) {
                    target.is_locked = false;
                } else {
                    if (target.unlock_challenge_rating < 30) target.unlock_challenge_rating += 5;
                    if (target.unlock_challenge_rating > 30) target.unlock_challenge_rating = 30;
                }
            }
        }
    }


    public bool SpottedBy(Actor other_actor)
    {
        // TODO: account for environment; decide if being spotted should force Appear
        if (other_actor == null) return false;
        bool spotted = !IsHiding || other_actor.Senses.PerceptionCheck(false, ChallengeRatting);
        return spotted;
    }


    // private


    private IEnumerator Obscure()
    {
        float starting_speed_adjustment = Me.Actions.Movement.SpeedAdjustment;
        Me.Actions.Movement.AdjustSpeed(starting_speed_adjustment - 0.2f);  // if no other adjustment, move at half speed
        IsHiding = true;

        while (IsHiding) {
            GetComponent<MeshRenderer>().enabled = false;
            yield return null;
        }

        GetComponent<MeshRenderer>().enabled = true;
        Me.Actions.Movement.ResetSpeed();
        Me.Actions.Movement.AdjustSpeed(starting_speed_adjustment);
    }


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
    }


    private int StealthRating()
    {
        bool proficient = Me.Stats.Skills.Contains(Proficiencies.Skill.Stealth);
        bool expertise =  Me.Stats.ExpertiseInSkills.Contains(Proficiencies.Skill.Stealth);
        int proficiency_bonus = expertise ? Me.Stats.ProficiencyBonus * 2 : Me.Stats.ProficiencyBonus;
        int skill_bonus = Me.Stats.GetAdjustedAttributeScore(Proficiencies.Instance.GetAttributeForSkill(Proficiencies.Skill.Stealth));

        return proficient ? proficiency_bonus + skill_bonus : skill_bonus;
    }
}
