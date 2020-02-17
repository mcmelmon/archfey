using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public enum Category { Bone = 0, Herbs = 1, Hides = 2, Meat = 3, Ore = 4, Timber = 5, Textile = 6 }
    // properties

    public List<Resource> AvailableResources { get; set; }
    public static Resources Instance { get; set; }

    public List<HarvestingNode> HarvestingNodes { get; set; }

    // Unity


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one resources instance!");
            Destroy(this);
            return;
        }
        Instance = this;
        SetComponents();
    }


    // private

    void SetComponents()
    {
        AvailableResources = new List<Resource>(GetComponentsInChildren<Resource>());
        HarvestingNodes = new List<HarvestingNode>(GetComponentsInChildren<HarvestingNode>());
    }
}
