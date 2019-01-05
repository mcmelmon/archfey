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
    public Action OnHostilesSighted { get; set; }
    public Action OnIdle { get; set; }
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
                // Freedom!
                CloseWithEnemies();
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
                // Freedom!
                OnHostilesSighted.Invoke();
                break;
            case Decider.State.Idle:
                // Try to control a ruin
                OnIdle.Invoke();
                break;
            case Decider.State.InCombat:
                // Freedom!
                Maneuver();
                CastOffensiveSpell();
                break;
            case Decider.State.OnWatch:
                // engage enemies that appear, but return to post quickly
                break;
            case Decider.State.UnderAttack:
                // Freedom!
                CastOffensiveSpell();
                Maneuver();
                CloseWithEnemies();
                break;
            default:
                OnIdle.Invoke();
                break;
        }
    }


    private void CastDefensiveSpell()
    {

    }


    private void CastOffensiveSpell()
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


    private void CloseWithEnemies()
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


    private void Maneuver()
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
