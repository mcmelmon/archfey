using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TemplateMelee : MonoBehaviour, IAct
{
    // properties

    public Actor Me { get; set; }


    // public


    public virtual void OnBadlyInjured()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }


    public virtual void OnCrafting() 
    { 
        OnIdle(); 
    }


    public virtual void OnDamagedFriendlyStructuresSighted()
    { 
        OnIdle(); 
    }


    public virtual void OnFriendlyActorsSighted()
    { 
        OnIdle(); 
    }


    public virtual void OnFriendsInNeed()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }


    public virtual void OnFullLoad() 
    { 
        OnIdle(); 
    }


    public virtual void OnHarvesting()
    { 
        OnIdle();
    }


    public virtual void OnHasObjective()
    {
        ClaimNode target_node = null;

        List<ClaimNode> target_nodes = FindObjectsOfType<ClaimNode>()
            .Where(claim_node => Me.Actions.Decider.Objectives.Contains(claim_node.Objective) && (claim_node.CurrentClaimPercentage() < 1f) || claim_node.ClaimFaction != Me.Faction)
            .OrderBy(claim_node => Vector3.Distance(transform.position, claim_node.transform.position))
            .ToList();

        if (target_nodes.Any()) {
            target_node = target_nodes.First();
        }

        if (target_node != null) {
            Me.Actions.Decider.AchievedAllObjectives = false;
            Me.Actions.Movement.SetDestination(target_node.transform.position);
        } else {
            Me.Actions.Decider.AchievedAllObjectives = true;
        }
    }


    public virtual void OnHostileActorsSighted()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }


    public virtual void OnHostileStructuresSighted()
    {
        if (Me.Actions.Decider.HostileStructures.Count > 0) {
            Structure target = Me.Actions.Decider.HostileStructures[Random.Range(0, Me.Actions.Decider.HostileStructures.Count)];
            Me.Actions.Movement.SetDestination(target.GetInteractionPoint(Me));
        }

        Me.Actions.Attack();
    }


    public virtual void OnIdle()
    {
        Me.Actions.SheathWeapon();

        if (Me.Route.local_stops.Length > 1){
            Me.Route.MoveToNextPosition();
        } else if (Me.Actions.Movement.Destinations.ContainsKey(Movement.CommonDestination.Home)) {
            Me.Actions.Movement.Home(); 
        }
    }


    public virtual void OnInCombat()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }


    public virtual void OnMedic() { }


    public virtual void OnMovingToGoal()
    {
        Me.Actions.SheathWeapon();
    }


    public virtual void OnNeedsRest()
    {
        Me.Actions.SheathWeapon();
        if (Me.Actions.Decider.Objectives.Any()) {
            Me.Actions.Movement.ResetPath();
        } else {
            Me.Actions.Movement.Home();
        }
    }


    public virtual void OnPerformingTask() { }


    public virtual void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();
        OnIdle();
    }


    public virtual void OnUnderAttack()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
        Me.RestCounter = 0;
    }


    public virtual void OnWatch()
    {
        Me.Actions.Movement.ResetPath();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }
}
