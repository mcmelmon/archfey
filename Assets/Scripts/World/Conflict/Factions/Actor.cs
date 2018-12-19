using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actor : MonoBehaviour {

    public enum State { Idle = 0, UnderAttack = 1, AlliesUnderAttack = 2, HostilesSighted = 3, OccupyingRuin = 4, HasObjective = 5, OnWatch = 6, InCombat = 7};

    public Conflict.Faction faction;
    public Conflict.Role role;
    public int ruin_control_rating;

    public Fey fey;
    public Ghaddim ghaddim;
    public Mhoddim mhoddim;

    List<GameObject> enemies = new List<GameObject>();
    List<GameObject> friends = new List<GameObject>();


    Attack attack;
    Health health;
    Movement movement;
    RuinControlPoint ruin_control_point;
    Senses senses;
    State state;
    Stealth stealth;
    Threat threat;


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void ActOnTurn()
    {
        SetState();

        switch (state) {
            case State.AlliesUnderAttack:
                CloseWithEnemies();
                break;
            case State.HasObjective:
                // evaluate available objectives
                break;
            case State.HostilesSighted:
                CloseWithEnemies();
                break;
            case State.Idle:
                // put away weapons and go back to work
                ResolveRuinControl();
                break;
            case State.InCombat:
                attack.AttackEnemiesInRange();
                break;
            case State.OccupyingRuin:
                // evaluate moving to superior unoccupied control point to cement control
                break;
            case State.OnWatch:
                // engage enemies that appear, but return to post quickly
                break;
            case State.UnderAttack:
                CloseWithEnemies();
                break;
            default:
                break;
        }
    }


    public List<GameObject> GetEnemies()
    {
        return enemies;
    }


    public List<GameObject> GetFriends()
    {
        return friends;
    }


    public GameObject GetAFriend()
    {
        return friends.Count > 0 ? friends[UnityEngine.Random.Range(0, friends.Count)] : null;
    }


    public GameObject GetAnEnemy()
    {
        return enemies.Count > 0 ? enemies[UnityEngine.Random.Range(0, enemies.Count)] : null;
    }


    public bool IsFriendOrNeutral(GameObject _unit)
    {
        if (_unit == null || _unit == gameObject) return true;
        if (IsMyRole(_unit)) return true;  // but don't automatically return false if not my role (I might be a scout, it might be fey)
        if (GetComponent<Scout>() != null) return true; // we are a scout, and scouts do not engage unless damaged
        if (GetComponent<Ent>() != null) return false; // we are an Ent, and ents engage mortals; Fey have same role and will already have returned true
        if (_unit.GetComponent<Fey>() != null) return true; // fey are neutral until individual units (e.g. Ents) attack (and get added to damagers)

        return false;  // if none of the above, it's probably the other faction and no exceptions apply
    }


    public void SetRuinControlRating(int _rating)
    {
        ruin_control_rating = _rating;
    }


    public void SetStealth(Stealth _stealth)
    {
        stealth = _stealth;
    }


    // private


    private bool AlliesUnderAttack()
    {
        foreach (var sighting in senses.GetSightings()) {
            if (IsAttackingMyFaction(sighting)) {
                return true;
            }
        }

        return false;
    }


    private void CloseWithEnemies()
    {
        if (movement == null) {
            attack.AttackEnemiesInRange();
        } else {
            GameObject _enemy = GetAnEnemy();

            if (transform != null && _enemy != null)
            {   // we or they may have been destroyed...
                movement.SetRoute(Route.Linear(transform.position, _enemy.transform.position));
            }
            attack.AttackEnemiesInRange();
        }
    }


    private bool HasObjective()
    {
        if (movement != null) {
            return movement.GetRoute() != null && !movement.GetRoute().completed;
        }

        return false;
    }


    private bool HostilesSighted()
    {
        enemies.Clear();
        foreach (var sighting in senses.GetSightings())
        {
            if (!IsFriendOrNeutral(sighting) && !enemies.Contains(sighting))
            {
                enemies.Add(sighting);
            }
        }

        return enemies.Count > 0;
    }


    private bool InCombat()
    {
        return attack.Engaged();
    }


    private bool IsAThreat(GameObject _unit)
    {
        return threat.IsAThreat(_unit);
    }


    private bool IsAttackingMyFaction(GameObject _unit)
    {
        bool attacker;
        if (faction == Conflict.Faction.Fey || faction == Conflict.Faction.None) {  // the "Quick Fix" is a lie...
            attacker = false;
        } else {
            attacker = (faction == Conflict.Faction.Ghaddim) ? (ghaddim.IsFactionThreat(_unit)) : (mhoddim.IsFactionThreat(_unit));
        }
        return attacker;
    }


    private bool IsMyFaction(GameObject _unit)
    {
        return faction == _unit.GetComponent<Actor>().faction;
    }


    private bool IsMyRole(GameObject _unit)
    {
        return role == _unit.GetComponent<Actor>().role;
    }


    private Ruin GetNearestUnoccupiedRuin()
    {
        Ruin closest_ruin = null;
        float distance;
        float shortest_distance = float.MaxValue;

        foreach(var ruin in GetComponentInParent<World>().GetComponentInChildren<Ruins>().GetRuins()) {
            distance = Vector3.Distance(transform.position, ruin.transform.position);
            if (ruin.GetNearestUnoccupiedControlPoint(transform.position) != null) {
                if (distance < shortest_distance) {
                    closest_ruin = ruin;
                    shortest_distance = distance;
                }
            } else {
                continue;
            }
        }

        return closest_ruin;
    }


    private bool OccupyingRuin()
    {
        return ruin_control_point != null && ruin_control_point.OccupiedBy(gameObject);
    }


    private bool OnWatch()
    {
        // TODO: implement notion of sentry
        return false;
    }


    private void ReachedControlPoint()
    {
        // TODO: attack opposing faction in control

        if (ruin_control_point == null)
        {
            ResolveRuinControl();
        }
        else if (!ruin_control_point.IsOccupied())
        {
            ruin_control_point.Occupy(gameObject);
        }
        else
        {
            ResolveRuinControl();
        }
    }


    private void ResolveRuinControl()
    {
        if (fey != null) return;

        if (ruin_control_point != null)
        {
            if (ruin_control_point.OccupiedBy(gameObject))
            {
                // we occupy the objective, keep it
                return;
            }
            else if (!ruin_control_point.IsOccupied())
            {
                // the objective is still up for grabs, don't pick another
                return;
            }
            else
            {
                // someone else has occupied our objective, pick another; TODO: fight other faction
                movement.ResetPath();
                ruin_control_point = null;
            }
        }

        Ruin ruin = GetNearestUnoccupiedRuin();
        if (ruin != null)
        {
            GameObject control_point = ruin.GetNearestUnoccupiedControlPoint(transform.position);

            if (control_point != null)
            {
                ruin_control_point = control_point.GetComponent<RuinControlPoint>();
                movement.SetRoute(Route.Linear(transform.position, control_point.transform.position, ReachedControlPoint));
            }
        }
    }


    private void SetComponents()
    {
        attack = GetComponent<Attack>();
        mhoddim = GetComponent<Mhoddim>();
        ghaddim = GetComponent<Ghaddim>();
        fey = GetComponent<Fey>();
        movement = GetComponent<Movement>();
        health = GetComponent<Health>();
        senses = GetComponent<Senses>();
        threat = gameObject.AddComponent<Threat>();
        faction = (fey != null) ? Conflict.Faction.Fey : (ghaddim != null) ? Conflict.Faction.Ghaddim : Conflict.Faction.Mhoddim;
        role = Conflict.Role.None;  // offense and defense set this role for mortals
        state = State.Idle;

        SetRuinControlRating(5);  // TODO: pass this in unit by unit
    }


    public void SetState()
    {
        if (InCombat()) {
            state = State.InCombat;
            return;
        }
        else if (UnderAttack())
        {
            GameObject _enemy = threat.BiggestThreat();
            if (_enemy != null) {
                enemies.Add(_enemy);
                state = State.UnderAttack;
            } else {
                state = State.Idle;
            }
            return;
        }
        else if (AlliesUnderAttack())
        {
            GameObject _enemy = (faction == Conflict.Faction.Ghaddim) ? ghaddim.BiggestFactionThreat() : mhoddim.BiggestFactionThreat();
            if (_enemy != null) {
                enemies.Add(_enemy);
                state = State.AlliesUnderAttack;
            } else {
                state = State.Idle;
            }
            return;
        }
        else if (OccupyingRuin())
        {
            state = State.OccupyingRuin;
            return;
        }
        else if (HostilesSighted())
        {
            state = State.HostilesSighted;
            return;
        }
        else if (HasObjective())
        {
            state = State.HasObjective;
            return;
        }
        else if (OnWatch())
        {
            state = State.OnWatch;
            return;
        }
        else {
            state = State.Idle;
            return;
        }
    }


    private bool UnderAttack()
    {
        return threat.GetThreats().Count > 0;
    }
}