﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gnoll : TemplateMelee
{
    // Unity


    private void Start()
    {
        SetStats();
    }


    // private


    private void SetStats()
    {
        Me = GetComponent<Actor>();
        SetAdditionalStats();
    }


    private void SetAdditionalStats()
    {
        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.Hide));
        Me.Actions.Combat.EquipShield(Armors.Instance.GetArmorNamed(Armors.ArmorName.Shield));
        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Spear));
        Me.Actions.Combat.EquipRangedWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Longbow));
    }
}
