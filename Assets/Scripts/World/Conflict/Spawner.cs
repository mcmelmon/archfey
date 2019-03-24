using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum RespawnStrategy { Once, Proximity, Timer };

    // Inspector settings
    public List<SpawnEntry> spawn_prefabs;
    public RespawnStrategy respawn_strategy;
    public float player_proximity_trigger;
    public int respawn_delay;
    public float spawn_circle_radius;
    public Faction faction;

    [Serializable]
    public struct SpawnEntry
    {
        public string spawn_name;
        public GameObject spawn_prefab;
        public int units_to_spawn;
    }

    // properties

    public int RespawnTick { get; set; }
    public Circle SpawnCircle { get; set; }
    public Dictionary<string, List<Actor>> Spawned { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
        StartCoroutine(Respawn());
        StartCoroutine(ProximitySpawn());
    }


    private void Start()
    {
        if (respawn_strategy == RespawnStrategy.Once || respawn_strategy == RespawnStrategy.Timer) Spawn();
    }


    // public


    public void Spawn()
    {
        PruneSpawned();

        foreach (var spawn in spawn_prefabs) {
            GameObject prefab = spawn.spawn_prefab;
            int already_spawned = Spawned.ContainsKey(spawn.spawn_name) ? Spawned[spawn.spawn_name].Count() : 0;
            for (int i = 0; i < spawn.units_to_spawn - already_spawned; i++) {
                GameObject new_spawn = Instantiate(prefab, SpawnCircle.RandomContainedPoint(), prefab.transform.rotation);
                new_spawn.transform.parent = FindObjectOfType<Characters>().gameObject.transform;
                Actor actor = new_spawn.GetComponent<Actor>();
                actor.Faction = faction;
                actor.Alignment = faction.alignment;
                if (Spawned.ContainsKey(spawn.spawn_name)) {
                    Spawned[spawn.spawn_name].Add(actor);
                } else {
                    Spawned[spawn.spawn_name] = new List<Actor>() { actor };
                }
                faction.Units.Add(actor);
            }
        }
    }


    // private

    private void PruneSpawned()
    {
        foreach (KeyValuePair<string, List<Actor>> pair in Spawned) {
            for (int i = 0; i < pair.Value.Count; i++) {
                if (pair.Value[i] == null) pair.Value.Remove(pair.Value[i]);
            }
        }
    }


    private IEnumerator ProximitySpawn()
    {
        while (respawn_strategy == RespawnStrategy.Proximity) {
            if (Player.Instance != null && Vector3.Distance(transform.position, Player.Instance.transform.position) < player_proximity_trigger) {
                if (RespawnTick == 0 || RespawnTick >= respawn_delay) {
                    // The first time the player approaches, spawn right away; but then delay

                    Spawn(); // Spawn ensures that the number outstanding is equal to or less than the number to be spawned
                    RespawnTick = 0;
                }
                RespawnTick++; // only start the delay counter if the player has come within range
            }
            yield return new WaitForSeconds(1);
        }
    }



    private IEnumerator Respawn()
    {
        while (respawn_strategy == RespawnStrategy.Timer) {
            if (RespawnTick >= respawn_delay ) {
                Spawn(); // Spawn ensures that the number outstanding is equal to or less than the number to be spawned
                RespawnTick = 0;
            }
            RespawnTick++;
            yield return new WaitForSeconds(1);
        }
    }


    private void SetComponents()
    {
        SpawnCircle = Circle.New(transform.position, spawn_circle_radius);
        Spawned = new Dictionary<string, List<Actor>>();
    }
}
