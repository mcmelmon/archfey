using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actor : MonoBehaviour
{

    public enum State { Idle = 0, UnderAttack = 1, AlliesUnderAttack = 2, HostilesSighted = 3, OccupyingRuin = 4, HasObjective = 5, OnWatch = 6, InCombat = 7 };

    State state;

    // properties

    public Attack Attack { get; set; }
    public Conflict.Faction Faction { get; set; }
    public List<GameObject> Enemies { get; set; }
    public Fey Fey { get; set; }
    public List<GameObject> Friends { get; set; }
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
                CloseWithEnemies();
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


    public bool IsFriendOrNeutral(GameObject _unit)
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
            GameObject sighting = Senses.Sightings[i];
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
            GameObject nearest_enemy = null;
            float shortest_distance = float.MaxValue;
            float distance;

            for (int i = 0; i < Enemies.Count; i++) {
                GameObject enemy = Enemies[i];
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
            GameObject sighting = Senses.Sightings[i];
            if (sighting == null) continue;
            if (!IsFriendOrNeutral(sighting) && !Enemies.Contains(sighting)) {
                Enemies.Add(sighting);
            }
        }

        return Enemies.Count > 0;
    }


    private bool InCombat()
    {
        return HostilesSighted() && Attack.Engaged();
    }


    private bool IsAThreat(GameObject _unit)
    {
        return Threat.IsAThreat(_unit);
    }


    private bool IsAttackingMyFaction(GameObject _unit)
    {
        return Faction != Conflict.Faction.Fey && Faction != Conflict.Faction.None 
                                  && (Faction == Conflict.Faction.Ghaddim) ? (Ghaddim.IsFactionThreat(_unit)) : (Mhoddim.IsFactionThreat(_unit));
    }


    private bool IsMyFaction(GameObject _unit)
    {
        return _unit != null && Faction == _unit.GetComponent<Actor>().Faction;
    }


    private bool IsMyRole(GameObject _unit)
    {
        return _unit != null && Role == _unit.GetComponent<Actor>().Role;
    }


    private bool OccupyingRuin()
    {
        return RuinControlPoint != null && RuinControlPoint.Occupier == this;
    }


    private bool OnWatch()
    {
        // TODO: implement notion of sentry
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
        Attack = GetComponent<Attack>();
        Enemies = new List<GameObject>();
        Friends = new List<GameObject>();
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
            GameObject _enemy = Threat.BiggestThreat();
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
            GameObject _enemy = Threat.BiggestThreat();
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
            GameObject _enemy = (Faction == Conflict.Faction.Ghaddim) ? Ghaddim.BiggestFactionThreat() : Mhoddim.BiggestFactionThreat();
            
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