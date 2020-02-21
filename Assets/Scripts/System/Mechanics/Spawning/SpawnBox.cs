using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBox : MonoBehaviour
{
    // properties

    public bool Occupied { get; set; }

    // Unity
    
    private void Awake() {
        Occupied = false;
        foreach (var collider in Physics.OverlapBox(transform.position, GetComponent<MeshFilter>().mesh.bounds.extents / 2f)) {
            if (collider.gameObject.GetComponent<SpawnBox>() == null) {
                Occupied = true;
                break;
            }
        }
    }

    private void Update() {
        Occupied = false;
        foreach (var collider in Physics.OverlapBox(transform.position, GetComponent<MeshFilter>().mesh.bounds.extents / 2f)) {
            if (collider.gameObject.GetComponent<SpawnBox>() == null) {
                Occupied = true;
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    // public

    public GameObject Spawn(GameObject _prefab)
    {
        GameObject new_spawn = Instantiate(_prefab, transform.position, _prefab.transform.rotation);
        Occupied = true;
        return new_spawn;
    }
}
