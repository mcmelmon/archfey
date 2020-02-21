using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBox : MonoBehaviour
{
    // properties

    public bool Occupied { get; set; }

    // Unity
    
    private void Awake() {
        CheckForOccupation();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    // public

    public void CheckForOccupation()
    {
        Occupied = false;
        foreach (var collider in Physics.OverlapBox(transform.position, GetComponent<MeshFilter>().mesh.bounds.extents / 2f)) {
            if (!collider.isTrigger) {
                Occupied = true;
                break;
            }
        }
    }

    public GameObject Spawn(GameObject _prefab)
    {
        if (Occupied) return null;

        GameObject new_spawn = Instantiate(_prefab, transform.position, _prefab.transform.rotation);
        Occupied = true;
        return new_spawn;
    }
}
