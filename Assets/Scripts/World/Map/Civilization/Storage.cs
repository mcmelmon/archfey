using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [Serializable]
    public struct StoredMaterials
    {
        public string material;
        public int amount;
        public float value_cp;

        public StoredMaterials(string _material, int _amount, float _value)
        {
            this.material = _material;
            this.amount = _amount;
            this.value_cp = _value;
        }
    }

    [Serializable]
    public struct StoredProducts
    {
        public string name;
        public int quantity;
        public float value_cp;

        public StoredProducts(string _name, int _amount)
        {
            this.name = _name;
            this.quantity = _amount;
            this.value_cp = Industry.Products.First(p => p.Name == _name).Value_CP * _amount;
        }
    }

    public List<StoredMaterials> materials = new List<StoredMaterials>();
    public List<StoredProducts> products = new List<StoredProducts>();

    // properties

    public Structure Structure { get; set; }


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


    public void RemoveMaterials(string _material, int _amount)
    {
        StoredMaterials inventory_row = materials.First(r => r.material == _material);
        int new_amount = inventory_row.amount - _amount;
        if (new_amount < 0) new_amount = 0;
        float value = new_amount * 1; // TODO: implement resource value
        materials.Remove(inventory_row);
        materials.Add(new StoredMaterials(_material, new_amount, value));
    }


    public void StoreMaterials(Actor _unit)
    {
        foreach (KeyValuePair<HarvestingNode, int> pair in _unit.Load)
        {
            StoredMaterials inventory_row = materials.First(r => r.material == pair.Key.material);
            int new_amount = inventory_row.amount + pair.Value;
            float value = new_amount * 5;  // TODO: implement resource value
            materials.Remove(inventory_row);
            materials.Add(new StoredMaterials(pair.Key.material, new_amount, value));
        }

        _unit.Load.Clear();
        _unit.harvesting = "";
    }


    public void StoreProducts(Industry.Product _product, int _amount)
    {
        StoredProducts inventory_row = products.First(fg => fg.name == _product.Name);
        int new_amount = inventory_row.quantity + _amount;
        float value = new_amount * _product.Value_CP;
        products.Remove(inventory_row);
        products.Add(new StoredProducts(_product.Name, new_amount));
    }


    public bool UsefulToMe(Actor _artisan)
    {
        var my_products = Industry.Products
                                  .Where(p => _artisan.Stats.Tools.Contains(p.Tool) || _artisan.Stats.Tools.Contains(p.Tool))
                                  .Select(p => p.Name)
                                  .ToList();

        foreach (var stored_good in products) {
            if (my_products.Contains(stored_good.name)) {
                return true;
            }
        }

        return false;
    }


    // private


    private IEnumerator MonitorStorage()
    {
        while (true) {
            if (Industry.Products.Count == 0) { 
                yield return new WaitForSeconds(Turn.ActionThreshold);
            }

            List<string> resources = materials.Select(row => row.material).ToList();
            List<Industry.Product> potential_products = Industry.Products
                .Where(p => resources.Contains(p.Material)).ToList();

            foreach (var product in potential_products) {
                if (materials.First(r => r.material == product.Material).amount > product.MaterialAmount) {
                SpawnArtisanFor(product); // if we have the materials, spawn a tool maker if one is not available
                }
            }

            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private void SetComponents()
    {
        Structure = GetComponent<Structure>();
    }


    private void SpawnArtisanFor(Industry.Product _product)
    {
        Actor primary_artistan;
        Structure random_residence = FindObjectsOfType<Structure>()
            .Where(s => s.owner == Structure.owner && s.purpose == Structure.Purpose.Residential && s.GetComponent<HarvestingNode>() == null)
            .OrderBy(s => UnityEngine.Random.value)
            .ToList()
            .First();

        var primary_artisans = Structure.AttachedUnits.Where(a => a.Stats.Tools.Contains(_product.Tool)).ToList();
        if (primary_artisans.Count == 0) {
            primary_artistan = (Structure.owner == Conflict.Faction.Ghaddim)
                ? Offense.Instance.SpawnToolUser(_product.Tool, random_residence.RandomEntrance().transform)
                         : Defense.Instance.SpawnToolUser(_product.Tool, random_residence.RandomEntrance().transform);
            Structure.AttachedUnits.Add(primary_artistan);
        }
    }
}
