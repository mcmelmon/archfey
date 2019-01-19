using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public enum Raw
    {
        Copper = 0,
        Farm = 1,
        Fish = 2,
        Game = 3,
        Gold = 4,
        Iron = 5,
        Skins = 6,
        Timber = 7,
        None = 99
    };

    [Serializable]
    public struct RawValue {
        public Raw material;
        public float value_cp ;
    }

    // Inspector settings

    public List<HarvestingNode> harvesting_nodes;
    public List<RawValue> resource_valuations;

    // properties

    public static Resources Instance { get; set; }


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
    }


    // private
}
