using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offense : MonoBehaviour {

    public Map map;
    public Combatant combatant_template;

    List<Combatant> combatants;
    float count = 5f;
    float delay = 5f;
    int wave = 0;


    // Unity


    private void Start()
    {
        combatants = new List<Combatant>();
    }

    void Update()
    {
        if (delay <= 0f)
        {
            StartCoroutine(Wave());
            delay = count;
        }

        delay -= Time.deltaTime;
    }


    // private


    private void AssignTheTroops() 
    {
        foreach (var combatant in combatants)
        {
            if (combatant.destination == null)
            {
                combatant.FindTarget(map.installations.listing);
            }
        }
    }


    private void Spawn()
    {
        Tile spawn_point = map.layout.PickRandomEdgeTile();

        if (spawn_point != null && spawn_point.occupier == null)
        {
            Combatant _combatant = Instantiate(combatant_template, (spawn_point.transform.position + new Vector3(0, 3, 0)), combatant_template.transform.rotation);
            if (_combatant != null)
            {
                spawn_point.occupier = _combatant;
                _combatant.transform.parent = spawn_point.transform;
                _combatant.FindTarget(map.installations.listing);
                combatants.Add(_combatant);
            }
        }
    }


    private IEnumerator Wave()
    {
        wave++;
        for (int i = 0; i < wave; i++)
        {
            if (!map.layout.AllBordersOccupied())
            {
                Spawn();
                yield return new WaitForSeconds(2f);
            }
        }
    }


}