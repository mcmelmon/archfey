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

    public static Characters Characters { get; set; }
    public static Conflict Instance { get; set; }
    public static Proficiencies Proficiencies { get; set; }


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


    public Faction EnemyFaction(Actor _unit)
    {
        return (_unit.Faction != Faction.None || _unit.Faction != Faction.Fey) ? (_unit.Faction == Faction.Ghaddim) ? Faction.Mhoddim : Faction.Ghaddim : Faction.None;
    }


    public void Hajime()
    {
        PopulateStats();
        ChooseSides();
        StartCoroutine(Waves());
    }


    // private


    private void CreateNavigationMesh()
    {
        GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
    }


    private void ChooseSides()
    {

        Defense.Instance.Faction = Faction.Mhoddim;
        Offense.Instance.Faction = Faction.Ghaddim;
        Defense.Instance.Deploy();
    }


    private void PopulateStats()
    {
        Characters.Instance.GenerateStats();
    }


    private void SetComponents()
    {

        Characters = gameObject.AddComponent<Characters>();
        Proficiencies = gameObject.AddComponent<Proficiencies>();
    }


    private IEnumerator Waves()
    {
        while (true) {
            yield return new WaitForSeconds(60f);

            Defense.Instance.Reinforce();
            Offense.Instance.Reinforce();
        }
    }
}