using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Combat : MonoBehaviour
{
    // properties

    public Range AttackFromRange { get; set; }
    public Melee AttackInMelee { get; set; }
    public int AttacksPerAction { get; set; }
    public AdditionalDamage CalculateAdditionalDamage { get; set; }
    public List<Weapon> CombatSpells { get; set; }
    public int CriticalRangeStart { get; set; }
    public bool Engaged { get; set; }
    public Armor EquippedArmor { get; set; }
    public Weapon EquippedMeleeWeapon { get; set; }
    public Weapon EquippedRangedWeapon { get; set; }
    public Weapon EquippedOffhand { get; set; }
    public Actor Me { get; set; }

    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public delegate int AdditionalDamage(GameObject target, bool is_ranged);


    public int DefaultAdditionalDamage(GameObject target, bool is_ranged)
    {
        return 0;
    }


    public void EquipArmor(Armor armor)
    {
        EquippedArmor = Instantiate(armor, transform.position, transform.rotation);
        EquippedArmor.transform.parent = transform;
        EquippedArmor.name = "Armor";
        EquippedArmor.gameObject.SetActive(false);
    }


    // TODO: weapon sets; enforce offhand rules
    public void EquipMeleeWeapon(Weapon weapon)
    {    
        EquippedMeleeWeapon = Instantiate(weapon, Me.MainHand.position, transform.rotation);
        EquippedMeleeWeapon.transform.parent = Me.MainHand;
        EquippedMeleeWeapon.name = "Melee Weapon";
        EquippedMeleeWeapon.gameObject.SetActive(false);
    }


    public void EquipOffhand(Weapon weapon)
    {
        EquippedOffhand = Instantiate(weapon, Me.OffHand.position, transform.rotation);
        EquippedOffhand.transform.parent = Me.OffHand;
        EquippedOffhand.name = "Offhand";
        EquippedOffhand.gameObject.SetActive(false);
    }


    public void EquipRangedWeapon(Weapon weapon)
    {
        EquippedRangedWeapon = Instantiate(weapon, Me.MainHand.position, transform.rotation);
        EquippedRangedWeapon.transform.parent = Me.MainHand;
        EquippedRangedWeapon.name = "Ranged Weapon";
        EquippedRangedWeapon.gameObject.SetActive(false);
    }


    public void EquipShield(Armor shield)
    {
        EquippedOffhand = Instantiate(shield, Me.OffHand.position, transform.rotation).GetComponent<Weapon>();
        EquippedOffhand.transform.parent = Me.OffHand;
        EquippedOffhand.name = "Shield";
        EquippedOffhand.gameObject.SetActive(false);
    }


    public bool HasSurprise(Actor other_actor)
    {
        return !Me.Actions.Stealth.SpottedBy(other_actor);
    }


    public bool IsAttackable(GameObject target)
    {
        return target.GetComponent<Actor>() != null || target.GetComponent<Structure>() != null;
    }


    public bool IsWithinAttackRange(Transform target)
    {
        float separation = Me.SeparationFrom(target);
        return (EquippedRangedWeapon != null) ? separation < EquippedRangedWeapon.Range : IsWithinMeleeRange(target.transform);
    }


    public bool IsWithinMeleeRange(Transform target)
    {
        float separation = Me.SeparationFrom(target) - Me.Actions.Movement.StoppingDistance();
        return separation < MeleeRange();
    }


    public float MeleeRange()
    {
        return EquippedMeleeWeapon == null
            ? 0f
            : EquippedMeleeWeapon.HasReach
                ? 1f
                : 0.5f;
    }


    public void StrikeEnemy(GameObject target, bool is_ranged, bool offhand = false, bool player_target = false)
    {
        if (target == null || (EquippedMeleeWeapon == null && EquippedRangedWeapon == null)) return;

        if (player_target) {
            StrikePlayerChoice(target, is_ranged, offhand);
            return;
        }

        if (!is_ranged) {
            EquippedMeleeWeapon.gameObject.SetActive(true);
            if (EquippedOffhand != null) EquippedOffhand.gameObject.SetActive(true);
            if (EquippedRangedWeapon != null) EquippedRangedWeapon.gameObject.SetActive(false);
            if (IsWithinMeleeRange(target.transform)) AttackInMelee.Strike(target, offhand);
        } else {
            EquippedRangedWeapon.gameObject.SetActive(true);
            if (EquippedMeleeWeapon != null) EquippedMeleeWeapon.gameObject.SetActive(false);
            if (EquippedOffhand != null) EquippedOffhand.gameObject.SetActive(false);
            if (IsWithinAttackRange(target.transform)) AttackFromRange.Strike(target);
        }
        if (Me.Actions.Stealth.IsHiding) Me.Actions.Stealth.StopHiding(); // appear after the strike to ensure sneak attack damage, etc
        Engaged = true;
    }


    // private


    private void SetComponents()
    {
        AttacksPerAction = 1;
        CalculateAdditionalDamage = DefaultAdditionalDamage;
        CombatSpells = new List<Weapon>();
        CriticalRangeStart = 20;
        Engaged = false;
        Me = GetComponentInParent<Actor>();
        AttackInMelee = GetComponent<Melee>();
        AttackFromRange = GetComponent<Range>();
    }


    private void StrikePlayerChoice(GameObject target, bool is_ranged, bool offhand = false)
    {
        if (!is_ranged) {
            EquippedMeleeWeapon.gameObject.SetActive(true);
            if (EquippedOffhand != null) EquippedOffhand.gameObject.SetActive(true);
            if (EquippedRangedWeapon != null) EquippedRangedWeapon.gameObject.SetActive(false);
            AttackInMelee.Strike(target, offhand);
        } else if (EquippedRangedWeapon != null) {
            EquippedRangedWeapon.gameObject.SetActive(true);
            if (EquippedMeleeWeapon != null) EquippedMeleeWeapon.gameObject.SetActive(false);
            if (EquippedOffhand != null) EquippedOffhand.gameObject.SetActive(false);
            AttackFromRange.Strike(target);
        }
        if (Me.Actions.Stealth.IsHiding) Me.Actions.Stealth.StopHiding(); // appear after the strike to ensure sneak attack damage, etc
        Engaged = true;
    }
}
