using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

    public bool enemies_abound;
    public float ruin_control_radius;
    public int ruin_control_raiting;

    public Fey fey;
    public Ghaddim ghaddim;
    public Mhoddim mhoddim;

    List<GameObject> enemies = new List<GameObject>();
    List<GameObject> friends = new List<GameObject>();
    bool has_objective;

    Health health;
    Senses senses;
    Movement movement;


    // Unity


    private void Awake()
    {
        enemies_abound = false;
        has_objective = false;
    }


    // public


    public void EstablishRuinControl()
    {
        if (fey != null || has_objective) return;  // only the mortals contend for mortal things

        Ruin ruin = GetNearestRuin();
        if (ruin != null) {

            ruin.GetComponent<Renderer>().material.color = Color.blue;

            GameObject control_point = ruin.GetUnoccupiedControlPoint();

            if (control_point != null) {
                control_point.transform.Find("Marker").GetComponent<Renderer>().material.color = Color.red;
                movement.SetRoute(Route.Linear(transform.position, control_point.transform.position, ReachedControlPoint));
                has_objective = true;
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
        return friends.Count > 0 ? friends[Random.Range(0, friends.Count)] : null;
    }


    public GameObject GetAnEnemy()
    {
        return enemies.Count > 0 ? enemies[Random.Range(0, enemies.Count)] : null;
    }


    public bool IsFriendOrNeutral(GameObject _unit)
    {
        if (IsMyFaction(_unit)) return true;
        if (_unit == null || _unit == gameObject) return true;
        if (GetComponent<Scout>() != null) return true; // scouts are everyone's friend unless damaged
        if (GetComponent<Ent>() != null && _unit.GetComponent<Fey>() == null) return false; // Ents always attack
        if ( (ghaddim != null && ghaddim.IsFactionThreat(_unit)) || (mhoddim != null && mhoddim.IsFactionThreat(_unit)) )
            return false;

        foreach (var damager in health.GetDamagers().Keys) {
            if (damager == _unit) return true;
        }

        if (_unit.GetComponent<Fey>() != null) return true; // fey are neutral until individual units attack (and get added to damagers)

        return false;  // if none of the above, it's plenty weird
    }


    public bool IsMyFaction(GameObject _unit)
    {
        Fey other_fey = _unit.GetComponent<Fey>();
        Ghaddim other_ghaddim = _unit.GetComponent<Ghaddim>();
        Mhoddim other_mhoddim = _unit.GetComponent<Mhoddim>();

        return (ghaddim != null && other_ghaddim != null) || (mhoddim != null && other_mhoddim != null) || (fey != null && other_fey != null);
    }


    public void ReachedControlPoint()
    {
        Debug.Log("Reached control point");
    }


    public void SetComponents()
    {
        mhoddim = GetComponent<Mhoddim>();
        ghaddim = GetComponent<Ghaddim>();
        fey = GetComponent<Fey>();
        movement = GetComponent<Movement>();
        health = GetComponent<Health>();
        senses = GetComponent<Senses>();

        SetRuinControlRadius(25f);  // TODO: pass this in unit by unit
        SetRuinControlRating(10);  // TODO: pass this in unit by unit
    }


    public void SetRuinControlRadius(float _radius)
    {
        ruin_control_radius = _radius;
    }


    public void SetRuinControlRating(int _rating)
    {
        ruin_control_raiting = _rating;
    }


    // private


    private Ruin GetNearestRuin()
    {
        Ruin closest_ruin = null;
        float distance;
        float shortest_distance = float.MaxValue;

        foreach(var ruin in GetComponentInParent<World>().GetComponentInChildren<Ruins>().GetRuins()) {
            distance = Vector3.Distance(transform.position, ruin.transform.position);
            if (distance < shortest_distance) {
                closest_ruin = ruin;
                shortest_distance = distance;
            }
        }

        return closest_ruin;
    }
}