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
        ChallengeRatting = Me.Actions.SkillCheck(true, Proficiencies.Skill.Stealth); // TODO: advantage/disadvantage
        StartingSpeedAdjustment = Me.Actions.Movement.SpeedAdjustment;
        Me.Actions.Movement.AdjustSpeed(-.2f);
        StartCoroutine(Obscure());
    }


    public void PickLock()
    {
        if (Mouse.SelectedObjects.Count == 1) {
            Item target = Mouse.SelectedObjects.First().GetComponent<Item>();

            if (target != null && !target.IsUnlocked) {
                if (Me.Actions.ToolCheck(Proficiencies.Tool.Thief) > target.unlock_challenge_rating) {
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

            if (target_actor != null) {
                int sleight_challenge_rating = Me.Actions.SkillCheck(true, Proficiencies.Skill.SleightOfHand); // TODO: advantage/disadvantage
                bool target_perception_check = target_actor.Senses.PerceptionCheck(false, sleight_challenge_rating);
                if (!target_perception_check) {
                    GameObject thing = target_actor.Inventory.Pockets.FirstOrDefault();
                    if (thing != null) {
                        Player.Instance.Me.Inventory.AddToPockets(thing);
                        target_actor.Inventory.Pockets.Remove(thing);
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
}
