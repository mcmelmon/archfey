using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gnoll : MonoBehaviour
{
    // properties

    public Actor Actor { get; set; }

    // Unity


    private void Start()
    {
        SetStats();
    }


    // public

    public void OnAlliesUnderAttack()
    {
        Actor.Actions.CloseWithEnemies();
    }


    public void OnContestingObjective()
    {
        Actor.Actions.Movement.Route = null;
        Actor.Actions.Movement.ResetPath();
    }


    public void OnBadlyInjured()
    {
        Actor.Actions.CloseWithEnemies();
    }


    public void OnHasObjective()
    {
        Actor.Actions.Movement.Advance();
    }


    public void OnHostilesSighted()
    {
        Actor.Actions.CloseWithEnemies();
    }


    public void OnIdle()
    {
        List<Objective> objectives = Objectives.HeldByFaction[Conflict.Instance.EnemyFaction(Actor)];

        Actor.Actions.Movement.SetRoute(Route.Linear(transform.position, objectives[Random.Range(0, objectives.Count)].control_points[0].transform.position, Actor.Actions.Decider.FinishedRoute));
    }


    public void OnInCombat()
    {
        Actor.Actions.CloseWithEnemies();
    }


    public void OnUnderAttack()
    {
        Actor.Actions.CloseWithEnemies();
    }


    public void OnWatch()
    {
        Actor.Actions.CloseWithEnemies();
    }


    // private


    private void SetStats()
    {
        // can't do in Actor until the Gnoll component has been attached

        Actor = GetComponent<Actor>();
        SetBaseStats();

        Actor.Actions.Attack.EquipMeleeWeapon();
        Actor.Actions.Attack.EquipRangedWeapon();
        Actor.Actions.OnAlliesUnderAttack = OnAlliesUnderAttack;
        Actor.Actions.OnContestingObjective = OnContestingObjective;
        Actor.Actions.OnBadlyInjured = OnBadlyInjured;
        Actor.Actions.OnHasObjective = OnHasObjective;
        Actor.Actions.OnHostilesSighted = OnHostilesSighted;
        Actor.Actions.OnIdle = OnIdle;
        Actor.Actions.OnInCombat = OnInCombat;
        Actor.Actions.OnUnderAttack = OnUnderAttack;
        Actor.Actions.OnWatch = OnWatch;

        Actor.Health.SetCurrentAndMaxHitPoints();  // calculated from hit dice and constitution, set in base stats
    }


    private void SetBaseStats()
    {
        Actor.Actions.ObjectiveControlRating = Characters.objective_control_rating[Characters.Template.Gnoll];

        Actor.Actions.Attack.AvailableWeapons = Characters.available_weapons[Characters.Template.Gnoll];

        Actor.Actions.Defend.ArmorClass = Characters.armor_class[Characters.Template.Gnoll];
        Actor.Actions.Defend.SetResistances(Characters.resistances[Characters.Template.Base]);

        Actor.Health.HitDice = (Characters.hit_dice[Characters.Template.Gnoll]);
        Actor.Health.HitDiceType = (Characters.hit_dice_type[Characters.Template.Gnoll]);

        Actor.Actions.Movement.Speed = Characters.speed[Characters.Template.Base];
        Actor.Actions.Movement.Agent.speed = Characters.speed[Characters.Template.Base];

        Actor.Senses.Darkvision = Characters.darkvision_range[Characters.Template.Gnoll];
        Actor.Senses.PerceptionRange = Characters.perception_range[Characters.Template.Base];

        Actor.Stats.CharismaProficiency = Characters.charisma_proficiency[Characters.Template.Gnoll];
        Actor.Stats.ConstitutionProficiency = Characters.constituion_proficiency[Characters.Template.Base];
        Actor.Stats.DexterityProficiency = Characters.dexterity_proficiency[Characters.Template.Gnoll];
        Actor.Stats.IntelligenceProficiency = Characters.intelligence_proficiency[Characters.Template.Gnoll];
        Actor.Stats.StrengthProficiency = Characters.strength_proficiency[Characters.Template.Gnoll];
        Actor.Stats.WisdomProficiency = Characters.wisdom_proficiency[Characters.Template.Base];
    }
}
