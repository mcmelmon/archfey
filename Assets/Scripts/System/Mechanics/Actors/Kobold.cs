using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Kobold : TemplateMelee
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
        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.None));
        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Scimitar));
        Me.Actions.Combat.EquipRangedWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Hand_Crossbow)); 
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
    }
}
