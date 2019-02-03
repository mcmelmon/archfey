using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefaultMelee : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public bool Advantage { get; set; }
    public int AttackModifier { get; set; }
    public bool Critical { get; set; }
    public float Damage { get; set; }
    public int DamageModifier { get; set; }
    public bool Disadvantage { get; set; }
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

        CheckAdvantageAndDisadvantage();

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
            if (target_actor.Health != null && target_actor.Actions.Stats != null && Me.Actions != null) {
                int damage_roll = (Critical) ? (Me.Actions.RollDie(Weapon.dice_type, Weapon.number_of_dice) * 2) + 1 : Me.Actions.RollDie(Weapon.dice_type, Weapon.number_of_dice);
                damage_roll += DamageModifier;
                Damage = target_actor.Actions.Stats.DamageAfterDefenses(damage_roll, Weapon.damage_type);
                target_actor.Health.LoseHealth(Damage, Me);
            }
        } else if (target_structure != null) {
            int damage_roll = (Critical) ? Me.Actions.RollDie(Weapon.dice_type, Weapon.number_of_dice) + 1 : Random.Range(0, Weapon.dice_type) + 1;
            damage_roll += DamageModifier;
            target_structure.LoseStructure(damage_roll, Weapon.damage_type);
        }

        Critical = false;
    }


    private void CheckAdvantageAndDisadvantage()
    {
        var friends_in_melee = Me.Senses.Actors
                                 .Where(f => Me.Actions.Decider.IsFriendOrNeutral(f) && Vector3.Distance(transform.position, f.transform.position) < 2f)
                                 .ToList();

        Advantage |= friends_in_melee.Count > Me.Actions.Attack.AvailableMeleeTargets.Count;
    }


    private void DisplayEffects(Vector3 _location)
    {
        GameObject _impact = Instantiate(SpellEffects.Instance.physical_strike_prefab, _location, SpellEffects.Instance.physical_strike_prefab.transform.rotation);
        _impact.name = "Impact";
        Destroy(_impact, 3f);
    }


    private bool Hit()
    {
        Actor target_actor = Target.GetComponent<Actor>();
        Structure target_structure = Target.GetComponent<Structure>();

        int roll = Advantage && !Disadvantage
            ? Mathf.Max(Random.Range(1, 21), Random.Range(1, 21))
            : !Advantage && Disadvantage ? Mathf.Min(Random.Range(1, 21), Random.Range(1, 21)) : Random.Range(1, 21);

        if (roll == 20) Critical = true;

        if (target_actor != null) {
            return roll + AttackModifier > target_actor.Actions.Stats.ArmorClass;
        } else if (target_structure != null) {
            return roll + AttackModifier > target_structure.armor_class;
        }

        return false;
    }


    private void SetComponents()
    {
        Advantage = false;
        Critical = false;
        Disadvantage = false;
        Me = GetComponentInParent<Actor>();
    }


    private void SetModifiers()
    {
        if (Weapon.is_light) {
            AttackModifier = Me.Stats.AttributeProficiency[Proficiencies.Attribute.Dexterity] + Weapon.attack_bonus;
            DamageModifier = Me.Stats.AttributeProficiency[Proficiencies.Attribute.Dexterity] + Weapon.damage_bonus;
        } else {
            AttackModifier = Me.Stats.AttributeProficiency[Proficiencies.Attribute.Dexterity] + Weapon.attack_bonus;
            DamageModifier = Me.Stats.AttributeProficiency[Proficiencies.Attribute.Dexterity] + Weapon.damage_bonus;
        }
    }
}
