using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Conflict : MonoBehaviour
{

    public enum Alignment { Unaligned = 0, Good = 1, Evil = 2, Neutral = 3 };

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


    public Alignment EnemyFaction(Actor _unit)
    {
        return (_unit.Alignment != Alignment.Unaligned || _unit.Alignment != Alignment.Neutral) ? ((_unit.Alignment == Alignment.Evil) ? Alignment.Good : Alignment.Evil) : Alignment.Unaligned;
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
        Faction good_faction = FindObjectsOfType<Faction>().First(faction => faction.alignment == Alignment.Good);
        good_faction.Reinforce();
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

            Faction good_faction = FindObjectsOfType<Faction>().First(faction => faction.alignment == Alignment.Good);
            good_faction.Reinforce();

            Faction evil_faction = FindObjectsOfType<Faction>().First(faction => faction.alignment == Alignment.Evil);
            evil_faction.Reinforce();
        }
    }
}