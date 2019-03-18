using System.Collections;
using System.Linq;
using UnityEngine;

public class Stealth : MonoBehaviour {

    // properties

    public Actor Me { get; set; }
    public bool IsHiding { get; set; }
    public bool IsPerforming { get; set; }
    public int PerformanceChallengeRating { get; set; }
    public float StartingSpeedAdjustment { get; set; }
    public int StealthChallengeRating { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void Hide()
    {
        if (IsHiding) return;

        IsHiding = true;
        GetComponent<MeshRenderer>().enabled = false;
        StealthChallengeRating = Me.Actions.SkillCheck(true, Proficiencies.Skill.Stealth); // TODO: advantage/disadvantage
        StartingSpeedAdjustment = Me.Actions.Movement.SpeedAdjustment;
        Me.Actions.Movement.AdjustSpeed(-.2f);
    }


    public void Performance()
    {
        StopHiding();
        if (IsPerforming) return;

        GetComponent<Renderer>().material = GetComponent<Interactable>().highlight_material;
        IsPerforming = true;
        PerformanceChallengeRating = Me.Actions.SkillCheck(true, Proficiencies.Skill.Performance); // TODO: advantage/disadvantage
        StartingSpeedAdjustment = Me.Actions.Movement.SpeedAdjustment;
        Me.Actions.Movement.AdjustSpeed(-.1f);
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
        bool spotted = !IsHiding || other_actor.Senses.PerceptionCheck(false, StealthChallengeRating);
        return spotted;
    }


    public void StopHiding()
    {
        if (!IsHiding) return;

        GetComponent<MeshRenderer>().enabled = true;
        IsHiding = false;
        StealthChallengeRating = 0;
        Me.Actions.Movement.ResetSpeed();
        Me.Actions.Movement.AdjustSpeed(StartingSpeedAdjustment);
    }


    public void StopPerforming()
    {
        if (!IsPerforming) return;

        GetComponent<Renderer>().material = GetComponent<Interactable>().OriginalMaterial;
        IsPerforming = false;
        PerformanceChallengeRating = 0;
        Me.Actions.Movement.ResetSpeed();
        Me.Actions.Movement.AdjustSpeed(StartingSpeedAdjustment);
    }


    // private


    private IEnumerator Distract()
    {
        while (IsPerforming) {
            GetComponent<Renderer>().material = GetComponent<Interactable>().highlight_material;
            yield return null;
        }
        GetComponent<Renderer>().material = GetComponent<Interactable>().OriginalMaterial;
    }


    private IEnumerator Obscure()
    {
        while (IsHiding) {
            GetComponent<MeshRenderer>().enabled = false;
            yield return null;
        }
    }


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
    }
}
