using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacredFlame : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public Weapons.DamageType DamageType { get; set; }
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


    public void Cast(Actor _target, bool dispel_effect = false)
    {
        if (_target == null) return;
        Target = _target;

        int challenge_rating = 8 + Me.Stats.ProficiencyBonus + Me.Stats.AttributeProficiency[Proficiencies.Attribute.Wisdom];
        if (!Target.Actions.SavingThrow(Proficiencies.Attribute.Dexterity, challenge_rating)) {
            ApplyDamage();
            DisplayEffects();
        }
    }


    // private


    private void ApplyDamage()
    {
        int damage_roll = Me.Actions.RollDie(Die, NumberOfDice);
        int damage_inflicted = Target.Actions.Stats.DamageAfterDefenses(damage_roll, Weapons.DamageType.Radiant);
        Target.Health.LoseHealth(damage_inflicted, Me);
    }


    private void DisplayEffects()
    {
        GameObject flare = Instantiate(SpellEffects.Instance.sacred_flame_prefab, Target.transform.position, Target.transform.rotation, Target.transform);
        flare.name = "SacredFlame";
        flare.transform.position += new Vector3(0, 3, 0);
        Destroy(flare, 0.5f);
    }


    private void SetComponents()
    {
        DamageType = Weapons.DamageType.Radiant;
        Die = 8;
        Level = 0;
        Me = GetComponentInParent<Actor>();
        Range = 10f;
        School = Magic.School.Evocation;
        switch (Me.Stats.Level) {
            case int n when (n > 0 && n < 5):
                NumberOfDice = 1;
                break;
            case int n when (n >= 5 && n < 11):
                NumberOfDice = 2;
                break;
            case int n when (n >= 11 && n < 17):
                NumberOfDice = 3;
                break;
            case int n when (n >= 17):
                NumberOfDice = 4;
                break;
        }
    }
}
