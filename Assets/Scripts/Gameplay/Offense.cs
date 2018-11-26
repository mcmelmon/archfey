using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offense : MonoBehaviour {

    public Actor actor_prefab;

    Map map;
    List<Actor> actors;
    float count = 5f;
    float delay = 5f;
    int wave = 0;



    // Unity

    private void Awake()
    {
        map = GetComponent<Map>();
    }


    private void Start()
    {
        actors = new List<Actor>();
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
        foreach (var combatant in actors)
        {
            if (combatant.destination == null)
            {
                combatant.FindTarget(map.GetInstallations().listing);
            }
        }
    }


    private void Spawn()
    {
        Tile spawn_point = map.GetTerrain().PickRandomEdgeTile();

        if (spawn_point != null && spawn_point.occupier == null)
        {
            Actor _actor = Instantiate(actor_prefab, (spawn_point.transform.position + new Vector3(0, 3, 0)), actor_prefab.transform.rotation);
            if (_actor != null)
            {
                spawn_point.occupier = _actor;
                _actor.transform.parent = spawn_point.transform;
                _actor.FindTarget(map.GetInstallations().listing);
                actors.Add(_actor);
            }
        }
    }


    private IEnumerator Wave()
    {
        wave++;
        for (int i = 0; i < wave; i++)
        {
            if (!map.GetTerrain().AllBordersOccupied())
            {
                Spawn();
                yield return new WaitForSeconds(2f);
            }
        }
    }


}