using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conflict : MonoBehaviour {

    public int faction_size;
    public Ghaddim ghaddim_prefab;
    public Mhoddim mhoddim_prefab;
    public Fey fey_prefab;

    Map map;
    Queue<GameObject> aggressors;
    Queue<GameObject> defenders;

    private void Awake()
    {
        map = GetComponentInParent<World>().GetMap();
        aggressors = new Queue<GameObject>();
        defenders = new Queue<GameObject>();
    }

    private void Start()
    {
        PopulateFactions();
        ChooseSides();
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


    private void ChooseSides()
    {
        if (Random.Range(0,2) == 1) {
            aggressors = PopulateMhoddim();
            defenders = PopulateGhaddim();
        } else {
            defenders = PopulateMhoddim();
            aggressors = PopulateGhaddim();
        }
    }


    private void Hajime()
    {
        GetComponentInChildren<Defense>().Defend(defenders);
        GetComponentInChildren<Offense>().Attack(aggressors);
    }


    private void PopulateFactions()
    {
        PopulateMhoddim();
        PopulateGhaddim();
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