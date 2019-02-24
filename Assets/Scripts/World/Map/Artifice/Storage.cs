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
            this.value_cp = 5; // placeholder
        }
    }

    // Inspector settings
    public List<StoredMaterials> materials = new List<StoredMaterials>();
    public List<StoredProducts> products = new List<StoredProducts>();

    // properties

    public Structure Structure { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
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


    public void StoreMaterials(Actor harvester)
    {
        foreach (KeyValuePair<HarvestingNode, int> pair in harvester.Load)
        {
            StoredMaterials inventory_row = materials.First(r => r.material == pair.Key.material);
            int new_amount = inventory_row.amount + pair.Value;
            float value = new_amount * 5;  // TODO: implement resource value
            materials.Remove(inventory_row);
            materials.Add(new StoredMaterials(pair.Key.material, new_amount, value));
        }

        harvester.Load.Clear();
    }


    public void StoreProducts(Industry.Product product, int amount)
    {
        StoredProducts inventory_row = products.First(p => p.name == product.Name);
        int new_amount = inventory_row.quantity + amount;
        float value = new_amount * product.Value_CP;
        products.Remove(inventory_row);
        products.Add(new StoredProducts(product.Name, new_amount));
    }


    // private


    private void SetComponents()
    {
        Structure = GetComponent<Structure>();
    }
}
