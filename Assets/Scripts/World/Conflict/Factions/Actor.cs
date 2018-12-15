using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

    public bool enemies_abound;

    public Mhoddim mhoddim;
    public Ghaddim ghaddim;
    public Fey fey;
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


    // public


    public void FriendAndFoe()
    {
        List<GameObject> sightings = senses.GetSightings();

        enemies.Clear();
        friends.Clear();


        foreach (KeyValuePair<GameObject, float> damager in health.GetDamagers()) {
            if (damager.Key != null) {
                // The enemy has damaged either us or an ally
                // But if they are out of range, forget about them for now
                // TODO: don't completely forget about damagers who are out of range
                // TODO: "link" with allies who are attacking something, even if that something is out of our sight

                if (sightings.Contains(damager.Key)) enemies.Add(damager.Key);
            }
        }

        foreach (var sighting in sightings) {
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