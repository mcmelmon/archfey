using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sanctuary : MonoBehaviour
{
    // properties

    public int ChallengeRating { get; set; }
    public int Duration { get; set; }
    public GameObject Effects { get; set; }
    public Actor Me { get; set; }
    public int Level { get; set; }
    public static Dictionary<Actor, Sanctuary> ProtectedTargets { get; set; }
    public float Range { get; set; }
    public Magic.School School { get; set; }
    public Actor Target { get; set; }
    public int Tick { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }

    
    // public


    public void Cast(Actor _target, int _level = 1)
    {
        if (_target == null) return;
        Target = _target;
        Level = _level;

        ProtectTarget();
    }


    // private


    private void DisplayEffects()
    {
        Effects = Instantiate(SpellEffects.Instance.fountain_of_healing_prefab, Target.transform.position, Target.transform.rotation, Target.transform);
        Effects.name = "Sanctuary";
        Effects.transform.position += new Vector3(0, 3, 0);
    }


    private IEnumerator Expire()
    {
        DisplayEffects();

        while (Tick < Duration) {
            Tick++;
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }

        Destroy(Effects, .5f);
        if (ProtectedTargets[Target] == this) ProtectedTargets.Remove(Target);  // a new casting may have supplanted another
    }


    private void ProtectTarget()
    {
        // The Decider must check Sanctuary.AffectedTargets

        // TODO: create an intermediate class that manages ongoing spell effects for things like
        // redirecting attacks.

        if (ProtectedTargets.ContainsKey(Target)) {
            ProtectedTargets[Target].Tick = Duration;  // time out the existing ienumerator
            ProtectedTargets[Target] = this;
        } else {
            ProtectedTargets[Target] = this;
        }
    }


    private void SetComponents()
    {
        ProtectedTargets = new Dictionary<Actor, Sanctuary>();
        Duration = 10;
        Level = 1;
        Me = GetComponentInParent<Actor>();
        Range = 15f;
        School = Magic.School.Abjuration;
        Tick = 0;

        ChallengeRating = 8 + Me.Stats.ProficiencyBonus + Me.Stats.AttributeProficiency[Proficiencies.Attribute.Wisdom];
    }
}
