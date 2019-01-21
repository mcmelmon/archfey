using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Industry : MonoBehaviour
{
    public enum Coin { Copper, Silver, Electrum, Gold, Platinum };

    [Serializable]
    public struct Product
    {
        public string name;
        public Proficiencies.Tool primary_tool;
        public Proficiencies.Tool secondary_tool;
        public Resources.Raw primary_raw_material;
        public int primary_materials_required;
        public Resources.Raw secondary_raw_material;
        public int secondary_materials_required;
        public int market_value_cp;
    }

    // Inspector settings

    public List<Product> products;

    // properties

    public static Dictionary<Coin, int> ExchangeRates { get; set; }
    public static Industry Instance { get; set; }
    public static List<Actor> Manufacturers { get; set; }


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


    public bool Manufacture(Storage _storage, Product _product, List<Actor> _artisans)
    {
        foreach (var artisan in _artisans) {
            if (Manufacturers.Contains(artisan)) return false;
        }

        Manufacturers.AddRange(_artisans);
        StartCoroutine(ManufacturingProgress(_storage, _product, _artisans));
        return true;
    }


    // private


    private IEnumerator ManufacturingProgress(Storage _storage, Product _product, List<Actor> _artisans)
    {
        int turn = 0;
        float time_to_finish = 12 * _product.market_value_cp / 500f;  // The formula is 1 day per 5gp; but we will say every turn = 2 hours instead of 6 seconds

        while (turn < time_to_finish) {
            foreach (var artisan in _artisans) {
                if (artisan.gameObject == null) break;
            }
            turn++;
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }

        foreach (var artisan in _artisans) {
            Manufacturers.Remove(artisan);
        }

        _storage.StoreProducts(_product, 1);
    }


    private void SetComponents()
    {
        ExchangeRates = new Dictionary<Coin, int>
        {
            [Coin.Copper] = 1,
            [Coin.Silver] = 10,
            [Coin.Electrum] = 50,
            [Coin.Gold] = 100,
            [Coin.Platinum] = 1000
        };
        Manufacturers = new List<Actor>();
    }
}
