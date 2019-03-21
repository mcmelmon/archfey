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
        } else {
            List<Objective> objectives = FindObjectsOfType<Objective>().Where(objective => objective.Claim == Conflict.Instance.EnemyFaction(Me)).ToList();
            if (objectives.Count > 0) {
                Objective next_objective = objectives[Random.Range(0, objectives.Count)];
                Me.Actions.Movement.SetDestination(next_objective.claim_nodes[0].transform);
            }
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
