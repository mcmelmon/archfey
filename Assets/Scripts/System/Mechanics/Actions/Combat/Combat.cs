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
    public int CriticalRangeStart { get; set; }
    public bool Engaged { get; set; }
    public Armor EquippedArmor { get; set; }
    public Weapon EquippedMeleeWeapon { get; set; }
    public Weapon EquippedRangedWeapon { get; set; }
    public Weapon EquippedOffhand { get; set; }
    public Actor Me { get; set; }
    public bool Raging { get; set; }


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
        EquippedMeleeWeapon = Instantiate(weapon, Me.weapon_transform.position, transform.rotation);
        EquippedMeleeWeapon.transform.parent = Me.weapon_transform;
        EquippedMeleeWeapon.name = "Melee Weapon";
        EquippedMeleeWeapon.gameObject.SetActive(false);
    }


    public void EquipOffhand(Weapon weapon)
    {
        EquippedOffhand = Instantiate(weapon, Me.offhand_transform.position, transform.rotation);
        EquippedOffhand.transform.parent = Me.offhand_transform;
        EquippedOffhand.name = "Offhand";
        EquippedOffhand.gameObject.SetActive(false);
    }


    public void EquipRangedWeapon(Weapon weapon)
    {
        EquippedRangedWeapon = Instantiate(weapon, Me.weapon_transform.position, transform.rotation);
        EquippedRangedWeapon.transform.parent = Me.weapon_transform;
        EquippedRangedWeapon.name = "Ranged Weapon";
        EquippedRangedWeapon.gameObject.SetActive(false);
    }


    public void EquipShield(Armor shield)
    {
        EquippedOffhand = Instantiate(shield, Me.offhand_transform.position, transform.rotation).GetComponent<Weapon>();
        EquippedOffhand.transform.parent = Me.offhand_transform;
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
        float separation = Vector3.Distance(target.transform.position, transform.position);
        return (EquippedRangedWeapon != null) ? separation < EquippedRangedWeapon.Range : IsWithinMeleeRange(target.transform);
    }


    public bool IsWithinMeleeRange(Transform target)
    {
        float separation = Vector3.Distance(target.transform.position, transform.position);
        return separation < MeleeRange() + 1f;
    }


    public float MeleeRange()
    {
        return EquippedMeleeWeapon == null
            ? 0f
            : EquippedMeleeWeapon.HasReach
                ? Me.Actions.Movement.ReachedThreshold + 2f
                : Me.Actions.Movement.ReachedThreshold + 1f;
    }


    public void StrikeEnemy(GameObject target, bool is_ranged, bool offhand = false, bool player_target = false)
    {
        if (target == null) return;

        if (player_target) {
            StrikePlayerChoice(target, is_ranged, offhand);
            return;
        }

        if (!is_ranged) {
            EquippedMeleeWeapon.gameObject.SetActive(true);
            if (EquippedOffhand != null) EquippedOffhand.gameObject.SetActive(true);
            if (EquippedRangedWeapon != null) EquippedRangedWeapon.gameObject.SetActive(false);
            AttackInMelee.Strike(target, offhand);
        } else {
            EquippedRangedWeapon.gameObject.SetActive(true);
            if (EquippedMeleeWeapon != null) EquippedMeleeWeapon.gameObject.SetActive(false);
            if (EquippedOffhand != null) EquippedOffhand.gameObject.SetActive(false);
            AttackFromRange.Strike(target);
        }
        if (Me.Actions.Stealth.IsHiding) Me.Actions.Stealth.StopHiding(); // appear after the strike to ensure sneak attack damage, etc
        Engaged = true;
    }


    // private


    private void SetComponents()
    {
        AttacksPerAction = 1;
        CalculateAdditionalDamage = DefaultAdditionalDamage;
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
