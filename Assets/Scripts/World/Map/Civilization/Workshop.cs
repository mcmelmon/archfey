using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Workshop : MonoBehaviour
{
    // Inspector settings
    public List<string> shop_tools = new List<string>();


    // properties

    public List<Industry.Product> Craftworks { get; set; }
    public Storage Storage { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    private void Start()
    {
        StartCoroutine(MonitorStorage());
    }


    // public


    public bool Craft(Actor artisan)
    {
        if (Craftworks.Count == 0 || Industry.CurrentlyCrafting.Contains(artisan)) return false;
        Industry.Product product = null;

        foreach (var tool in artisan.Stats.Tools) {
            // TODO: pick "most valuable" product/tool available to artisan
            product = Craftworks.First(cw => cw.Tool == tool);
            if (product != null) break;
        }

        if (product == null) return false;

        if (Storage.materials.First(m => m.material == product.Material).amount >= product.MaterialAmount) {
            Industry.CurrentlyCrafting.Add(artisan);
            StartCoroutine(Crafting(product, artisan));
            return true;
        }

        return false;
    }


    public bool UsefulTo(Actor artisan)
    {
        foreach (var tool in shop_tools) {
            if (artisan.Stats.Tools.Contains(tool)) {
                return true;
            }
        }

        return false;
    }


    // private


    private IEnumerator Crafting(Industry.Product product, Actor artisan)
    {
        Storage.RemoveMaterials(product.Material, product.MaterialAmount);

        int turn = 0;
        float time_to_finish = 1 + product.Value_CP / 100f;


        while (turn < time_to_finish) {
            if (artisan.gameObject == null) break;
            turn++;
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }

        Industry.CurrentlyCrafting.Remove(artisan);
        Storage.StoreProducts(product, 1);
    }


    public IEnumerator GetProductsMadeWith(string tool)
    {
        // It "may" make sense to grab all of the products at once (in Industry), but when there
        // are a lot of them, we will only need a small subset workshop by workshop as the settlements
        // expand.

        UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/products?tool=" + tool);
        Industry.JSON_Products products_json = new Industry.JSON_Products();
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            products_json = JsonUtility.FromJson<Industry.JSON_Products>(www.downloadHandler.text);
            List<string> products_from_server = new List<string>(products_json.products);
            foreach (var product in products_from_server) {
                string[] split = product.Split(new char[] { ',' });
                Industry.Product new_product = new Industry.Product {
                    Name = split[0],
                    Material = split[1],
                    MaterialAmount = int.Parse(split[2]),
                    Tool = split[3],
                    Value_CP = int.Parse(split[4])
                };

                Craftworks.Add(new_product);
            }
        }
    }


    private IEnumerator MonitorStorage()
    {
        while (true) {
            if (Craftworks.Count == 0) {
                yield return new WaitForSeconds(Turn.ActionThreshold);
            }
            
            foreach (var work in Craftworks) {
                if (Storage.materials.First(m => m.material == work.Material).amount > work.MaterialAmount) {
                    SpawnArtisanFor(work); // if we have the materials, spawn a tool maker if one is not available
                }
            }

            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private void SetComponents()
    {
        Craftworks = new List<Industry.Product>();
        Storage = GetComponent<Storage>();
        foreach (var tool in shop_tools) {
            StartCoroutine(GetProductsMadeWith(tool));
        }
    }


    private void SpawnArtisanFor(Industry.Product _product)
    {
        Actor primary_artistan;
        Structure random_residence = FindObjectsOfType<Structure>()
            .Where(s => s.alignment == Storage.Structure.alignment && s.purpose == Structure.Purpose.Residential && s.GetComponent<HarvestingNode>() == null)
            .OrderBy(s => UnityEngine.Random.value)
            .ToList()
            .First();

        var primary_artisans = Storage.Structure.AttachedUnits.Where(a => a.Stats.Tools.Contains(_product.Tool)).ToList();
        if (primary_artisans.Count == 0)
        {
            primary_artistan = GetComponentInParent<Faction>().SpawnToolUser(_product.Tool, random_residence.RandomEntrance().transform);
            Storage.Structure.AttachedUnits.Add(primary_artistan);
        }
    }
}
