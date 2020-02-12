using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public enum Raw
    {
        Adamantite,
        Copper,
        Electrum,
        Farm,
        Fish,
        Game,
        Gold,
        Herbs,
        Iron,
        Mithril,
        Silver,
        Skins,
        Timber,
        Tin,
        None
    };

    [Serializable]
    public struct RawValue {
        public Raw material;
        public float value_cp ;
    }

    // Inspector settings

    public List<RawValue> resource_valuations;

    // properties

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
        HarvestingNodes = new List<HarvestingNode>(GetComponentsInChildren<HarvestingNode>());
    }
}
