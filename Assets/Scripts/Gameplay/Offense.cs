using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Move from Map to Conflict

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


    private void Spawn()
    {
        Tile spawn_point = map.GetTerrain().PickRandomEdgeTile();

        if (spawn_point != null && spawn_point.occupier == null && spawn_point.obstacles.Count == 0)
        {
            Actor _actor = Instantiate(actor_prefab, (spawn_point.transform.position + new Vector3(0, 3, 0)), actor_prefab.transform.rotation);
            _actor.transform.parent = spawn_point.transform;

            if (_actor != null)
            {
                if (_actor.PathAvailable())
                {
                    spawn_point.occupier = _actor;
                    actors.Add(_actor);
                } else {
                    _actor.gameObject.SetActive(false);
                    Destroy(_actor);
                }
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