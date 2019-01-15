using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mhoddim : MonoBehaviour
{
    // properties

    public Actor Actor { get; set; }
    public static float TaxRate { get; set; }
    public static float Treasury { get; set; }
    public static Threat Threat { get; set; }


    // static


    public static float AfterTaxIncome(float transaction)
    {
        float tax = TaxRate * transaction;
        Treasury += tax;

        return transaction - tax;
    }


    public static GameObject SpawnUnit(Vector3 _point)
    {
        Mhoddim _mhoddim = Instantiate(Conflict.Instance.mhoddim_prefab, _point, Conflict.Instance.mhoddim_prefab.transform.rotation);  // drop from on high to avoid being in buildings etc.
        _mhoddim.gameObject.AddComponent<Characters>();

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
        Treasury = 0f;
    }


    // public


    public void AddFactionThreat(Actor _foe, float _threat)
    {
        Threat.AddThreat(_foe, _threat);
    }


    public Actor BiggestFactionThreat()
    {
        return Threat.BiggestThreat();
    }


    public bool IsFactionThreat(Actor _sighting)
    {
        return _sighting != null && Threat.IsAThreat(_sighting);
    }
}