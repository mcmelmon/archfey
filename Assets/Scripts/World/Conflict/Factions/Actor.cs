using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actor : MonoBehaviour {

    public bool enemies_abound;
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
    RuinControlPoint objective;
    Senses senses;
    Stealth stealth;
    Threat threat;


    // Unity


    private void Awake()
    {
        enemies_abound = false;
        SetComponents();
    }


    // public


    public void ActOnTurn()
    {
        // Priorities:
        //
        // Am I under attack?
        StartCoroutine(UnderAttack());


        // Do I see something attacking my allies?
        // Do I see an enemy?
        // Do I have somewhere to be?
        //    1) I am injured and need to find a controlled ruin
        //    2) I am scouting for ruins 
        //    3) I am near an uncontrolled ruin control point

        ResolveSightings();
        ResolveMovement();
        FriendAndFoe();
        ResolveAttacks();
        EstablishRuinControl();
    }


    public void EstablishRuinControl()
    {
        if (fey != null) return;

        if (objective != null) {
            if (objective.OccupiedBy(gameObject)) {
                // we occupy the objective, keep it
                return;
            } else if (!objective.IsOccupied()){
                // the objective is still up for grabs, don't pick another
                return;
            } else  {
                // someone else has occupied our objective, pick another; TODO: fight other faction
                movement.ResetPath();
                objective = null;
            }
        }

        Ruin ruin = GetNearestUnoccupiedRuin();
        if (ruin != null) {
            GameObject control_point = ruin.GetNearestUnoccupiedControlPoint(transform.position);

            if (control_point != null) {
                objective = control_point.GetComponent<RuinControlPoint>();
                movement.SetRoute(Route.Linear(transform.position, control_point.transform.position, ReachedControlPoint));
            }
        }
    }


    public void FriendAndFoe()
    {
        enemies.Clear();
        friends.Clear();

        foreach (var sighting in senses.GetSightings()) {
            if (sighting == gameObject || IsMyFaction(sighting)) {
                friends.Add(sighting);
            } else if (!IsFriendOrNeutral(sighting) && !enemies.Contains(sighting)) {
                enemies.Add(sighting);
            }
        }

        enemies_abound = enemies.Count > 0 ? true : false;
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
        if (IsAttackingMyFaction(_unit)) return false;
        if (_unit.GetComponent<Fey>() != null) return true; // fey are neutral until individual units (e.g. Ents) attack (and get added to damagers)

        return false;  // if none of the above, it's probably the other faction and no exceptions apply
    }


    public void ReachedControlPoint()
    {
        // TODO: attack opposing faction in control
        
        if (objective == null) {
            EstablishRuinControl();
        } else if (!objective.IsOccupied()) {
            objective.Occupy(gameObject);
        } else {
            EstablishRuinControl();
        }
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


    private bool IsAThreat(GameObject _unit)
    {
        return threat.IsAThreat(_unit);
    }


    private bool IsAttackingMyFaction(GameObject _unit)
    {
        return faction != Conflict.Faction.Fey && (faction == Conflict.Faction.Ghaddim) ? (ghaddim.IsFactionThreat(_unit)) : (mhoddim.IsFactionThreat(_unit));
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


    private void ResolveAttacks()
    {
        attack.ManageAttacks();
    }


    private void ResolveMovement()
    {
        if (movement == null) return;

        if (enemies_abound) {
            GameObject enemy = GetAnEnemy();
            if (enemy != null) {
                movement.SetRoute(Route.Linear(transform.position, enemy.transform.position));
            }
        }
    }


    private void ResolveSightings()
    {
        senses.Sight();
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

        SetRuinControlRating(5);  // TODO: pass this in unit by unit
    }



    private IEnumerator UnderAttack()
    {
        while (threat.GetThreats().Count > 0) {
            GameObject _attacker = threat.BiggestThreat();

            if (transform != null && _attacker != null) {   // we or they may have been destroyed...
                if (movement != null) 
                    movement.SetRoute(Route.Linear(transform.position, _attacker.transform.position));
            }

            yield return new WaitForSeconds(Turn.action_threshold);
        }
    }
}