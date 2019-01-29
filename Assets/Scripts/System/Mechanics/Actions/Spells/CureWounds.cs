using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CureWounds : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public int Die { get; set; }
    public int Level { get; set; }
    public int NumberOfDice { get; set; }
    public float Range { get; set; }
    public Magic.School School { get; set; }
    public Actor Target { get; set; }


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

        ApplyHealing();
        DisplayEffects();
    }


    // private


    private void ApplyHealing()
    {
        int healing_roll = Me.Actions.RollDie(Die, NumberOfDice);
        Target.Health.RecoverHealth(healing_roll + Me.Stats.AttributeProficiency[Proficiencies.Attribute.Wisdom]);
    }


    private void DisplayEffects()
    {
        GameObject sparkles = Instantiate(SpellEffects.Instance.fountain_of_healing_prefab, Target.transform.position, Target.transform.rotation, Target.transform);
        sparkles.name = "Healing";
        sparkles.transform.position += new Vector3(0, 3, 0);
        Destroy(sparkles, 2f);
    }


    private void SetComponents()
    {
        Die = 8;
        Level = 1;
        Me = GetComponentInParent<Actor>();
        NumberOfDice = Level;
        Range = 2f;
        School = Magic.School.Evocation;
    }
}
