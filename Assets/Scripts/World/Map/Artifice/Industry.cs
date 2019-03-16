using System.Collections.Generic;
using UnityEngine;


public class Industry : MonoBehaviour
{
    public enum Coin { Copper, Silver, Electrum, Gold, Platinum };

    // properties

    public static List<Actor> CurrentlyCrafting { get; set; }
    public static Dictionary<Coin, int> ExchangeRates { get; set; }
    public static Industry Instance { get; set; }
    // TODO: accumulate all product/workshop/tool/material combinations

    // Unity


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one industry instance!");
            Destroy(this);
            return;
        }
        Instance = this;
        SetComponents();
    }


    // public


    // private


    private void SetComponents()
    {
        CurrentlyCrafting = new List<Actor>();
        ExchangeRates = new Dictionary<Coin, int>
        {
            [Coin.Copper] = 1,
            [Coin.Silver] = 10,
            [Coin.Electrum] = 50,
            [Coin.Gold] = 100,
            [Coin.Platinum] = 1000
        };
        //Products = new List<Product>();
    }

    public class Product
    {
        // properties

        public string Name { get; set; }
        public string Material { get; set; }
        public int MaterialAmount { get; set; }
        public Proficiencies.Tool Tool { get; set; }
        public int Value_CP { get; set; }
    }


    public class JSON_Products
    {
        public string[] products;
    }
}