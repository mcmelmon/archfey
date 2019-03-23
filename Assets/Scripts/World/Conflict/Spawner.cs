using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum RespawnStrategy { Once, PlayerNears, Timer };

    // Inspector settings
    public List<SpawnEntry> spawn_prefabs;
    public Conflict.Alignment alignment;
    public RespawnStrategy respawn_strategy;
    public float player_proximity_trigger;
    public int respawn_delay;

    [Serializable]
    public struct SpawnEntry
    {
        public string spawn_name;
        public GameObject spawn_prefab;
        public int units_to_spawn;
    }

    // properties

    public Faction Faction { get; set; }
    public int RespawnTick { get; set; }
    public Dictionary<string, List<Actor>> Spawned { get; set; }


    // Unity


    private void Awake()
    {
        Faction = GetComponent<Faction>();
        Spawned = new Dictionary<string, List<Actor>>();
        StartCoroutine(Respawn());
    }


    private void Start()
    {
        if (respawn_strategy == RespawnStrategy.Once || respawn_strategy == RespawnStrategy.Timer) Spawn();
    }


    // public


    public void Spawn()
    {
        foreach (var spawn in spawn_prefabs) {
            GameObject prefab = spawn.spawn_prefab;
            int already_spawned = Spawned.ContainsKey(spawn.spawn_name) ? Spawned[spawn.spawn_name].Count() : 0;
            for (int i = 0; i < spawn.units_to_spawn - already_spawned; i++) {
                GameObject new_spawn = Instantiate(prefab, transform.position, prefab.transform.rotation);
                new_spawn.transform.parent = FindObjectOfType<Characters>().gameObject.transform;
                Actor actor = new_spawn.GetComponent<Actor>();
                actor.Alignment = alignment;
                actor.Faction = Faction;
                if (Spawned.ContainsKey(spawn.spawn_name)) {
                    Spawned[spawn.spawn_name].Add(actor);
                } else {
                    Spawned[spawn.spawn_name] = new List<Actor>() { actor };
                }

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


    private IEnumerator Respawn()
    {
        while (respawn_strategy == RespawnStrategy.Timer) {
            PruneSpawned();
            if (RespawnTick >= respawn_delay ) {
                Spawn(); // Spawn ensures that the number outstanding is equal to or less than the number to be spawned
                RespawnTick = 0;
            }
            RespawnTick++;
            yield return new WaitForSeconds(1);
        }
    }

}
