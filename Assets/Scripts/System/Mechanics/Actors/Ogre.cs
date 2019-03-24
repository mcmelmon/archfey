﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ogre : TemplateMelee
{

    // Unity


    private void Start()
    {
        SetStats();
    }


    // public


    public int AdditionalDamage(GameObject target, bool is_ranged)
    {
        return is_ranged ? Me.Actions.RollDie(Me.Actions.Combat.EquippedRangedWeapon.DiceType, 1) : Me.Actions.RollDie(Me.Actions.Combat.EquippedMeleeWeapon.DiceType, 1);
    }


    // private


    private void SetStats()
    {
        Me = GetComponent<Actor>();
        StartCoroutine(Me.GetStatsFromServer(this.GetType().Name));
        SetAdditionalStats();
    }


    private void SetAdditionalStats()
    {
        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.Hide));
        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Greataxe));
        Me.Actions.Combat.EquipRangedWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Javelin)); 
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
        Me.Actions.Combat.CalculateAdditionalDamage = AdditionalDamage;

        GameObject pocket_lint = new GameObject("Pocket Lint");
        pocket_lint.transform.parent = transform;
        Me.Inventory.Pockets.Add(pocket_lint);
    }
}
