﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Goblin : MonoBehaviour, IAct
{
    // properties

    public Actor Me { get; set; }

    // Unity


    private void Start()
    {
        SetStats();
    }


    // public


    public void OnBadlyInjured()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }


    public void OnCrafting() {  }


    public void OnFriendlyActorsSighted() { }

    public void OnDamagedFriendlyStructuresSighted() { }


    public void OnFriendsInNeed()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }


    public void OnFullLoad() { }


    public void OnHarvesting() { }


    public void OnHostileActorsSighted()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }


    public void OnHostileStructuresSighted()
    {
        if (Me.Actions.Decider.HostileStructures.Count > 0)
        {
            Structure target = Me.Actions.Decider.HostileStructures[Random.Range(0, Me.Actions.Decider.HostileStructures.Count)];
            Me.Actions.Movement.SetDestination(target.GetInteractionPoint(Me));
        }

        Me.Actions.Attack();
    }


    public void OnIdle()
    {
        Me.Actions.SheathWeapon();
        Me.Actions.Stealth.Hide();

        if (Me.Route.local_stops.Length > 1)
        {
            Me.Route.MoveToNextPosition();
        }
        else
        {
            List<Objective> objectives = FindObjectsOfType<Objective>().Where(objective => objective.Claim == Conflict.Instance.EnemyFaction(Me)).ToList();
            if (objectives.Count > 0)
            {
                Objective next_objective = objectives[Random.Range(0, objectives.Count)];
                Me.Actions.Movement.SetDestination(next_objective.claim_nodes[0].transform);
            }
        }
    }


    public void OnInCombat()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }


    public void OnMedic() { }


    public void OnMovingToGoal()
    {
        Me.Actions.SheathWeapon();
    }


    public void OnNeedsRest() { }


    public void OnPerformingTask() { }


    public void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();
        OnIdle();
    }


    public void OnUnderAttack()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
        Me.RestCounter = 0;
    }


    public void OnWatch()
    {
        Me.Actions.Movement.ResetPath();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }


    // private


    private void SetStats()
    {
        Me = GetComponent<Actor>();
        StartCoroutine(Me.GetStatsFromServer(this.GetType().Name));
        SetAdditionalStats();
        Me.Actions.Movement.AddDestination(Movement.CommonDestination.Home, transform.position);
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
