using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerociousClaw : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public int AttackModifier { get; set; }
    public float Damage { get; set; }
    public int DamageModifier { get; set; }
    public GameObject Target { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }

    // public


    public void Cast(GameObject _target, bool dispel_effect = false)
    {
        if (_target == null) return;
        Target = _target;
        SetModifiers();

        if (Hit())
        {
            ApplyDamage();
            DisplayEffects(_target.transform.position);
        }
    }


    // private


    private void ApplyDamage()
    {
        Actor target_actor = Target.GetComponent<Actor>();
        Structure target_structure = Target.GetComponent<Structure>();

        if (target_actor != null) {
            if (target_actor.Health != null && target_actor.Actions.Stats != null && Me.Actions != null) {
                int damage_roll = Random.Range(0, 10) + 1;
                Damage = target_actor.Actions.Stats.DamageAfterDefenses(damage_roll + DamageModifier, Weapons.DamageType.Slashing);
                target_actor.Health.LoseHealth(Damage, Me);
            }
        } else if (target_structure != null) {
            int damage_roll = Random.Range(0, 10) + 1;
            target_structure.LoseStructure(damage_roll, Weapons.DamageType.Slashing);
        }
    }


    public void DisplayEffects(Vector3 _location)
    {
        GameObject _impact = Instantiate(SpellEffects.Instance.physical_strike_prefab, _location + new Vector3(1, 4f, 0), SpellEffects.Instance.physical_strike_prefab.transform.rotation);
        _impact.transform.parent = transform;
        _impact.name = "Impact";
        Destroy(_impact, 1f);
    }


    public bool Hit()
    {
        Actor target_actor = Target.GetComponent<Actor>();
        Structure target_structure = Target.GetComponent<Structure>();

        if (target_actor != null)
        {
            return Random.Range(1, 21) + AttackModifier > target_actor.Actions.Stats.ArmorClass;
        }
        else if (target_structure != null)
        {
            return Random.Range(1, 21) + AttackModifier > target_structure.armor_class;
        }

        return false;
    }


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
        Damage = 0;
    }


    private void SetModifiers()
    {
        AttackModifier = Me.Stats.AttributeProficiency[Proficiencies.Attribute.Strength];
        DamageModifier = Me.Stats.AttributeProficiency[Proficiencies.Attribute.Strength];
    }
}