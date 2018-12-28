﻿using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actor : MonoBehaviour
{

    public enum State { Idle = 0, UnderAttack = 1, AlliesUnderAttack = 2, HostilesSighted = 3, OccupyingRuin = 4, HasObjective = 5, OnWatch = 6, InCombat = 7, FriendliesSighted = 8 };

    State state;

    // properties

    public Abilities Abilities { get; set; }
    public Attack Attack { get; set; }
    public Defend Defend { get; set; }
    public Conflict.Faction Faction { get; set; }
    public List<Actor> Enemies { get; set; }
    public Fey Fey { get; set; }
    public List<Actor> Friends { get; set; }
    public Ghaddim Ghaddim { get; set; }
    public Health Health { get; set; }
    public Mhoddim Mhoddim { get; set; }
    public Movement Movement { get; set; }
    public Conflict.Role Role { get; set; }
    public RuinControlPoint RuinControlPoint { get; set; }
    public int RuinControlRating { get; set; }
    public Senses Senses { get; set; }
    public Stealth Stealth { get; set; }
    public Threat Threat { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void ActOnTurn()
    {
        Senses.Sight();
        SetState();

        switch (state) {
            case State.AlliesUnderAttack:
                // Freedom!
                CloseWithEnemies();
                break;
            case State.FriendliesSighted:
                // if healer, heal
                break;
            case State.HasObjective:
                // Stay on target
                Movement.Advance();
                break;
            case State.HostilesSighted:
                // Freedom!
                CloseWithEnemies();
                break;
            case State.Idle:
                // Try to control a ruin
                ControlRuin();
                break;
            case State.InCombat:
                // Freedom!
                break;
            case State.OccupyingRuin:
                // Our Precious
                ConfirmOccupation();
                break;
            case State.OnWatch:
                // engage enemies that appear, but return to post quickly
                break;
            case State.UnderAttack:
                // Freedom!
                CloseWithEnemies();
                break;
            default:
                ControlRuin();
                break;
        }
    }


    public List<Actor> IdentifyFriends()
    {
        FriendliesSighted();
        return Friends;
    }


    public bool IsFriendOrNeutral(Actor _unit)
    {
        if (_unit == null || _unit == gameObject) return true;
        if (IsMyRole(_unit)) return true;  // but don't automatically return false if not my role (I might be a scout, it might be fey)
        if (GetComponent<Scout>() != null) return true; // we are a scout, and scouts do not engage unless damaged
        if (GetComponent<Ent>() != null) return false; // we are an Ent, and ents engage mortals; Fey have same role and will already have returned true
        if (_unit.GetComponent<Fey>() != null) return true; // fey are neutral until individual units (e.g. Ents) attack (and get added to damagers)

        return false;  // if none of the above, it's probably the other faction and no exceptions apply
    }


    // private


    private bool AlliesUnderAttack()
    {
        for (int i = 0; i < Senses.Sightings.Count; i++) {
            Actor sighting = Senses.Sightings[i];
            if (sighting == null) continue;
            if (IsAttackingMyFaction(sighting)) return true;
        }

        return false;
    }


    private void CloseWithEnemies()
    {
        if (Movement == null) {
            Attack.AttackEnemiesInRange();
        } else {
            Actor nearest_enemy = null;
            float shortest_distance = float.MaxValue;
            float distance;

            for (int i = 0; i < Enemies.Count; i++) {
                Actor enemy = Enemies[i];
                if (enemy == null) continue;

                if (transform == null) continue;
                distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < shortest_distance) {
                    shortest_distance = distance;
                    nearest_enemy = enemy;
                }
            }
            Movement.SetRoute(Route.Linear(transform.position, nearest_enemy.transform.position));
        }
    }


    private void ConfirmOccupation()
    {
        // if we've tricked ourselves into believing we are the occupier, pick a new control point

        if (RuinControlPoint.ConfirmOccupation() != this) {
            // find another ruin
            RuinControlPoint = null;
            ControlRuin();
        } else if (Vector3.Distance(transform.position, RuinControlPoint.transform.position) > Route.reached_threshold) {
            // combat has pulled us away from our control point.  Choose the nearest unoccupied point.
            // the ruin handles abandonment by a previous occupier.
            RuinControlPoint = null;
            state = State.Idle;
        }
    }


    private void ControlRuin()
    {
        if (Fey != null || transform == null) return;

        // TODO: if we were are the Occupier of our RuinControlPoint, be sure to return to it after combat

        RuinControlPoint ruin_control_point = Ruins.Instance.GetNearestUnoccupiedControlPoint(gameObject);
        if (ruin_control_point != null && ruin_control_point != RuinControlPoint) {
            RuinControlPoint = ruin_control_point;
            Movement.SetRoute(Route.Linear(transform.position, ruin_control_point.transform.position, ReachedControlPoint));
        }
    }


    private bool FriendliesSighted()
    {
        Friends.Clear();

        for (int i = 0; i < Senses.Sightings.Count; i++)
        {
            Actor _friend = Senses.Sightings[i];
            if (_friend == null) continue;
            if (_friend != null && IsMyFaction(_friend) && !Friends.Contains(_friend))
            {
                Friends.Add(_friend);
            }
        }

        return Friends.Count > 0;
    }


    private bool HasObjective()
    {
        if (RuinControlPoint != null && RuinControlPoint.Occupier != null && RuinControlPoint.Occupier != this) {
            RuinControlPoint = null;
            ControlRuin();
            return true;
        }

        return Movement != null && Movement.Route != null && !Movement.Route.Completed();
    }


    private bool HostilesSighted()
    {
        Enemies.Clear();

        for (int i = 0; i < Senses.Sightings.Count; i++) {
            Actor _hostile = Senses.Sightings[i];
            if (_hostile == null) continue;
            if (!IsFriendOrNeutral(_hostile) && !Enemies.Contains(_hostile)) {
                Enemies.Add(_hostile);
            }
        }

        return Enemies.Count > 0;
    }


    private bool InCombat()
    {
        return HostilesSighted() && Attack.Engaged();
    }


    private bool IsAThreat(Actor _unit)
    {
        return Threat.IsAThreat(_unit);
    }


    private bool IsAttackingMyFaction(Actor _unit)
    {
        return Faction != Conflict.Faction.Fey && Faction != Conflict.Faction.None 
                                  && (Faction == Conflict.Faction.Ghaddim) ? (Ghaddim.IsFactionThreat(_unit)) : (Mhoddim.IsFactionThreat(_unit));
    }


    private bool IsMyFaction(Actor _unit)
    {
        return _unit != null && Faction == _unit.Faction;
    }


    private bool IsMyRole(Actor _unit)
    {
        return _unit != null && Role == _unit.Role;
    }


    private bool OccupyingRuin()
    {
        return RuinControlPoint != null && RuinControlPoint.Occupier == this;
    }


    private bool OnWatch()
    {
        // TODO: once a ruin is captured, switch to sentry and attack incoming enemies
        return false;
    }


    private void ReachedControlPoint()
    {
        // This will be called by Movement.Advance on each turn that we occupy the control point
        // We want to wait until the control point is controlled, and then find another near unoccupied one
        // That will reset Movement

        if (RuinControlPoint != null && RuinControlPoint.Occupier != this) {
            RuinControlPoint = null;
            ControlRuin();
        }
    }


    private void SetComponents()
    {
        Abilities = GetComponentInChildren<Abilities>();
        Attack = GetComponentInChildren<Attack>();
        Defend = GetComponentInChildren<Defend>();
        Enemies = new List<Actor>();
        Friends = new List<Actor>();
        Mhoddim = GetComponent<Mhoddim>();
        Ghaddim = GetComponent<Ghaddim>();
        Fey = GetComponent<Fey>();
        Movement = GetComponent<Movement>();
        Health = GetComponent<Health>();
        Senses = GetComponent<Senses>();
        Threat = gameObject.AddComponent<Threat>();
        Faction = (Fey != null) ? Conflict.Faction.Fey : (Ghaddim != null) ? Conflict.Faction.Ghaddim : Conflict.Faction.Mhoddim;
        Role = Conflict.Role.None;  // offense and defense set this role for mortals
        state = State.Idle;
    }


    public void SetState()
    {
        if (InCombat()) {
            Actor _enemy = Threat.BiggestThreat();
            if (_enemy != null) {
                if (!Enemies.Contains(_enemy)) Enemies.Add(_enemy);
                state = State.InCombat;
            } else {
                state = State.Idle;
            }
            return;
        }
        else if (UnderAttack())
        {
            Actor _enemy = Threat.BiggestThreat();
            if (_enemy != null) {
                if (!Enemies.Contains(_enemy)) Enemies.Add(_enemy);
                state = State.UnderAttack;
            } else {
                state = State.Idle;
            }
            return;
        }
        else if (OccupyingRuin())
        {
            SheathWeapon();
            state = State.OccupyingRuin;
            return;
        }
        else if (AlliesUnderAttack())
        {
            Actor _enemy = (Faction == Conflict.Faction.Ghaddim) ? Ghaddim.BiggestFactionThreat() : Mhoddim.BiggestFactionThreat();
            
            if (_enemy != null) {
                if (!Enemies.Contains(_enemy)) Enemies.Add(_enemy);
                state = State.AlliesUnderAttack;
            }
            else {
                state = State.Idle;
            }
            return;
        }
        else if (HostilesSighted())
        {
            state = State.HostilesSighted;
            return;
        }
        else if (HasObjective())
        {
            SheathWeapon();
            state = State.HasObjective;
            return;
        }
        else if (OnWatch())
        {
            SheathWeapon();
            state = State.OnWatch;
            return;
        }
        else {
            SheathWeapon();
            RuinControlPoint = null;
            state = State.Idle;
            return;
        }
    }


    private void SheathWeapon()
    {
        Weapon equipped_weapon = Attack.GetWeapon();
        if (equipped_weapon != null) {
            Destroy(equipped_weapon.gameObject);
        }
    }


    private bool UnderAttack()
    {
        return Threat.GetThreats().Count > 0;
    }
}