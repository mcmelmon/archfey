using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offense : MonoBehaviour {

    public Actor actor_prefab;
    public Map map;

    List<Actor> actors;
    float count = 5f;
    float delay = 5f;
    int wave = 0;



    // Unity

    private void Awake()
    {

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
        Vector3 spawn_point = map.GetGeography().RandomBorderLocation();

        Actor _actor = Instantiate(actor_prefab, (spawn_point + new Vector3(0, 3, 0)), actor_prefab.transform.rotation);
        _actor.transform.parent = transform;
        if (_actor != null) actors.Add(_actor);  // TODO: these lists can be tagged children
    }


    private IEnumerator Wave()
    {
        wave++;
        for (int i = 0; i < wave; i++)
        {
            Spawn();
            yield return new WaitForSeconds(2f);
        }
    }


}