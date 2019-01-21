﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Commoner : MonoBehaviour 
{
    // properties

    public Actor Me { get; set; }
    public Transform Post { get; set; }


    // Unity


    private void Start()
    {
        SetComponents();
    }


    // public


    public void OnBadlyInjured()
    {
        Me.Actions.Movement.ResetPath();
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.FleeFromEnemies();
    }


    public void OnFriendsInNeed()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack.AttackEnemiesInRange();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public void OnDamagedFriendlyStructuresSighted()
    {
        // repair
    }


    public void OnFullLoad()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        DeliverLoad();
    }


    public void OnHarvesting()
    {
        Me.Actions.Movement.ResetPath();
        Harvest();
    }


    public void OnHostileActorsSighted()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.CallForHelp();
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed * 2;
        AbandonLoad();
        FindGuard();
    }


    public void OnHostileStructuresSighted()
    {
        Me.Actions.CallForHelp();
        Me.Actions.FleeFromEnemies();
    }


    public void OnInCombat()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    public void OnIdle()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.SheathWeapon();

        // TODO: if badly injured, go to center of town

        if (Proficiencies.Instance.Harvester(Me)) {
            GoToWork();
        } else if (Proficiencies.Instance.Artisan(Me)) {
            GoHome();
        }
    }


    public void OnManufacturing()
    {
        // TODO: advance manufacturing stage
        ReturnToPost();
    }


    public void OnMovingToGoal()
    {
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Senses.Sight();
    }


    public void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.Decider.FriendsInNeed.Clear();

        if (!Transact()) {
            if (!Harvest()) {
                OnIdle();
            }
        }
    }


    public void OnUnderAttack()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        Me.Actions.Attack.AttackEnemiesInRange();
    }


    public void OnWatch()
    {
        // call for help after running away
    }


    // private


    private void AbandonLoad()
    {
        Me.Load.Clear();
        Me.harvesting = Resources.Raw.None;
    }


    private void DeliverLoad()
    {
        Structure nearest_commercial_structure = new List<Structure>(FindObjectsOfType<Structure>())
            .Where(s => s.owner == Me.Faction && s.Storage != null && s.MaterialsWanted().Contains(Me.Load.First().Key.raw_resource))
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        Me.Actions.Movement.SetDestination(nearest_commercial_structure.NearestEntranceTo(transform));
    }


    private void FindGuard()
    {
        Structure nearest_military_structure = new List<Structure>(FindObjectsOfType<Structure>())
            .Where(s => s.owner == Me.Faction && s.purpose == Structure.Purpose.Military)
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        Me.Actions.Movement.SetDestination(nearest_military_structure.NearestEntranceTo(transform));
    }


    private void GoHome()
    {
        Structure nearest_residential_structure = new List<Structure>(FindObjectsOfType<Structure>())
            .Where(s => s.owner == Me.Faction && s.purpose == Structure.Purpose.Residential)
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        Me.Actions.Movement.SetDestination(nearest_residential_structure.NearestEntranceTo(transform));
    }


    private void GoToWork()
    {
        var harvest_points = new List<HarvestingNode>(FindObjectsOfType<HarvestingNode>())
            .Where(r => r.owner == Me.Faction && r.AccessibleTo(Me))
            .ToList();

        HarvestingNode _resource = harvest_points[Random.Range(0, harvest_points.Count)];

        Me.Actions.Movement.SetDestination(_resource.transform);
    }


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
        Me.Actions.Movement.SetDestination(nearest_harvest.transform);

        return false;
    }


    private void ReturnToPost()
    {
        float distance = Vector3.Distance(transform.position, Post.position);

        if (distance > 0.01)
        {
            Me.Actions.Movement.Agent.destination = Post.position;
        }
    }


    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        StartCoroutine(Me.GetStatsFromServer(this.GetType().Name));
        SetAdditionalStats();

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


    private void SetAdditionalStats()
    {
        Me.Actions.Attack.AvailableWeapons = Characters.available_weapons[Characters.Template.Commoner];
        Me.Senses.Darkvision = Characters.darkvision_range[Characters.Template.Base];
        Me.Senses.PerceptionRange = Characters.perception_range[Characters.Template.Base];
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
    }


    private bool Transact()
    {
        if (Me.Load.Count <= 0) return false;

        Structure nearest_structure = new List<Structure>(FindObjectsOfType<Structure>())
            .Where(s => s.owner == Me.Faction && s.Storage != null && s.MaterialsWanted().Contains(Me.Load.First().Key.raw_resource))
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        Collider _collider = nearest_structure.GetComponent<Collider>();
        Vector3 closest_spot = _collider.ClosestPointOnBounds(transform.position);
        float distance = Vector3.Distance(closest_spot, transform.position) - Me.Size;

        if (distance <= Movement.ReachedThreshold) {
            nearest_structure.DeliverMaterials(Me, Random.Range(1, 12) * .1f); // TODO: base amount on resources!
            return true;
        }

        return false;
    }
}
