using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Conflict : MonoBehaviour {


    // Unity


    private void Start()
    {

        AssignFactionRoles();
        CreateNavigationMesh();
    }


    // private


    private void AssignFactionRoles()
    {
        if (Random.Range(0, 2) < 1) {
            transform.Find("Ghaddim").gameObject.AddComponent<Offense>();
            transform.Find("Mhoddim").gameObject.AddComponent<Defense>();
        } else {
            transform.Find("Ghaddim").gameObject.AddComponent<Defense>();
            transform.Find("Mhoddim").gameObject.AddComponent<Offense>();
        }
    }


    private void CreateNavigationMesh()
    {
        GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
    }
}