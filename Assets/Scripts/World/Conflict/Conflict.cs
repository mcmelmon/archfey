using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Conflict : MonoBehaviour {

    public static Conflict conflict_instance;

    public GameObject fey;
    public GameObject ghaddim;
    public GameObject mhoddim;

    // Unity


    private void Awake()
    {
        if (conflict_instance != null)
        {
            Debug.LogError("More than one conflict instance");
            Destroy(this);
            return;
        }
        conflict_instance = this;

        ConfigureFey.Populate();
        ConfigureGhaddim.Populate();
        ConfigureMhoddim.Populate();
    }


    // public


    public void Hajime()
    {
        AssignFactionRoles();
        CreateNavigationMesh();
        FlamesOfWar();
    }


    // private


    private void AssignFactionRoles()
    {
        if (Random.Range(0,2) < 1) {
            ghaddim.AddComponent<Offense>();
            mhoddim.AddComponent<Defense>();
        } else {
            ghaddim.AddComponent<Defense>();
            mhoddim.AddComponent<Offense>();
        }
    }


    private void CreateNavigationMesh()
    {
        GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
    }


    private void FlamesOfWar()
    {
        GetComponentInChildren<Defense>().Setup();
        GetComponentInChildren<Offense>().Setup();
    }
}