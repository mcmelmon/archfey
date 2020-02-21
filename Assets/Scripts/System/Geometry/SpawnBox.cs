using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnBox : MonoBehaviour
{
    // properties

    public List<Transform> Grid { get; set; }
    public List<Occupation> Occupied { get; set; }

    public struct Occupation {
        public Transform square;
        public GameObject occupier;

        public Occupation(Transform _square, GameObject _occupier) {
            square = _square;
            occupier = _occupier;
        }
    }

    // Unity

    private void Awake() {
        Grid = new List<Transform>();
        foreach (Transform child in transform) {
            Grid.Add(child);
        }
        Occupied = new List<Occupation>();
    }

    // public

    public GameObject Occupy(Transform _square, GameObject _prefab)
    {
        GameObject new_spawn = Instantiate(_prefab, _square.position, _prefab.transform.rotation);
        Occupation occupation = new Occupation(_square, new_spawn);
        Occupied.Add(occupation);
        return new_spawn;
    }

    public GameObject SpawnInNextUnnocupied(GameObject _prefab)
    {
        if (Occupied.Count == Grid.Count) return null;

        GameObject new_spawn = null;
        
        if (Occupied.Any()) {
            new_spawn = Occupy(Grid[Grid.IndexOf(Occupied.Last().square) + 1], _prefab);
        } else {
            new_spawn = Occupy(Grid.First(), _prefab);
        }

        return new_spawn;
    }

    public void Unoccupy(GameObject _occupier)
    {
        Occupied.Remove(Occupied.First(o => o.occupier == _occupier));
    }

    // private

    private IEnumerator PruneOccupied()
    {
        while (true) {
            // check to see if the trigger collider is empty
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }
}
