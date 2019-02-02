using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Objectives : MonoBehaviour {

    // Inspector settings
    public List<Objective> objectives;

    // properties

    public static Objectives Instance { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one ruins instance");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // public


    public Objective ObjectiveNearest(Vector3 location)
    {
        return FindObjectsOfType<Objective>().OrderBy(o => Vector3.Distance(o.transform.position, location)).First();
    }
}