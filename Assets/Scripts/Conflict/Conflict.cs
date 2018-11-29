﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conflict : MonoBehaviour {

    public int faction_size;
    public Ghaddim ghaddim_prefab;
    public Mhoddim mhoddim_prefab;
    public Fey fey_prefab;

    Map map;

    private void Awake()
    {
        map = GetComponentInParent<World>().GetMap();
    }

    private void Start()
    {
        Hajime();
    }


    // public


    public Vector3 ClearSpawn(Vector3 point, GameObject actor)
    {
        // TODO: The sphere collider appears to be preventing other objects from coming too
        // close, so this isn't necessary... yet.  I am skeptical.

        Vector3 contact;
        Actor _actor = actor.GetComponent<Actor>();

        if (_actor != null) {
            contact = _actor.GetPointOfContact();
            if (contact != Vector3.zero) {
                Debug.Log("In contact");
            }
        }

        return point;
    }


    public Map GetMap()
    {
        return map;
    }


    // private

    private void OnValidate()
    {
        if (faction_size > 30) faction_size = 30;
    }


    private void Hajime()
    {
        if (Random.Range(0, 2) == 1)
        {
            GetComponentInChildren<Defense>().Defend(PopulateMhoddim());
            GetComponentInChildren<Offense>().Attack(PopulateGhaddim());
        }
        else
        {
            GetComponentInChildren<Offense>().Attack(PopulateMhoddim());
            GetComponentInChildren<Defense>().Defend(PopulateGhaddim());
        }
    }


    private Queue<GameObject> PopulateMhoddim()
    {
        Queue<GameObject> mhoddim = new Queue<GameObject>();

        for (int i = 0; i < faction_size; i++)
        {

            Mhoddim _mhoddim = Instantiate(mhoddim_prefab, mhoddim_prefab.transform.position, mhoddim_prefab.transform.rotation, transform);
            _mhoddim.transform.gameObject.SetActive(false);
            mhoddim.Enqueue(_mhoddim.gameObject);
        }

        return mhoddim;
    }


    private Queue<GameObject> PopulateGhaddim()
    {
        Queue<GameObject> ghaddim = new Queue<GameObject>();

        for (int i = 0; i < faction_size; i++)
        {

            Ghaddim _ghaddim = Instantiate(ghaddim_prefab, mhoddim_prefab.transform.position, mhoddim_prefab.transform.rotation, transform);
            _ghaddim.transform.gameObject.SetActive(false);
            ghaddim.Enqueue(_ghaddim.gameObject);
        }

        return ghaddim;
    }
}