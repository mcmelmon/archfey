using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Commoner : MonoBehaviour 
{
    // properties

    public Actor Me { get; set; }


    // Unity


    private void Start()
    {
        SetComponents();
    }


    // public


    public void OnBadlyInjured()
    {

    }


    public void OnFriendsInNeed()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public void OnDamagedFriendlyStructuresSighted()
    {
        Me.Actions.CallForHelp();
        // repair
    }


    public void OnFullLoad()
    {
        Transact();

        if (Me.Load.Keys.Count == 0) return;
        
        Structure nearest_commercial_structure = new List<Structure>(FindObjectsOfType<Structure>())
            .Where(s => s.owner == Me.Faction && s.Wants().Contains(Me.Load.First().Key.raw_resource))
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        Me.Actions.Movement.SetDestination(nearest_commercial_structure.gameObject);
    }


    public void OnHarvesting()
    {
        Me.Actions.Movement.ResetPath();
        Harvest();
    }


    public void OnHostileActorsSighted()
    {
        Me.Actions.FleeFromEnemies();
        Me.Actions.CallForHelp();
    }


    public void OnHostileStructuresSighted()
    {
        Me.Actions.FleeFromEnemies();
    }


    public void OnInCombat()
    {
        Me.Actions.FleeFromEnemies();
    }


    public void OnIdle()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.SheathWeapon();

        var harvest_points = new List<HarvestingNode>(FindObjectsOfType<HarvestingNode>())
            .Where(r => r.owner == Me.Faction && r.AccessibleTo(Me))
            .ToList();

        HarvestingNode _resource = harvest_points[Random.Range(0, harvest_points.Count)];

        Me.Actions.Movement.SetDestination(_resource.gameObject);
    }


    public void OnManufacturing()
    {
        // TODO: advance manufacturing stage
    }


    public void OnMovingToGoal()
    {
        Me.Senses.Sight();
    }


    public void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();

        if (!Transact()) {
            if (!Harvest()) {
                OnIdle();
            }
        }
    }


    public void OnUnderAttack()
    {
        Me.Actions.FleeFromEnemies();
    }


    public void OnWatch()
    {
        // call for help after running away
    }


    // private


    private bool Harvest()
    {
        var nearest_harvest = new List<HarvestingNode>(FindObjectsOfType<HarvestingNode>())
            .Where(r => r.owner == Me.Faction)
            .OrderBy(r => Vector3.Distance(transform.position, r.transform.position))
            .ToList()
            .First();

        Collider _collider = nearest_harvest.GetComponent<Collider>();
        Vector3 closest_spot = _collider.ClosestPointOnBounds(transform.position);
        float distance = Vector3.Distance(closest_spot, transform.position);

        if (distance <= Movement.ReachedThreshold) {
            nearest_harvest.HarvestResource(Me);
            Me.harvesting = nearest_harvest.raw_resource;
            Me.harvested_amount = Me.Load.First().Value;
            return true;
        }

        // We've gotten bumped away from our harvest node
        Me.Actions.Movement.SetDestination(nearest_harvest.gameObject);

        return false;
    }


    private void SetComponents()
    {
        // can't do in Actor until the Commoner component has been attached
        Me = GetComponent<Actor>();
        SetBaseStats();

        Me.Actions.Attack.EquipMeleeWeapon();
        Me.Actions.Attack.EquipRangedWeapon();

        Me.Actions.OnBadlyInjured = OnBadlyInjured;
        Me.Actions.OnFriendsInNeed = OnFriendsInNeed;
        Me.Actions.OnFullLoad = OnFullLoad;
        Me.Actions.OnDamagedFriendlyStructuresSighted = OnDamagedFriendlyStructuresSighted;
        Me.Actions.OnHarvetsing = OnHarvesting;
        Me.Actions.OnHostileActorsSighted = OnHostileActorsSighted;
        Me.Actions.OnHostileStructuresSighted = OnHostileStructuresSighted;
        Me.Actions.OnIdle = OnIdle;
        Me.Actions.OnInCombat = OnInCombat;
        Me.Actions.OnManufacturing = OnManufacturing;
        Me.Actions.OnMovingToGoal = OnMovingToGoal;
        Me.Actions.OnReachedGoal = OnReachedGoal;
        Me.Actions.OnUnderAttack = OnUnderAttack;
        Me.Actions.OnWatch = OnWatch;

        Me.Health.SetCurrentAndMaxHitPoints();
    }


    private void SetBaseStats()
    {
        Me.Actions.ActionsPerRound = Characters.actions_per_round[Characters.Template.Base];

        Me.Actions.Attack.AvailableWeapons = Characters.available_weapons[Characters.Template.Commoner];

        Me.Health.HitDice = Characters.hit_dice[Characters.Template.Base];
        Me.Health.HitDiceType = Characters.hit_dice_type[Characters.Template.Base];

        Me.Actions.Movement.Speed = Characters.speed[Characters.Template.Base];
        Me.Actions.Movement.Agent.speed = Characters.speed[Characters.Template.Base];

        Me.Senses.Darkvision = Characters.darkvision_range[Characters.Template.Base];
        Me.Senses.PerceptionRange = Characters.perception_range[Characters.Template.Base];

        Me.Stats.ArmorClass = Characters.armor_class[Characters.Template.Base];
        Me.Stats.CharismaProficiency = Characters.charisma_proficiency[Characters.Template.Base];
        Me.Stats.ConstitutionProficiency = Characters.constituion_proficiency[Characters.Template.Base];
        Me.Stats.DexterityProficiency = Characters.dexterity_proficiency[Characters.Template.Base];
        Me.Stats.IntelligenceProficiency = Characters.intelligence_proficiency[Characters.Template.Base];
        Me.Stats.StrengthProficiency = Characters.strength_proficiency[Characters.Template.Base];
        Me.Stats.WisdomProficiency = Characters.wisdom_proficiency[Characters.Template.Base];

        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
        Me.Stats.ProficiencyBonus = Characters.proficiency_bonus[Characters.Template.Base];
    }


    private bool Transact()
    {
        if (Me.Load.Count <= 0) return false;

        Structure nearest_structure = new List<Structure>(FindObjectsOfType<Structure>())
            .Where(s => s.owner == Me.Faction && s.Wants().Contains(Me.Load.First().Key.raw_resource))
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        Collider _collider = nearest_structure.GetComponent<Collider>();
        Vector3 closest_spot = _collider.ClosestPointOnBounds(transform.position);
        float distance = Vector3.Distance(closest_spot, transform.position) - Me.Size;

        if (distance <= Movement.ReachedThreshold) {
            nearest_structure.TransactBusiness(Me, Random.Range(1, 12) * .1f); // TODO: base amount on resources!
            return true;
        }

        return false;
    }
}
