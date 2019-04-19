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
        if (!Me.Actions.Decider.Objectives.Any()) {
            Me.Actions.Decider.Goal = null;
            return;
        }

        ClaimNode target_node = Me.Actions.Decider.Goal ?? PickNodeFromObjective(Me.Actions.Decider.Objectives.First());

        if (target_node != null) {
            Me.Actions.Decider.AchievedAllObjectives = false;
            Me.Actions.Decider.Goal = target_node;
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
        } else if (Me.Actions.Decider.Objectives.Any() && !Me.Actions.Decider.AchievedAllObjectives) {
            PickRandomObjective();
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


    // private


    private void PickRandomObjective()
    {
        ClaimNode node = null;

        List<Objective> available_objectives = FindObjectsOfType<Objective>()
            .Where(objective => !objective.Claimed || objective.ClaimingFaction.IsHostileTo(Me.CurrentFaction))
            .ToList();

        if (available_objectives.Any()) {
            node = PickNodeFromObjective(available_objectives[Random.Range(0, available_objectives.Count)]);
        }

        if (node != null) {
            Me.Actions.Decider.Goal = node;
            Me.Actions.Movement.SetDestination(node.transform.position);
        } else {
            Me.Actions.Movement.Home();
        }
    }


    private ClaimNode PickNodeFromObjective(Objective objective)
    {
        List<ClaimNode> target_nodes = objective.claim_nodes
            .Where(node => (!node.Claimed || (node.NodeFaction != null && node.NodeFaction.IsHostileTo(Me.CurrentFaction))))
            .OrderBy(node => Vector3.Distance(transform.position, node.transform.position))
            .ToList();

        return target_nodes.FirstOrDefault();
    }
}
