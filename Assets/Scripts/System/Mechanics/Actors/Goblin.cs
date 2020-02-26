using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Goblin : MonoBehaviour
{

    // properties

    public Actor Me { get; set; }
    // Unity

    private void Start()
    {
        SetComponents();
        SetActions();
    }

    // public

    public void OnBadlyInjured()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }

    public void OnFriendsInNeed()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }

    public void OnHasObjective()
    {
        if (!Me.Actions.Decider.Objectives.Any()) {
            Me.Actions.Decider.GoalClaim = null;
            return;
        }

        ClaimNode target_node = Me.Actions.Decider.GoalClaim ?? PickNodeFromObjective(Me.Actions.Decider.Objectives.First());

        if (target_node != null) {
            Me.Actions.Decider.AchievedAllObjectives = false;
            Me.Actions.Decider.GoalClaim = target_node;
            Me.Actions.Movement.SetDestination(target_node.transform.position);
        } else {
            Me.Actions.Decider.AchievedAllObjectives = true;
        }
    }

    public void OnHostileActorsSighted()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }

    public void OnHostileStructuresSighted()
    {
        if (Me.Actions.Decider.HostileStructures.Count > 0) {
            Structure target = Me.Actions.Decider.HostileStructures[Random.Range(0, Me.Actions.Decider.HostileStructures.Count)];
            Me.Actions.Movement.SetDestination(target.GetInteractionPoint(Me));
        }

        Me.Actions.Attack();
    }

    public void OnIdle()
    {
        Me.Actions.SheathWeapon();

        if (Me.Route.local_stops.Length > 1){
            Me.Route.MoveToNextPosition();
        } else if (Me.Actions.Decider.Objectives.Any() && !Me.Actions.Decider.AchievedAllObjectives) {
            PickRandomObjective();
        }
    }


    public void OnInCombat()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }

    public void OnMovingToGoal()
    {
        Me.Actions.SheathWeapon();
    }

    public void OnNeedsRest()
    {
        Me.Actions.SheathWeapon();
        if (Me.Actions.Decider.Objectives.Any()) {
            Me.Actions.Movement.ResetPath();
        } else {
            Me.Actions.Movement.Home();
        }
    }

    private void OnResting()
    {
        Me.Actions.SheathWeapon();

        if (Me.RestCounter == Actor.rested_at) {
            Me.Health.RecoverHealth(Me.Actions.RollDie(Me.Health.LargestHitDie(), 1));
            if (Me.Actions.Magic != null) Me.Actions.Magic.RecoverSpellSlots();
            Me.RestCounter = 0;
        } else {
            Me.RestCounter++;
        }
    }

    public void OnUnderAttack()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
        Me.RestCounter = 0;
    }

    public void OnWatch()
    {
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
            Me.Actions.Decider.GoalClaim = node;
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

   private void SetActions()
    {
        Me.Actions.OnBadlyInjured = OnBadlyInjured;
        // Me.Actions.OnCrafting = OnCrafting;
        // Me.Actions.OnDamagedFriendlyStructuresSighted = OnDamagedFriendlyStructuresSighted;
        // Me.Actions.OnFriendlyActorsSighted = OnFriendlyActorsSighted;
        Me.Actions.OnFriendsInNeed = OnFriendsInNeed;
        // Me.Actions.OnFullLoad = OnFullLoad;
        // Me.Actions.OnHarvesting = OnHarvesting;
        Me.Actions.OnHasObjective = OnHasObjective;
        Me.Actions.OnHostileActorsSighted = OnHostileActorsSighted;
        Me.Actions.OnHostileStructuresSighted = OnHostileStructuresSighted;
        Me.Actions.OnIdle = OnIdle;
        // Me.Actions.OnInCombat = OnInCombat;
        // Me.Actions.OnMedic = OnMedic;
        // Me.Actions.OnMovingToGoal = OnMovingToGoal;
        Me.Actions.OnNeedsRest = OnNeedsRest;
        // Me.Actions.OnReachedGoal = OnReachedGoal;
        Me.Actions.OnResting = OnResting;
        // Me.Actions.OnUnderAttack = OnUnderAttack;
        // Me.Actions.OnWatch = OnWatch;
    }

    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        SetAdditionalStats();
    }


    private void SetAdditionalStats()
    {
        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.Leather));
        Me.Actions.Combat.EquipShield(Armors.Instance.GetArmorNamed(Armors.ArmorName.Shield));
        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Scimitar));
        Me.Actions.Combat.EquipRangedWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Shortbow));
        Me.Senses.DarkVisionRange = 60f;
    }
}
