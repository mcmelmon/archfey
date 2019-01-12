using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    // properties

    public int ActionsPerRound { get; set; }
    public Attack Attack { get; set; }
    public Decider Decider { get; set; }
    public Defend Defend { get; set; }
    public Movement Movement { get; set; }
    public int ClaimRating { get; set; }
    public Resources Resources { get; set; }
    public Stealth Stealth { get; set; }

    public Action OnAlliesUnderAttack { get; set; }
    public Action OnBadlyInjured { get; set; }
    public Action OnFriendliesSighted { get; set; }
    public Action OnHostilesSighted { get; set; }
    public Action OnIdle { get; set; }
    public Action OnInCombat { get; set; }
    public Action OnMovingToGoal { get; set; }
    public Action OnPerformingTask { get; set; }
    public Action OnReachedGoal { get; set; }
    public Action OnUnderAttack { get; set; }
    public Action OnWatch { get; set; }

    public static Dictionary<Weapon.DamageType, int> SuperiorWeapons { get; set; }


    // Unity


    void Awake()
    {
        SetComponents();
    }


    // public


    public void ActOnTurn()
    {
        Decider.ChooseState();

        switch (Decider.state)
        {
            case Decider.State.AlliesUnderAttack:
                OnAlliesUnderAttack.Invoke();
                break;
            case Decider.State.BadlyInjured:
                break;
            case Decider.State.FriendliesSighted:
                OnFriendliesSighted.Invoke();
                break;
            case Decider.State.HostilesSighted:
                OnHostilesSighted.Invoke();
                break;
            case Decider.State.Idle:
                OnIdle.Invoke();
                break;
            case Decider.State.InCombat:
                OnInCombat.Invoke();
                break;
            case Decider.State.MovingToGoal:
                OnMovingToGoal.Invoke();
                break;
            case Decider.State.PerformingTask:
                OnPerformingTask.Invoke();
                break;
            case Decider.State.ReachedGoal:
                OnReachedGoal.Invoke();
                break;
            case Decider.State.UnderAttack:
                OnUnderAttack.Invoke();
                break;
            case Decider.State.Watch:
                OnWatch.Invoke();
                break;
            default:
                OnIdle.Invoke();
                break;
        }
    }


    public void CastDefensiveSpell()
    {

    }


    public void CastOffensiveSpell()
    {
        // TODO: allow units to pick from their own particular spells

        if (!Resources.IsCaster || Decider.Enemies.Count == 0) return;

        Smite _smite = Resources.gameObject.GetComponent<Smite>();

        if (_smite != null && Resources.CurrentMana >= _smite.ManaCost)
        {
            float lowest_health = float.MaxValue;
            float health;
            Actor chosen_target = null;

            foreach (var enemy in Decider.Enemies)
            {
                if (Vector3.Distance(enemy.transform.position, transform.position) < _smite.Range)
                {
                    health = enemy.Health.CurrentHitPoints;
                    if (health < lowest_health)
                    {
                        lowest_health = health;
                        chosen_target = enemy;
                    }
                }
            }

            if (chosen_target != null) _smite.Cast(chosen_target);
        }
    }


    public void CloseWithEnemies()
    {
        if (Movement == null) {
            Attack.AttackEnemiesInRange();
        } else {
            Movement.ResetPath();
            Actor nearest_enemy = Decider.Threat.Nearest();

            if (nearest_enemy != null) {
                Movement.SetDestination(nearest_enemy.transform.position);
            } else {
                Decider.state = Decider.previous_state;
            }
        }
    }


    public void FleeFromEnemies()
    {
        SheathWeapon();

        Movement.Agent.speed = 2 * Movement.Speed;
        Vector3 run_away_from = Vector3.zero;

        foreach (var enemy in Decider.Enemies) {
            run_away_from += enemy.transform.position;
        }

        Vector3 run_away_direction = (transform.position - run_away_from).normalized;
        Vector3 run_away_to = transform.position + (run_away_direction * Movement.Agent.speed * Movement.Agent.speed);
        Movement.Route = null;
        Movement.ResetPath();
        Movement.SetDestination(run_away_to);
    }


    public void Maneuver()
    {
        // TODO: allow units to pick from their own particular spells

        if (Decider.Enemies.Count == 0) return;

        FerociousClaw _claw = Resources.gameObject.GetComponent<FerociousClaw>();

        if (_claw != null && Resources.CurrentEnergy >= _claw.EnergyCost)
        {
            float lowest_health = float.MaxValue;
            float health;
            Actor chosen_target = null;

            foreach (var enemy in Decider.Enemies)
            {
                if (Vector3.Distance(enemy.transform.position, transform.position) < _claw.Range)
                {
                    health = enemy.Health.CurrentHitPoints;
                    if (health < lowest_health)
                    {
                        lowest_health = health;
                        chosen_target = enemy;
                    }
                }
            }

            if (chosen_target != null) _claw.Cast(chosen_target);
        }
    }


    public void SheathWeapon()
    {
        if (Attack.EquippedMeleeWeapon != null) Attack.EquippedMeleeWeapon.gameObject.SetActive(false);
        if (Attack.EquippedRangedWeapon != null) Attack.EquippedRangedWeapon.gameObject.SetActive(false);
    }


    // private


    private void SetComponents()
    {
        Resources = GetComponentInChildren<Resources>();
        Attack = GetComponentInChildren<Attack>();
        Decider = GetComponent<Decider>();
        Defend = GetComponentInChildren<Defend>();
        Movement = GetComponent<Movement>();
        SuperiorWeapons = new Dictionary<Weapon.DamageType, int>
        {
            [Weapon.DamageType.Acid] = 0,
            [Weapon.DamageType.Bludgeoning] = 0,
            [Weapon.DamageType.Cold] = 0,
            [Weapon.DamageType.Fire] = 0,
            [Weapon.DamageType.Force] = 0,
            [Weapon.DamageType.Lightning] = 0,
            [Weapon.DamageType.Necrotic] = 0,
            [Weapon.DamageType.Piercing] = 0,
            [Weapon.DamageType.Poison] = 0,
            [Weapon.DamageType.Psychic] = 0,
            [Weapon.DamageType.Slashing] = 0,
            [Weapon.DamageType.Thunder] = 0
        };
    }
}
