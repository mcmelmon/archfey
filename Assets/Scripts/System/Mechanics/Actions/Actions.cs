using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actions : MonoBehaviour
{
    // properties

    public int ActionsPerRound { get; set; }
    public Attack Attack { get; set; }
    public Decider Decider { get; set; }
    public Actor Me { get; set; }
    public Movement Movement { get; set; }
    public Stats Stats { get; set; }
    public Stealth Stealth { get; set; }
    public bool CanTakeTurn { get; set; }

    public Action OnBadlyInjured { get; set; }
    public Action OnCrafting { get; set; }
    public Action OnFriendsInNeed { get; set; }
    public Action OnFriendlyActorsSighted { get; set; }
    public Action OnFullLoad { get; set; }
    public Action OnDamagedFriendlyStructuresSighted { get; set; }
    public Action OnHarvetsing { get; set; }
    public Action OnHostileActorsSighted { get; set; }
    public Action OnHostileStructuresSighted { get; set; }
    public Action OnIdle { get; set; }
    public Action OnInCombat { get; set; }
    public Action OnMedic { get; set; }
    public Action OnMovingToGoal { get; set; }
    public Action OnNeedsRest { get; set; }
    public Action OnReachedGoal { get; set; }
    public Action OnUnderAttack { get; set; }
    public Action OnWatch { get; set; }
    

    // Unity


    void Awake()
    {
        SetComponents();
    }


    // public


    public void ActOnTurn()
    {
        CanTakeTurn |= Me.gameObject.tag == "Player";

        Decider.ChooseState();

        switch (Decider.state) {
            case Decider.State.BadlyInjured:
                OnBadlyInjured?.Invoke();
                break;
            case Decider.State.Crafting:
                OnCrafting?.Invoke();
                break;
            case Decider.State.FriendsInNeed:
                OnFriendsInNeed?.Invoke();
                break;
            case Decider.State.FriendlyActorsSighted:
                OnFriendlyActorsSighted?.Invoke();
                break;
            case Decider.State.DamagedFriendlyStructuresSighted:
                OnDamagedFriendlyStructuresSighted?.Invoke();
                break;
            case Decider.State.FullLoad:
                OnFullLoad?.Invoke();
                break;
            case Decider.State.Harvesting:
                OnHarvetsing?.Invoke();
                break;
            case Decider.State.HostileActorsSighted:
                OnHostileActorsSighted?.Invoke();
                break;
            case Decider.State.HostileStructuresSighted:
                OnHostileStructuresSighted?.Invoke();
                break;
            case Decider.State.Idle:
                OnIdle?.Invoke();
                break;
            case Decider.State.InCombat:
                OnInCombat?.Invoke();
                break;
            case Decider.State.Medic:
                OnMedic?.Invoke();
                break;
            case Decider.State.MovingToGoal:
                OnMovingToGoal?.Invoke();
                break;
            case Decider.State.NeedsRest:
                OnNeedsRest?.Invoke();
                break;
            case Decider.State.ReachedGoal:
                OnReachedGoal?.Invoke();
                break;
            case Decider.State.Resting:
                Rest();
                break;
            case Decider.State.UnderAttack:
                OnUnderAttack?.Invoke();
                break;
            case Decider.State.Watch:
                OnWatch?.Invoke();
                break;
            default:
                OnIdle?.Invoke();
                break;
        }
    }


    public void CallForHelp()
    {
        List<Actor> friends = Decider.IdentifyFriends();

        // TODO: this should be possible as a Select, but there are type problems, and may be a problem if 
        // actors are destroyed in process
        for (int i = 0; i < friends.Count; i++) {
            if (Me == null) break;
            if (friends[i] != null && !friends[i].Actions.Decider.FriendsInNeed.Contains(Me)) {
                if (friends[i].GetComponent<Guard>() != null || Me.GetComponent<Guard>() != null)
                    friends[i].Actions.Decider.FriendsInNeed.Add(Me);
            }
        }
    }


    public void CloseWithEnemies()
    {
        // TODO: we may want to stay at range

        if (transform == null) return;

        if (Me.Actions.Attack.EquippedRangedWeapon != null) {
            Me.Actions.Movement.Agent.speed = Me.Actions.Movement.Speed;
        } else {
            Movement.Agent.speed = 2 * Movement.Speed;
        }

        Actor nearest_enemy = Decider.Threat.Nearest();

        if (nearest_enemy != null) {
            StartCoroutine(Movement.TrackUnit(nearest_enemy));
        }
    }


    public void FleeFromEnemies()
    {
        if (Me == null) return;

        Movement.Agent.speed = 2 * Movement.Speed;
        SheathWeapon();

        Vector3 run_away_from = Vector3.zero;

        var enemies = Me.Actions.Decider.IdentifyEnemies();

        if (enemies.Count > 0) {
            var _enemy = enemies.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).First();
            Vector3 run_away_direction = (transform.position - _enemy.transform.position).normalized;
            Vector3 run_away_to = transform.position + (run_away_direction * Movement.Agent.speed * Movement.Agent.speed);
            Movement.SetDestination(run_away_to);
        }
    }


    public int RollDie(int dice_type, int number_of_rolls)
    {
        int result = 0;

        for (int i = 0; i < number_of_rolls; i++) {
            int roll = UnityEngine.Random.Range(1, dice_type + 1);
            result += roll;
        }
        return result;
    }


    public bool RollSavingThrow(Proficiencies.Attribute attribute, int challenge_rating)
    {
        return UnityEngine.Random.Range(1, 21) + Me.Stats.AttributeProficiency[attribute] > challenge_rating;
    }


    public void SheathWeapon()
    {
        if (Attack.EquippedMeleeWeapon != null) Attack.EquippedMeleeWeapon.gameObject.SetActive(false);
        if (Attack.EquippedRangedWeapon != null) Attack.EquippedRangedWeapon.gameObject.SetActive(false);
    }


    // private


    private void Rest()
    {
        if (Me.RestCounter == Actor.rested_at) {
            Me.Health.RecoverHealth(RollDie(Me.Health.HitDiceType, 1));
            if (Me.Magic != null) Me.Magic.RecoverSpellLevels();
            Me.RestCounter = 0;
        } else {
            Me.RestCounter++;
        }
    }


    private void SetComponents()
    {
        Attack = GetComponentInChildren<Attack>();
        Decider = GetComponent<Decider>();
        Stats = GetComponentInParent<Stats>();
        Me = GetComponentInParent<Actor>();
        Movement = GetComponent<Movement>();
        CanTakeTurn = false; // currently only relevant for player
    }
}
