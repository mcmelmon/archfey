using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commoner : MonoBehaviour {

    // properties

    public Actor Actor { get; set; }


    // Unity


    private void Start()
    {
        SetComponents();
    }


    // public


    public void OnAlliesUnderAttack()
    {
        Actor.Actions.FleeFromEnemies();
    }


    public void OnBadlyInjured()
    {

    }


    public void OnHasObjective()
    {
        Actor.Actions.Movement.Advance();
    }


    public void OnHostilesSighted()
    {
        Actor.Actions.FleeFromEnemies();
    }


    public void OnInCombat()
    {
        Actor.Actions.FleeFromEnemies();
    }


    public void OnIdle()
    {
        Actor.Actions.Movement.Agent.speed = Actor.Actions.Movement.Speed;
        Actor.Actions.SheathWeapon();

        List<Objective> objectives = Objectives.HeldByFaction[Actor.Faction];
        Objective next_objective = objectives[Random.Range(0, objectives.Count)];

        if (Actor.Actions.Movement.Route == null) {
            Actor.Actions.Movement.SetRoute(Route.Linear(transform.position, next_objective.control_points[0].transform.position, Actor.Actions.Decider.FinishedRoute));
        }
    }


    public void OnUnderAttack()
    {
        Actor.Actions.FleeFromEnemies();
    }


    public void OnWatch()
    {
        // call for help after running away
    }


    // private


    private void SetComponents()
    {
        // can't do in Actor until the Commoner component has been attached
        Actor = GetComponent<Actor>();
        SetBaseStats();

        Actor.Actions.Attack.EquipMeleeWeapon();
        Actor.Actions.Attack.EquipRangedWeapon();
        Actor.Actions.OnAlliesUnderAttack = OnAlliesUnderAttack;
        Actor.Actions.OnBadlyInjured = OnBadlyInjured;
        Actor.Actions.OnHasObjective = OnHasObjective;
        Actor.Actions.OnHostilesSighted = OnHostilesSighted;
        Actor.Actions.OnIdle = OnIdle;
        Actor.Actions.OnInCombat = OnInCombat;
        Actor.Actions.OnUnderAttack = OnUnderAttack;
        Actor.Actions.OnWatch = OnWatch;

        Actor.Health.SetCurrentAndMaxHitPoints();
    }


    private void SetBaseStats()
    {
        Actor.Actions.ActionsPerRound = Characters.actions_per_round[Characters.Template.Base];
        Actor.Actions.ObjectiveControlRating = Characters.objective_control_rating[Characters.Template.Commoner];

        Actor.Actions.Attack.AvailableWeapons = Characters.available_weapons[Characters.Template.Commoner];

        Actor.Actions.Defend.ArmorClass = Characters.armor_class[Characters.Template.Base];
        Actor.Actions.Defend.SetResistances(Characters.resistances[Characters.Template.Base]);

        Actor.Health.HitDice = Characters.hit_dice[Characters.Template.Base];
        Actor.Health.HitDiceType = Characters.hit_dice_type[Characters.Template.Base];

        Actor.Actions.Movement.Speed = Characters.speed[Characters.Template.Base];
        Actor.Actions.Movement.Agent.speed = Characters.speed[Characters.Template.Base];

        Actor.Senses.Darkvision = Characters.darkvision_range[Characters.Template.Base];
        Actor.Senses.PerceptionRange = Characters.perception_range[Characters.Template.Base];

        Actor.Stats.CharismaProficiency = Characters.charisma_proficiency[Characters.Template.Base];
        Actor.Stats.ConstitutionProficiency = Characters.constituion_proficiency[Characters.Template.Base];
        Actor.Stats.DexterityProficiency = Characters.dexterity_proficiency[Characters.Template.Base];
        Actor.Stats.IntelligenceProficiency = Characters.intelligence_proficiency[Characters.Template.Base];
        Actor.Stats.StrengthProficiency = Characters.strength_proficiency[Characters.Template.Base];
        Actor.Stats.WisdomProficiency = Characters.wisdom_proficiency[Characters.Template.Base];
    }
}
