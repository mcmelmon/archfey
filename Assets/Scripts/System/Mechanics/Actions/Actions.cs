using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actions : MonoBehaviour
{
    // properties

    public Attack Attack { get; set; }
    public bool CanTakeTurn { get; set; }
    public Decider Decider { get; set; }
    public bool InCombat { get; set; }
    public Actor Me { get; set; }
    public Movement Movement { get; set; }
    public Stats Stats { get; set; }
    public Stealth Stealth { get; set; }

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
        CanTakeTurn |= (Me == Player.Instance.Me);
        if (Stealth.Hiding) Stealth.Hide(); // re-up the Stealth ChallengeRating for the round; TODO: account for obscurity at the new location, etc
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

        Actor nearest_enemy = Decider.Threat.Nearest();

        if (nearest_enemy != null) {
            StartCoroutine(Movement.TrackUnit(nearest_enemy));
        }
    }


    public void FleeFromEnemies()
    {
        if (Me == null) return;

        SheathWeapon();

        Vector3 run_away_from = Vector3.zero;

        var enemies = Me.Actions.Decider.IdentifyEnemies();

        if (enemies.Count > 0) {
            var _enemy = enemies.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).First();
            Vector3 run_away_direction = (transform.position - _enemy.transform.position).normalized;
            Vector3 run_away_to = transform.position + (run_away_direction * Movement.Agent.speed * Movement.Agent.speed);
            Movement.AdjustSpeed(0.5f);
            Movement.SetDestination(run_away_to);
        }
    }


    public int RollDie(int dice_type, int number_of_rolls, bool advantage = false, bool disadvantage = false)
    {
        int die_roll = 0;

        if (number_of_rolls > 1 || (advantage && disadvantage) || (!advantage && !disadvantage)) {
            // advantage only applies in situations with one roll

            for (int i = 0; i < number_of_rolls; i++) {
                int this_roll = UnityEngine.Random.Range(1, dice_type + 1);
                die_roll += this_roll;
            }
        } else if (advantage) {
            die_roll = Mathf.Max(UnityEngine.Random.Range(1, dice_type + 1), UnityEngine.Random.Range(1, dice_type + 1));
        } else if (disadvantage) {
            die_roll = Mathf.Min(UnityEngine.Random.Range(1, dice_type + 1), UnityEngine.Random.Range(1, dice_type + 1));
        }

        return die_roll;
    }


    public bool SavingThrow(Proficiencies.Attribute attribute, int challenge_rating, bool advantage = false, bool disadvantage = false)
    {
        int proficiency_bonus = Me.Stats.SavingThrows.Contains(attribute) ? Me.Stats.ProficiencyBonus : 0;
        int attribute_bonus = Me.Stats.GetAdjustedAttributeScore(attribute);
        int bonus = proficiency_bonus + attribute_bonus;

        int die_roll = RollDie(20, 1, advantage, disadvantage);

        return die_roll + bonus > challenge_rating;
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
        Stealth = GetComponentInParent<Stealth>();
        Me = GetComponentInParent<Actor>();
        Movement = GetComponent<Movement>();
        CanTakeTurn = Me.Health.CurrentHitPoints != 0; // currently only relevant for player
    }
}
