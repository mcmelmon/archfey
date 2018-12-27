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
    public static Faction Victor { get; set; }
    public static bool Victory { get; set; }
    public static Faction VictoryContender { get; set; }
    public static int VictoryThreshold { get; set; }


    private int victory_ticks;
    private int current_tick;


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
        StartCoroutine(CheckForVictory());
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


    private IEnumerator CheckForVictory()
    {
        while (!Victory && Ruins.Instance != null && VictoryThreshold > 0) {
            if (Ruins.Instance.FactionControl(Faction.Ghaddim) > VictoryThreshold) {
                if (VictoryContender == Faction.Ghaddim) {
                    current_tick++;
                    if (current_tick >= victory_ticks) {
                        Victory = true;
                        Victor = Faction.Ghaddim;
                    }
                } else {
                    current_tick = 0;
                }
            } else if (Ruins.Instance.FactionControl(Faction.Mhoddim) > VictoryThreshold) {
                if (VictoryContender == Faction.Mhoddim)
                {
                    current_tick++;
                    if (current_tick >= victory_ticks)
                    {
                        Victory = true;
                        Victor = Faction.Mhoddim;
                    }
                }
                else
                {
                    current_tick = 0;
                }
            }
            yield return new WaitForSeconds(Turn.action_threshold);
        }
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
        Casualties = new Dictionary<Faction, int>
        {
            [Faction.Ghaddim] = 0,
            [Faction.Mhoddim] = 0
        };
        current_tick = 0;
        Units = new List<GameObject>();
        Victor = Faction.None;
        Victory = false;
        VictoryContender = Faction.None;
        VictoryThreshold = 0;  // set from Ruins after it has completed constructing them
        victory_ticks = 5;
    }


    private IEnumerator Waves()
    {
        while (!Victory) {
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