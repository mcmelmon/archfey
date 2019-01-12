using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gnoll : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }

    // Unity


    private void Start()
    {
        SetStats();
    }


    // public


    public void OnAlliesUnderAttack()
    {
        Me.Actions.CloseWithEnemies();
    }


    public void OnBadlyInjured()
    {
        Me.Actions.CloseWithEnemies();
    }


    public void OnHostilesSighted()
    {
        Me.Actions.CloseWithEnemies();
    }


    public void OnIdle()
    {
        var structures = FindObjectsOfType<Structure>();

        Me.Actions.Movement.SetDestination(structures[Random.Range(0, structures.Length)].transform.position);
    }


    public void OnInCombat()
    {
        Me.Actions.CloseWithEnemies();
    }


    public void OnMovingToGoal()
    {
        Me.Actions.Movement.Advance();
    }


    public void OnPerformingTask()
    {

    }


    public void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();
        OnIdle();
    }


    public void OnUnderAttack()
    {
        Me.Actions.CloseWithEnemies();
    }


    public void OnWatch()
    {
        Me.Actions.CloseWithEnemies();
    }


    // private


    private void SetStats()
    {
        // can't do in Actor until the Gnoll component has been attached

        Me = GetComponent<Actor>();
        SetBaseStats();

        Me.Actions.Attack.EquipMeleeWeapon();
        Me.Actions.Attack.EquipRangedWeapon();
        Me.Actions.OnAlliesUnderAttack = OnAlliesUnderAttack;
        Me.Actions.OnBadlyInjured = OnBadlyInjured;
        Me.Actions.OnHostilesSighted = OnHostilesSighted;
        Me.Actions.OnIdle = OnIdle;
        Me.Actions.OnInCombat = OnInCombat;
        Me.Actions.OnMovingToGoal = OnMovingToGoal;
        Me.Actions.OnPerformingTask = OnPerformingTask;
        Me.Actions.OnReachedGoal = OnReachedGoal;
        Me.Actions.OnUnderAttack = OnUnderAttack;
        Me.Actions.OnWatch = OnWatch;

        Me.Health.SetCurrentAndMaxHitPoints();  // calculated from hit dice and constitution, set in base stats
    }


    private void SetBaseStats()
    {
        Me.Actions.ClaimRating = Characters.claim_rating[Characters.Template.Gnoll];

        Me.Actions.Attack.AvailableWeapons = Characters.available_weapons[Characters.Template.Gnoll];

        Me.Actions.Defend.ArmorClass = Characters.armor_class[Characters.Template.Gnoll];
        Me.Actions.Defend.SetResistances(Characters.resistances[Characters.Template.Base]);

        Me.Health.HitDice = (Characters.hit_dice[Characters.Template.Gnoll]);
        Me.Health.HitDiceType = (Characters.hit_dice_type[Characters.Template.Gnoll]);

        Me.Actions.Movement.Speed = Characters.speed[Characters.Template.Base];
        Me.Actions.Movement.Agent.speed = Characters.speed[Characters.Template.Base];

        Me.Senses.Darkvision = Characters.darkvision_range[Characters.Template.Gnoll];
        Me.Senses.PerceptionRange = Characters.perception_range[Characters.Template.Base];

        Me.Stats.CharismaProficiency = Characters.charisma_proficiency[Characters.Template.Gnoll];
        Me.Stats.ConstitutionProficiency = Characters.constituion_proficiency[Characters.Template.Base];
        Me.Stats.DexterityProficiency = Characters.dexterity_proficiency[Characters.Template.Gnoll];
        Me.Stats.IntelligenceProficiency = Characters.intelligence_proficiency[Characters.Template.Gnoll];
        Me.Stats.StrengthProficiency = Characters.strength_proficiency[Characters.Template.Gnoll];
        Me.Stats.WisdomProficiency = Characters.wisdom_proficiency[Characters.Template.Base];
    }
}
