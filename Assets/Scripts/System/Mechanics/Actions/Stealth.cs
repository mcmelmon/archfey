using System.Collections;
using System.Linq;
using UnityEngine;

public class Stealth : MonoBehaviour {

    // properties

    public int ChallengeRatting { get; set; }
    public Actor Me { get; set; }
    public bool IsHiding { get; set; }
    public float StartingSpeedAdjustment { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void Appear()
    {
        if (!IsHiding) return;

        IsHiding = false;
        ChallengeRatting = 0;
        Me.Actions.Movement.ResetSpeed();
        Me.Actions.Movement.AdjustSpeed(StartingSpeedAdjustment);
    }


    public void Hide()
    {
        if (IsHiding) return;

        IsHiding = true;
        ChallengeRatting = Me.Actions.RollDie(20, 1) + StealthRating();
        StartingSpeedAdjustment = Me.Actions.Movement.SpeedAdjustment;
        Me.Actions.Movement.AdjustSpeed(-.2f);
        StartCoroutine(Obscure());
    }


    public void PickLock()
    {
        if (Mouse.SelectedObjects.Count == 1) {
            Item target = Mouse.SelectedObjects.First().GetComponent<Item>();

            if (target != null && !target.IsUnlocked) {
                bool proficient = Me.Stats.Tools.Contains(Proficiencies.Tool.Thief);
                bool expertise = Me.Stats.ExpertiseInTools.Contains(Proficiencies.Tool.Thief);
                int proficiency_bonus = expertise ? Me.Stats.ProficiencyBonus * 2 : Me.Stats.ProficiencyBonus;
                int dexterity_bonus = Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Dexterity);

                int bonus = proficient ? proficiency_bonus + dexterity_bonus : dexterity_bonus;
                int roll = Me.Actions.RollDie(20, 1);
                if (roll + bonus > target.unlock_challenge_rating) {
                    target.IsUnlocked = true;
                } else {
                    if (target.unlock_challenge_rating < 30) target.unlock_challenge_rating += 1;
                    if (target.unlock_challenge_rating > 30) target.unlock_challenge_rating = 30;
                }
            }
        }
    }


    public void SleightOfHand()
    {
        if (Mouse.SelectedObjects.Count == 1) {
            Actor target_actor = Mouse.SelectedObjects.First().GetComponent<Actor>();
            Item target_item = Mouse.SelectedObjects.First().GetComponent<Item>();

            bool proficient = Me.Stats.Skills.Contains(Proficiencies.Skill.SleightOfHand);
            bool expertise = Me.Stats.ExpertiseInSkills.Contains(Proficiencies.Skill.SleightOfHand);
            int proficiency_bonus = expertise ? Me.Stats.ProficiencyBonus * 2 : Me.Stats.ProficiencyBonus;
            int skill_bonus = Me.Stats.GetAdjustedAttributeScore(Proficiencies.Instance.GetAttributeForSkill(Proficiencies.Skill.SleightOfHand));
            int roll = Me.Actions.RollDie(20, 1);

            int challenge_rating = roll + skill_bonus;

            if (target_actor != null) {
                bool target_perception_check = target_actor.Senses.PerceptionCheck(false, challenge_rating); // TODO: advantage/disadvantage
                if (!target_perception_check) {
                    GameObject thing = target_actor.Pockets.FirstOrDefault();
                    if (thing != null) {
                        Player.Instance.Inventory.AddThing(thing);
                        target_actor.Pockets.Remove(thing);
                    }
                } else {
                    //Appear();
                }
            } else if (target_item != null) {
                // TODO: swipe items out in the open; failure may result in town guard
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
        while (IsHiding) {
            GetComponent<MeshRenderer>().enabled = false;
            yield return null;
        }
        GetComponent<MeshRenderer>().enabled = true;
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
