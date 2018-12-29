﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffects : MonoBehaviour
{
    // Inspector settings

    public GameObject fountain_of_healing_prefab;
    public GameObject smite_prefab;

    // properties
    public static SpellEffects Instance { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one spell effects instance");
            Destroy(this);
            return;
        }
        Instance = this;
    }

}
