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

    public static Dictionary<Faction, int> Casualties { get; set; }
    public Conflict.Role NextWave { get; set; }
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
        SetComponents();
    }


    // public


    public void AddCasualty(Faction _faction)
    {
        if (Casualties.ContainsKey(_faction)) {
            Casualties[_faction]++;
        } else {
            Casualties[_faction] = 1;
        }
    }


    public void Hajime()
    {
        GenerateStats();
        CreateNavigationMesh();
        ChooseSides();
        if (World.Instance.battleground) {
            Offense.Instance.Deploy();  // defense has already deployed
        } else {
            StartCoroutine(Waves());
        }
    }


    // private


    private void CreateNavigationMesh()
    {
        GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
    }


    private void ChooseSides()
    {
        if (Random.Range(0,2) < 1) {
            Defense.Instance.Faction = Faction.Ghaddim;
            Offense.Instance.Faction = Faction.Mhoddim;
        } else {
            Defense.Instance.Faction = Faction.Mhoddim;
            Offense.Instance.Faction = Faction.Ghaddim;
        }

        Defense.Instance.Deploy();
        NextWave = Role.Offense;
    }


    private void GenerateStats()
    {
        ConfigureFey.GenerateStats();
        ConfigureGhaddim.GenerateStats();
        ConfigureMhoddim.GenerateStats();
    }


    private void SetComponents()
    {
        Units = new List<GameObject>();
        Casualties = new Dictionary<Faction, int>
        {
            [Faction.Ghaddim] = 0,
            [Faction.Mhoddim] = 0
        };
    }


    private IEnumerator Waves()
    {
        while (true) {
            yield return new WaitForSeconds(60f);

            Light the_sun = World.Instance.Sun();

            switch (NextWave) {
                case Role.Defense:
                    Defense.Instance.Deploy();
                    NextWave = Role.Offense;
                    break;
                case Role.Offense:
                    Offense.Instance.Deploy();
                    NextWave = Role.Defense;
                    break;
            }
        }
    }
}