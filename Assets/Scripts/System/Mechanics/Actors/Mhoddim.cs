using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Mhoddim : MonoBehaviour
{
    // properties

    public Actor Actor { get; set; }
    public static float TaxRate { get; set; }
    public static Threat Threat { get; set; }


    // static


    public static float AfterTaxIncome(float transaction)
    {
        float tax = TaxRate * transaction;

        // An outpost will not accumulate tax revenue until it has a civic structure
        var structures = new List<Structure>(FindObjectsOfType<Structure>())
            .Where(s => s.owner == Conflict.Alignment.Good && s.purpose == Structure.Purpose.Civic)
            .ToList();

        foreach (var structure in structures) {
            structure.revenue_cp += tax / structures.Count;
        }

        return transaction - tax;
    }


    public static GameObject SpawnUnit(Vector3 _point)
    {
        Mhoddim _mhoddim = Instantiate(Conflict.Instance.mhoddim_prefab, _point, Conflict.Instance.mhoddim_prefab.transform.rotation);

        if (_mhoddim.GetComponent<NavMeshAgent>() == null) {
            Debug.Log("No agent.");
        }

        return _mhoddim.gameObject;
    }


    // Unity

    private void Awake()
    {
        Actor = GetComponent<Actor>();
        TaxRate = 0.25f;
        Threat = gameObject.AddComponent<Threat>();  // threat for the faction, not for individuals (don't add to game objects)
    }


    // public


    public void AddFactionThreat(Actor _foe, float _threat)
    {
        Threat.AddThreat(_foe, _threat);
    }


    public Actor BiggestFactionThreat()
    {
        return Threat.PrimaryThreat();
    }


    public bool IsFactionThreat(Actor _sighting)
    {
        return _sighting != null && Threat.IsAThreat(_sighting);
    }
}