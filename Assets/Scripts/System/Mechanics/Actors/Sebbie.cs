using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sebbie : TemplateMelee
{
    // properties

    public RayOfFrost RayOfFrost { get; set; }


    // Unity


    private void Start()
    {
        SetStats();
    }


    // public

    public override void OnHostileActorsSighted()
    {
        Me.Actions.KeepEnemiesAtRange();
        AttackWithSpell();
        Me.RestCounter = 0;
    }


    public override void OnInCombat()
    {
        Me.Actions.KeepEnemiesAtRange();
        AttackWithSpell();
        Me.RestCounter = 0;
    }


    public override void OnUnderAttack()
    {
        Me.Actions.KeepEnemiesAtRange();
        AttackWithSpell();
        Me.RestCounter = 0;
    }


    // private


    private void AttackWithSpell()
    {
        Actor target = Me.Actions.Decider.Threat.PrimaryThreat();

        if (target != null && Vector3.Distance(target.transform.position, transform.position) < RayOfFrost.Range) {
            RayOfFrost.Cast(target);
        }
    }


    private void SetStats()
    {
        Me = GetComponent<Actor>();
        StartCoroutine(Me.GetStatsFromServer(this.GetType().Name));
        SetAdditionalStats();
    }


    private void SetAdditionalStats()
    {
        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.Leather));
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
        RayOfFrost = gameObject.AddComponent<RayOfFrost>();
        Me.Actions.Combat.CombatSpells.Add(Weapons.Instance.GetSpellNamed(Spell.SpellName.RayOfFrost));
    }
}
