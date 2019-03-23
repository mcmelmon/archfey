using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bugbear : TemplateVersatile
{
    // Unity


    private void Start()
    {
        SetStats();
    }


    // public


    public int AdditionalDamage(GameObject target, bool is_ranged)
    {
        Actor victim = target.GetComponent<Actor>();

        int additional_damage = Me.Actions.Combat.HasSurprise(victim) ? Me.Actions.RollDie(6, 2) : 0;
        return is_ranged ? additional_damage : additional_damage + Me.Actions.RollDie(Me.Actions.Combat.EquippedMeleeWeapon.DiceType, 1);
    }


    public override void OnBadlyInjured()
    {
        Me.Actions.FleeFromEnemies();
    }


    public override void OnIdle()
    {
        Me.Actions.Stealth.Hide();
        base.OnIdle();
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
        Me.Actions.Combat.EquipShield(Armors.Instance.GetArmorNamed(Armors.ArmorName.Shield));
        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Morningstar));
        Me.Actions.Combat.EquipRangedWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Javelin));
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
        Me.Actions.Combat.CalculateAdditionalDamage = AdditionalDamage;

        Me.Stats.ExpertiseInSkills.Add(Proficiencies.Skill.Stealth);
        Me.Stats.Skills.Add(Proficiencies.Skill.Stealth);
        Me.Stats.Skills.Add(Proficiencies.Skill.Survival);
    }
}
