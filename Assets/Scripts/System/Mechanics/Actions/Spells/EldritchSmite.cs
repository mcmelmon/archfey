using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EldritchSmite : MonoBehaviour
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
        ApplyDamage();
        // TODO: knock prone
    }


    // private


    private void ApplyDamage()
    {
        int damage_roll = Me.Actions.RollDie(Die, NumberOfDice);
        int damage_inflicted = Target.Actions.Stats.DamageAfterDefenses(damage_roll, DamageType);
        Target.Health.LoseHealth(damage_inflicted, Me);
    }


    private void DisplayEffects()
    {
        GameObject flare = Instantiate(SpellEffects.Instance.sacred_flame_prefab, Target.transform.position, Target.transform.rotation, Target.transform);
        flare.name = "EldritchSmite";
        flare.transform.position += new Vector3(0, 3, 0);
        Destroy(flare, 1.5f);
    }


    private void SetComponents()
    {
        DamageType = Weapons.DamageType.Radiant;
        Die = 8;
        Level = 4;
        Me = GetComponentInParent<Actor>();
        Range = 10f;
        School = Magic.School.Evocation;
        NumberOfDice = Level;
    }
}
