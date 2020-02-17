using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Products : MonoBehaviour
{

    // Inspector settings

    [SerializeField] List<Product> available_products;

    // properties

    public List<Product> AvailableProducts { get; set; }
    public static Products Instance { get; set; }


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
        AvailableProducts = new List<Product>(available_products);
    }
}
