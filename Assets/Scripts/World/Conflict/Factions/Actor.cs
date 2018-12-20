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
        SetState();

        switch (state) {
            case State.AlliesUnderAttack:
                CloseWithEnemies();
                break;
            case State.HasObjective:
                Movement.Advance();
                break;
            case State.HostilesSighted:
                CloseWithEnemies();
                break;
            case State.Idle:
                // put away weapons and go back to work
                ResolveRuinControl();
                break;
            case State.InCombat:
                Attack.AttackEnemiesInRange();
                break;
            case State.OccupyingRuin:
                // sit tight for now
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


    public GameObject GetAFriend()
    {
        return Friends.Count > 0 ? Friends[UnityEngine.Random.Range(0, Friends.Count)] : null;
    }


    public GameObject GetAnEnemy()
    {
        return Enemies.Count > 0 ? Enemies[UnityEngine.Random.Range(0, Enemies.Count)] : null;
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
        foreach (var sighting in Senses.GetSightings()) {
            if (IsAttackingMyFaction(sighting)) {
                return true;
            }
        }

        return false;
    }


    private void CloseWithEnemies()
    {
        if (Movement == null) {
            Attack.AttackEnemiesInRange();
        } else {
            GameObject _enemy = GetAnEnemy();

            if (transform != null && _enemy != null)
            {   // we or they may have been destroyed...
                Movement.SetRoute(Route.Linear(transform.position, _enemy.transform.position));
            }
            Attack.AttackEnemiesInRange();
        }
    }


    private bool HasObjective()
    {
        if (Movement != null) {
            return Movement.Route != null && !Movement.Route.Completed;
        }

        return false;
    }


    private bool HostilesSighted()
    {
        Enemies.Clear();
        foreach (var sighting in Senses.GetSightings())
        {
            if (!IsFriendOrNeutral(sighting) && !Enemies.Contains(sighting))
            {
                Enemies.Add(sighting);
            }
        }

        return Enemies.Count > 0;
    }


    private bool InCombat()
    {
        return Attack.Engaged();
    }


    private bool IsAThreat(GameObject _unit)
    {
        return Threat.IsAThreat(_unit);
    }


    private bool IsAttackingMyFaction(GameObject _unit)
    {
        bool attacker;
        if (Faction == Conflict.Faction.Fey || Faction == Conflict.Faction.None) {  // the "Quick Fix" is a lie...
            attacker = false;
        } else {
            attacker = (Faction == Conflict.Faction.Ghaddim) ? (Ghaddim.IsFactionThreat(_unit)) : (Mhoddim.IsFactionThreat(_unit));
        }
        return attacker;
    }


    private bool IsMyFaction(GameObject _unit)
    {
        return Faction == _unit.GetComponent<Actor>().Faction;
    }


    private bool IsMyRole(GameObject _unit)
    {
        return Role == _unit.GetComponent<Actor>().Role;
    }


    private bool OccupyingRuin()
    {
        return RuinControlPoint != null && RuinControlPoint.Occupied == gameObject;
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

        if (RuinControlPoint.Occupier != gameObject) {
            RuinControlPoint = null;
            ResolveRuinControl();
        } else {
            Movement.Route.Completed = true;
            Movement.ResetPath();
        }
    }


    private void ResolveRuinControl()
    {
        if (Fey != null) return;

        if (RuinControlPoint != null) {
            if (RuinControlPoint.Occupied == gameObject) {
                // we occupy the objective, keep it
                return;
            } else if (!RuinControlPoint.Occupied) {
                // the objective is still up for grabs, don't pick another
                return;
            }
        }

        RuinControlPoint ruin_control_point = Ruins.Instance.GetNearestUnoccupiedControlPoint(gameObject);
        if (ruin_control_point != null) {
                RuinControlPoint = ruin_control_point;
                Movement.SetRoute(Route.Linear(transform.position, ruin_control_point.transform.position, ReachedControlPoint));
        }
    }


    private void SetComponents()
    {
        Attack = GetComponent<Attack>();
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

        Enemies = new List<GameObject>();
        Friends = new List<GameObject>();
        RuinControlRating = 5;  // TODO: pass this in unit by unit
    }


    public void SetState()
    {
        if (InCombat()) {
            GameObject _enemy = Threat.BiggestThreat();
            if (_enemy != null) {
                if (!Enemies.Contains(_enemy)) Enemies.Add(_enemy);
                state = State.InCombat;
            } else {
                SheathWeapon();
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
                SheathWeapon();
                state = State.Idle;
            }
            return;
        }
        else if (AlliesUnderAttack())
        {
            GameObject _enemy = (Faction == Conflict.Faction.Ghaddim) ? Ghaddim.BiggestFactionThreat() : Mhoddim.BiggestFactionThreat();
            if (_enemy != null) {
                if (!Enemies.Contains(_enemy)) Enemies.Add(_enemy);
                state = State.AlliesUnderAttack;
            } else {
                state = State.Idle;
            }
            return;
        }
        else if (OccupyingRuin())
        {
            SheathWeapon();
            if (Movement != null) {
                Movement.Advance(); // be sure to finish out our Route to trigger ReachedControlPoint
            }
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