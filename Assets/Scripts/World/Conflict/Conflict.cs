using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Conflict : MonoBehaviour
{

    public enum Faction { None = 0, Ghaddim = 1, Mhoddim = 2, Fey = 3 };
    public enum Role { None = 0, Defense = 1, Offense = 2 };

    public Ghaddim ghaddim_prefab;
    public Mhoddim mhoddim_prefab;

    // properties

    public static Conflict Instance { get; set; }
    public static List<GameObject> Units { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one conflict instance");
            Destroy(this);
            return;
        }
        Instance = this;
        Units = new List<GameObject>();
    }


    // public


    public void Hajime()
    {
        GenerateStats();
        CreateNavigationMesh();
        FirstWave();
    }


    // private


    private void CreateNavigationMesh()
    {
        GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
    }


    private void FirstWave()
    {
        if (Random.Range(0,2) < 1) {
            Defense.Instance.Faction = Faction.Ghaddim;
            Offense.Instance.Faction = Faction.Mhoddim;
        } else {
            Defense.Instance.Faction = Faction.Mhoddim;
            Offense.Instance.Faction = Faction.Ghaddim;
        }

        Defense.Instance.Deploy();
        Offense.Instance.Deploy();
    }


    private void GenerateStats()
    {
        ConfigureFey.GenerateStats();
        ConfigureGhaddim.GenerateStats();
        ConfigureMhoddim.GenerateStats();
    }
}