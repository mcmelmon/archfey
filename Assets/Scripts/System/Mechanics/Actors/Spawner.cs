﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum RespawnStrategy { Once, Proximity, Timer };

    // Inspector settings
    [SerializeField] List<SpawnEntry> spawn_prefabs;
    [SerializeField] RespawnStrategy respawn_strategy;
    [SerializeField] float player_proximity_trigger;
    [SerializeField] int respawn_turns;
    [SerializeField] float spawn_circle_radius;
    [SerializeField] Faction faction;
    [SerializeField] Objective objective;

    [Serializable]
    public struct SpawnEntry
    {
        public string spawn_name;
        public GameObject spawn_prefab;
        public int units_to_spawn;
    }

    // properties

    public Faction Allegiance { get; set; }
    public int RespawnTurn { get; set; }
    public Circle SpawnCircle { get; set; }
    public Structure Structure { get; set; }
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

        if (faction != null) {
            foreach (var spawn in spawn_prefabs) {
                GameObject prefab = spawn.spawn_prefab;
                int already_spawned = Spawned.ContainsKey(spawn.spawn_name) ? Spawned[spawn.spawn_name].Count() : 0;
                Vector3 spawn_location = (Structure != null) ? Structure.RandomEntrance().position : SpawnCircle.RandomContainedPoint();

                for (int i = 0; i < spawn.units_to_spawn - already_spawned; i++) {
                    GameObject new_spawn = Instantiate(prefab, spawn_location, prefab.transform.rotation);
                    new_spawn.transform.parent = FindObjectOfType<Characters>().gameObject.transform;

                    Renderer rend = new_spawn.GetComponentInChildren<Renderer>();
                    rend.sharedMaterial.SetColor("_BaseColor", faction.colors);

                    Actor actor = new_spawn.GetComponent<Actor>();
                    actor.CurrentFaction = faction;
                    actor.Alignment = faction.alignment;

                    if (Spawned.ContainsKey(spawn.spawn_name)) {
                        Spawned[spawn.spawn_name].Add(actor);
                    } else {
                        Spawned[spawn.spawn_name] = new List<Actor>() { actor };
                    }

                    if (objective != null) {
                        actor.Actions.Decider.Objectives.Add(objective);
                    }
                }
            }
        }
    }


    public int TotalUnitsAvailable()
    {
        int total_units = spawn_prefabs.Select(prefab => prefab.units_to_spawn).Sum();
        return total_units;
    }


    // private


    private void PruneSpawned()
    {
        foreach (KeyValuePair<string, List<Actor>> pair in Spawned) {
            for (int i = pair.Value.Count - 1; i > -1; i--) {
                if (pair.Value[i] == null) pair.Value.Remove(pair.Value[i]);
            }
        }
    }


    private IEnumerator ProximitySpawn()
    {
        while (respawn_strategy == RespawnStrategy.Proximity) {
            if (Player.Instance != null && Vector3.Distance(transform.position, Player.Instance.transform.position) < player_proximity_trigger) {
                if (RespawnTurn == 0 || RespawnTurn >= respawn_turns) {
                    // The first time the player approaches, spawn right away; but then delay

                    Spawn(); // Spawn ensures that the number outstanding is equal to or less than the number to be spawned
                    RespawnTurn = 0;
                }
                RespawnTurn++; // only start the delay counter if the player has come within range
            }
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }



    private IEnumerator Respawn()
    {
        while (respawn_strategy == RespawnStrategy.Timer) {
            if (RespawnTurn >= respawn_turns ) {
                Spawn(); // Spawn ensures that the number outstanding is equal to or less than the number to be spawned
                RespawnTurn = 0;
            }
            RespawnTurn++;
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private void SetComponents()
    {
        Allegiance = faction;
        SpawnCircle = Circle.New(transform.position, spawn_circle_radius);
        Spawned = new Dictionary<string, List<Actor>>();
        Structure = GetComponentInParent<Structure>();
    }
}