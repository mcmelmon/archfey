using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Products : MonoBehaviour
{
    public enum Category { Armor = 0, Clothing = 1, Construction = 2, Food = 3, Sacred = 4, Tool = 5, Transportation = 6, Weapon = 7 }

    // properties

    public List<Product> AvailableProducts { get; set; }
    public static Products Instance { get; set; }

    // Unity

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one products instance!");
            Destroy(this);
            return;
        }
        Instance = this;
        SetComponents();
    }


    // private

    void SetComponents()
    {
        AvailableProducts = new List<Product>(GetComponentsInChildren<Product>());
    }
}
