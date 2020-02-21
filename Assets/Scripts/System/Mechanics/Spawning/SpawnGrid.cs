using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnGrid : MonoBehaviour
{
    // properties

    public List<SpawnBox> Grid { get; set; }

    // Unity

    private void Awake() {
        Grid = new List<SpawnBox>(GetComponentsInChildren<SpawnBox>());
    }

    // public

    public GameObject SpawnInNextUnnocupied(GameObject _prefab)
    {
        GameObject new_spawn = null;

        List<SpawnBox> unoccupied = Grid.Where(box => !box.Occupied).ToList();

        if (unoccupied.Any()) {
            new_spawn = unoccupied.First().Spawn(_prefab);
        }

        return new_spawn;
    }
}
