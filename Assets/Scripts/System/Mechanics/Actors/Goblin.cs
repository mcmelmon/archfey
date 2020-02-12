using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Goblin : TemplateMelee
{
    // Unity


    private void Start()
    {
        SetStats();
    }


    // public



    // private


    private void SetStats()
    {
        Me = GetComponent<Actor>();
        SetAdditionalStats();
    }


    private void SetAdditionalStats()
    {
        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.Leather));
        Me.Actions.Combat.EquipShield(Armors.Instance.GetArmorNamed(Armors.ArmorName.Shield));
        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Scimitar));
        Me.Actions.Combat.EquipRangedWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Shortbow));
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
    }
}
