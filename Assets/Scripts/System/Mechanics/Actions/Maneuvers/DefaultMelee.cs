using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMelee : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public int AttackModifier { get; set; }
    public float Damage { get; set; }
    public int DamageModifier { get; set; }
    public GameObject Target { get; set; }
    public Weapon Weapon { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    public void Strike(GameObject _target)
    {
        if (_target == null) return;

        Target = _target;
        Weapon = Me.Actions.Attack.EquippedMeleeWeapon;
        Weapon.gameObject.SetActive(true);
        SetModifiers();

        if (Hit()) {
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
            if (target_actor.Health != null && target_actor.Actions.Defend != null && Me.Actions != null) {
                int damage_roll = Random.Range(0, Weapon.damage_die) + 1;
                Damage = target_actor.Actions.Defend.DamageAfterDefenses(damage_roll + DamageModifier, Weapon.damage_type);
                target_actor.Health.LoseHealth(Damage, Me);
            }
        } else if (target_structure != null) {
            int damage_roll = Random.Range(0, Weapon.damage_die) + 1;
            target_structure.LoseStructure(damage_roll);
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

        if (target_actor != null) {
            return Random.Range(1, 21) + AttackModifier > target_actor.Actions.Defend.ArmorClass;
        } else if (target_structure != null) {
            return Random.Range(1, 21) + AttackModifier > target_structure.armor_class;
        }

        return false;
    }


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
    }


    private void SetModifiers()
    {
        if (Weapon.is_light)
        {
            AttackModifier = Me.Stats.DexterityProficiency + Weapon.attack_bonus + Actions.SuperiorWeapons[Weapon.damage_type];
            DamageModifier = Me.Stats.DexterityProficiency + Weapon.damage_bonus + Actions.SuperiorWeapons[Weapon.damage_type];
        }
        else
        {
            AttackModifier = Me.Stats.StrengthProficiency + Weapon.attack_bonus + Actions.SuperiorWeapons[Weapon.damage_type];
            DamageModifier = Me.Stats.StrengthProficiency + Weapon.damage_bonus + Actions.SuperiorWeapons[Weapon.damage_type];
        }
    }
}
