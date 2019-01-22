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
    public Action OnMovingToGoal { get; set; }
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
            case Decider.State.BadlyInjured:
                OnBadlyInjured.Invoke();
                break;
            case Decider.State.Crafting:
                OnCrafting.Invoke();
                break;
            case Decider.State.FriendsInNeed:
                OnFriendsInNeed.Invoke();
                break;
            case Decider.State.FriendlyActorsSighted:
                OnFriendlyActorsSighted.Invoke();
                break;
            case Decider.State.DamagedFriendlyStructuresSighted:
                OnDamagedFriendlyStructuresSighted.Invoke();
                break;
            case Decider.State.FullLoad:
                OnFullLoad.Invoke();
                break;
            case Decider.State.Harvesting:
                OnHarvetsing.Invoke();
                break;
            case Decider.State.HostileActorsSighted:
                OnHostileActorsSighted.Invoke();
                break;
            case Decider.State.HostileStructuresSighted:
                OnHostileStructuresSighted.Invoke();
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


    public void CallForHelp()
    {
        List<Actor> friends = Decider.IdentifyFriends();

        // TODO: this should be possible as a Select, but there are type problems, and may be a problem if 
        // actors are destroyed in process
        for (int i = 0; i < friends.Count; i++) {
            if (Me == null) break;
            if (friends[i] != null && !friends[i].Actions.Decider.FriendsInNeed.Contains(Me)) {
                if (friends[i].GetComponent<Guard>() != null || Me.GetComponent<Commoner>() == null) // commoners rally around guards, but not other commoners
                    friends[i].Actions.Decider.FriendsInNeed.Add(Me);
            }
        }
    }


    public void CastDefensiveSpell()
    {

    }


    public void CastOffensiveSpell()
    {
        //// TODO: allow units to pick from their own particular spells

        //if (Decider.Enemies.Count == 0) return;

        //Smite _smite = Resources.gameObject.GetComponent<Smite>();

        //if (_smite != null)
        //{
        //    float lowest_health = float.MaxValue;
        //    float health;
        //    Actor chosen_target = null;

        //    foreach (var enemy in Decider.Enemies)
        //    {
        //        if (Vector3.Distance(enemy.transform.position, transform.position) < _smite.Range)
        //        {
        //            health = enemy.Health.CurrentHitPoints;
        //            if (health < lowest_health)
        //            {
        //                lowest_health = health;
        //                chosen_target = enemy;
        //            }
        //        }
        //    }

        //    if (chosen_target != null) _smite.Cast(chosen_target);
        //}
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
            StartCoroutine(TrackEnemy(nearest_enemy));
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


    public void Maneuver()
    {
    //    // TODO: allow units to pick from their own particular spells

    //    if (Decider.Enemies.Count == 0) return;

    //    FerociousClaw _claw = Resources.gameObject.GetComponent<FerociousClaw>();

    //    if (_claw != null && Resources.CurrentEnergy >= _claw.EnergyCost)
    //    {
    //        float lowest_health = float.MaxValue;
    //        float health;
    //        Actor chosen_target = null;

    //        foreach (var enemy in Decider.Enemies)
    //        {
    //            if (Vector3.Distance(enemy.transform.position, transform.position) < _claw.Range)
    //            {
    //                health = enemy.Health.CurrentHitPoints;
    //                if (health < lowest_health)
    //                {
    //                    lowest_health = health;
    //                    chosen_target = enemy;
    //                }
    //            }
    //        }

    //        if (chosen_target != null) _claw.Cast(chosen_target);
    //    }
    }


    public void SheathWeapon()
    {
        if (Attack.EquippedMeleeWeapon != null) Attack.EquippedMeleeWeapon.gameObject.SetActive(false);
        if (Attack.EquippedRangedWeapon != null) Attack.EquippedRangedWeapon.gameObject.SetActive(false);
    }


    // private


    private void SetComponents()
    {
        Attack = GetComponentInChildren<Attack>();
        Decider = GetComponent<Decider>();
        Stats = GetComponentInParent<Stats>();
        Me = GetComponentInParent<Actor>();
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


    private IEnumerator TrackEnemy(Actor _enemy)
    {
        float separation = Vector3.Distance(transform.position, _enemy.transform.position);
        int count = 0;

        while (_enemy != null && count < 6 && separation > Movement.ReachedThreshold) {
            Movement.SetDestination(_enemy.transform);
            count++;
            yield return new WaitForSeconds(1);
        }
    }
}
