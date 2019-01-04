using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Conflict : MonoBehaviour
{

    public enum Faction { None = 0, Ghaddim = 1, Mhoddim = 2, Fey = 3 };
    public enum Role { None = 0, Defense = 1, Offense = 2 };

    // Inspector settings

    public Ghaddim ghaddim_prefab;
    public Mhoddim mhoddim_prefab;

    // properties

    public static Dictionary<Faction, int> Casualties { get; set; }
    public Conflict.Role NextWave { get; set; }
    public static Conflict Instance { get; set; }
    public static int ToHitBase { get; set; }
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
        //StartCoroutine(CheckForVictory());
    }


    // public


    public void AddCasualty(Faction _faction)
    {
        Casualties[_faction]++;
    }


    public Faction EnemyFaction(Actor _unit)
    {
        return (_unit.Faction != Faction.None || _unit.Faction != Faction.Fey) ? (_unit.Faction == Faction.Ghaddim) ? Faction.Mhoddim : Faction.Ghaddim : Faction.None;
    }


    public void Hajime()
    {
        GenerateStats();
        //CreateNavigationMesh();
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
        while (true) {
            if (!Victory && Objectives.Instance != null && VictoryThreshold > 0)  // don't put test in while or enumerator never starts up
            {
                if (Objectives.HeldByFaction[Faction.Ghaddim].Count >= VictoryThreshold)
                {
                    if (VictoryContender == Faction.Ghaddim)
                    {
                        current_tick++;
                        if (current_tick >= victory_ticks)
                        {
                            Victory = true;
                            Victor = Faction.Ghaddim;
                        }
                    }
                    else
                    {
                        current_tick = 0;
                        VictoryContender = Faction.Ghaddim;
                    }
                }
                else if (Objectives.HeldByFaction[Faction.Mhoddim].Count >= VictoryThreshold)
                {
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
                        VictoryContender = Faction.Mhoddim;
                    }
                }
            }
            yield return new WaitForSeconds(Turn.action_threshold);
        }
    }


    private void ChooseSides()
    {

        Defense.Instance.Faction = Faction.Mhoddim;
        Offense.Instance.Faction = Faction.Ghaddim;
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
        ToHitBase = 10;
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
                    Defense.Instance.Reinforce();
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