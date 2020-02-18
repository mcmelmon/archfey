using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Industry : MonoBehaviour
{
    public enum Coin { Copper, Silver, Electrum, Gold, Platinum };

    // properties

    public List<Actor> CurrentlyCrafting { get; set; }
    public static Dictionary<Coin, int> ExchangeRates { get; set; }
    public static Industry Instance { get; set; }

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
        StartCoroutine(MakeProductsFromResources());
    }

    // public


    // private

    private IEnumerator MakeProductsFromResources()
    {
        while (true) {
            foreach (var workshop in FindObjectsOfType<Workshop>()) {
                foreach (var product in Products.Instance.AvailableProducts) {
                    if (product.SufficientMaterialsInStorage(workshop.Structure)) {
                        workshop.CraftByShop(product);
                    }
                }
            }

            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }

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
    }
}