using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

    public bool enemies_abound;

    Mhoddim mhoddim;
    Ghaddim ghaddim;
    Fey fey;
    Health health;
    Senses senses;
    Movement movement;

    List<GameObject> enemies = new List<GameObject>();
    List<GameObject> friends = new List<GameObject>();


    // Unity


    private void Awake()
    {
        enemies_abound = false;
    }


    private void Start()
    {

    }


    private void OnMouseDown()
    {
        Debug.Log("Quit touching me!");
    }


    // public


    public void FriendAndFoe()
    {
        enemies.Clear();
        friends.Clear();

        foreach (KeyValuePair<GameObject, float> damager in health.GetDamagers()) {
            if (damager.Key != null) enemies.Add(damager.Key);
        }

        foreach (var sighting in senses.GetSightings()) {
            if (sighting == gameObject || IsFriendOrNeutral(sighting)) {
                friends.Add(sighting);  // we are our best friend
            } else if (!enemies.Contains(sighting)) {
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


    public void SetComponents()
    {
        mhoddim = GetComponent<Mhoddim>();
        ghaddim = GetComponent<Ghaddim>();
        fey = GetComponent<Fey>();
        movement = GetComponent<Movement>();
        health = GetComponent<Health>();
        senses = GetComponent<Senses>();
    }


    public void SetStats()
    {
        if (mhoddim != null) {
            mhoddim.SetHealthStats(gameObject);
        }
        else if (ghaddim != null) {
            ghaddim.SetHealthStats(gameObject);
        } else if (fey != null) {
            fey.SetHealthStats(gameObject);
        }
    }


    // private

    private bool IsFriendOrNeutral(GameObject _target)
    {
        // TODO: differentiate between friend and neutral by faction

        if (_target == null) return true;  // null is everyone's friend, or at least not their enemy

        if (mhoddim != null && mhoddim.IsFactionThreat(_target)) return false;
        if (ghaddim != null && ghaddim.IsFactionThreat(_target)) return false;
        if (fey != null && _target.GetComponent<Fey>() == null) return false; // Ents hate mortals; mortals can't see ents until they attack

        Mhoddim target_mhoddim = _target.GetComponent<Mhoddim>();
        Ghaddim target_ghaddim = _target.GetComponent<Ghaddim>();

        bool friend_or_neutral = (mhoddim == null && target_mhoddim == null) || (ghaddim == null && target_ghaddim == null);

        return friend_or_neutral;
    }
}