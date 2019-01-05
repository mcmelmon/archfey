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
    public int ObjectiveControlRating { get; set; }
    public Action OnAlliesUnderAttack { get; set; }
    public Action OnHostilesSighted { get; set; }
    public Action OnIdle { get; set; }
    public Action OnInCombat { get; set; }
    public Action OnUnderAttack { get; set; }
    public Resources Resources { get; set; }
    public Stealth Stealth { get; set; }

    public static Dictionary<Weapon.DamageType, int> SuperiorWeapons { get; set; }


    // Unity


    void Awake()
    {
        SetComponents();
    }


    // public


    public void ActOnTurn()
    {
        Decider.SetState();

        switch (Decider.state)
        {
            case Decider.State.AlliesUnderAttack:
                OnAlliesUnderAttack.Invoke();
                break;
            case Decider.State.BadlyInjured:
                // defensive spell or flee
                break;
            case Decider.State.FriendliesSighted:
                // if healer, heal
                break;
            case Decider.State.HasObjective:
                // Stay on target
                Movement.Advance();
                break;
            case Decider.State.HostilesSighted:
                OnHostilesSighted.Invoke();
                break;
            case Decider.State.Idle:
                // Try to control a ruin
                OnIdle.Invoke();
                break;
            case Decider.State.InCombat:
                OnInCombat.Invoke();
                break;
            case Decider.State.OnWatch:
                // engage enemies that appear, but return to post quickly
                break;
            case Decider.State.UnderAttack:
                OnUnderAttack.Invoke();
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
        if (Movement == null)
        {
            Attack.AttackEnemiesInRange();
        }
        else
        {
            Actor nearest_enemy = null;
            float shortest_distance = float.MaxValue;
            float distance;

            for (int i = 0; i < Decider.Enemies.Count; i++)
            {
                Actor enemy = Decider.Enemies[i];
                if (enemy == null) continue;

                if (transform == null) continue;
                distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < shortest_distance)
                {
                    shortest_distance = distance;
                    nearest_enemy = enemy;
                }
            }
            Movement.SetRoute(Route.Linear(transform.position, nearest_enemy.transform.position));
        }
    }


    public void FleeFromEnemies()
    {
        Movement.Agent.speed = 2 * Movement.Speed;
        Vector3 run_away_from = Vector3.zero;

        foreach (var enemy in Decider.Enemies) {
            run_away_from += enemy.transform.position;
        }

        Vector3 run_away_direction = (transform.position - run_away_from).normalized;
        Vector3 run_away_to = transform.position + (run_away_direction * Movement.Agent.speed * Movement.Agent.speed);
        Movement.Route = null;
        Movement.ResetPath();
        Movement.SetRoute(Route.Linear(transform.position, run_away_to, Decider.FinishedRoute));
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


    private void SheathWeapon()
    {
        if (Attack.EquippedMeleeWeapon != null) Attack.EquippedMeleeWeapon.gameObject.SetActive(false);
        if (Attack.EquippedRangedWeapon != null) Attack.EquippedRangedWeapon.gameObject.SetActive(false);
    }
}
