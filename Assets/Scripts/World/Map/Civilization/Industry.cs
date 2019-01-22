using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Industry : MonoBehaviour
{
    public enum Coin { Copper, Silver, Electrum, Gold, Platinum };

    // properties

    public static Dictionary<Coin, int> ExchangeRates { get; set; }
    public static Industry Instance { get; set; }
    public static List<Actor> Crafters { get; set; }
    public static List<Product> Products { get; set; }


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
        StartCoroutine(GetProductsFromServer());
    }


    // public


    public bool Craft(Storage _storage, Product _product, Actor _artisan)
    {
        if (Crafters.Contains(_artisan)) return false;

        Crafters.Add(_artisan);
        StartCoroutine(Crafting(_storage, _product, _artisan));
        return true;
    }


    // private


    private IEnumerator Crafting(Storage _storage, Product _product, Actor _artisan)
    {
        int turn = 0;
        float time_to_finish = 1 + _product.Value_CP / 100f;

        while (turn < time_to_finish) {
            if (_artisan.gameObject == null) break;
            turn++;
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }

        Crafters.Remove(_artisan);
        _storage.StoreProducts(_product, 1);
    }


    public IEnumerator GetProductsFromServer()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/products/");
        JSON_Products products_json = new JSON_Products();
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            products_json = JsonUtility.FromJson<JSON_Products>(www.downloadHandler.text);
            List<string> products = new List<string>(products_json.products);
            foreach (var product in products) {
                string[] split = product.Split(new char[]{','});
                Product new_product = new Product
                {
                    Name = split[0],
                    Material = split[1],
                    MaterialAmount = int.Parse(split[2]),
                    Tool = split[3],
                    Value_CP = int.Parse(split[4])
                };

                Products.Add(new_product);
            }
        }
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
        Crafters = new List<Actor>();
        Products = new List<Product>();
    }

    public class Product
    {
        // properties

        public string Name { get; set; }
        public string Material { get; set; }
        public int MaterialAmount { get; set; }
        public string Tool { get; set; }
        public int Value_CP { get; set; }
    }


    public class JSON_Products
    {
        public string[] products;
    }


    public class JSON_Product
    {
        public string name;
        public string material;
        public int material_amount;
        public string tool;
        public int value_cp;
    }
}