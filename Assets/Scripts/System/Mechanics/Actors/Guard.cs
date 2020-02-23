using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Guard : MonoBehaviour
{

    // properties

    public Actor Me { get; set; }
    // Unity

    private void Start()
    {
        SetStats();
        SetActions();
    }

    // public

    public void OnBadlyInjured()
    {
        Me.Actions.Attack();
        FindShrine();
    }

    public void OnFriendsInNeed()
    {
        Me.Actions.Movement.SetDestination(Me.Actions.Decider.FriendsInNeed.First().transform);
        Me.Actions.Attack();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }

    public void OnInCombat()
    {
        Me.Actions.CallForHelp();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }

    public void OnIdle()
    {
        Me.Actions.SheathWeapon();
        Me.Actions.Movement.Home();
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
            if (Me.Magic != null) Me.Magic.RecoverSpellLevels();
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

    private void FindShrine()
    {
        List<Structure> available_sacred_structures = FindObjectsOfType<Structure>().Where(s => s.IsOpenToMe(Me) && s.Use == Structure.Purpose.Sacred).ToList();
        if (available_sacred_structures.Any()) {
            Me.Actions.Movement.SetDestination(available_sacred_structures.OrderBy(s => Vector3.Distance(transform.position, s.transform.position)).ToList().First().transform);
        } else {
            Me.Actions.CloseWithEnemies();
        }
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
        // Me.Actions.OnHasObjective = OnHasObjective;
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

    private void SetStats()
    {
        Me = GetComponent<Actor>();
        SetAdditionalStats();
    }

    private void SetAdditionalStats()
    {
        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.Chain_Shirt));
        Me.Actions.Combat.EquipShield(Armors.Instance.GetArmorNamed(Armors.ArmorName.Shield));
        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Spear));
        Me.Actions.Combat.EquipRangedWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Longbow)); 
    }
}