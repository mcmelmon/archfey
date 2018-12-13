using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Conflict : MonoBehaviour {

    public int faction_size;
    public Ghaddim ghaddim_prefab;
    public Mhoddim mhoddim_prefab;
    public Fey fey_prefab;

    public float haste_delta = 1f;

    Map map;

    private void Awake()
    {
        map = GetComponentInParent<World>().GetMap();
    }

    private void Start()
    {
        CreateNavigationMesh();
        Hajime();
    }


    // public


    public Map GetMap()
    {
        return map;
    }


    // private

    private void OnValidate()
    {
        if (faction_size > 30) faction_size = 30;
    }


    private void CreateNavigationMesh()
    {
        GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
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