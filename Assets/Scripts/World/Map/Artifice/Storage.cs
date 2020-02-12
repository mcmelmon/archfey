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
        public Resources.Raw material;
        public int amount;
        public float value_cp;

        public StoredMaterials(Resources.Raw _material, int _amount, float _value)
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
    [SerializeField] List<StoredMaterials> materials = new List<StoredMaterials>();
    [SerializeField] List<StoredProducts> products = new List<StoredProducts>();

    // properties

    public List<StoredMaterials> MaterialsHandled { get; set; }
    public List<StoredProducts> ProductsHandled { get; set; }

    public Structure Structure { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }


    // public


    public void RemoveMaterials(Resources.Raw _material, int _amount)
    {
        StoredMaterials inventory_row = MaterialsHandled.First(r => r.material == _material);
        int new_amount = inventory_row.amount - _amount;
        if (new_amount < 0) new_amount = 0;
        float value = inventory_row.value_cp * new_amount;
        MaterialsHandled.Remove(inventory_row); // we can't update the existing row; have to remove the old and add the new
        MaterialsHandled.Add(new StoredMaterials(_material, new_amount, value)); 
    }


    public void StoreMaterials(Actor harvester)
    {
        foreach (KeyValuePair<Resources.Raw, int> pair in harvester.Load) {
            StoredMaterials inventory_row = MaterialsHandled.First(r => r.material == pair.Key);
            int new_amount = inventory_row.amount + pair.Value;
            float value = inventory_row.value_cp * new_amount;
            MaterialsHandled.Remove(inventory_row); // we can't update the existing row; have to remove the old and add the new
            MaterialsHandled.Add(new StoredMaterials(pair.Key, new_amount, value));
        }

        harvester.Load.Clear(); // TODO: allow harvesters to have multiple materials
    }


    public void StoreProducts(Industry.Product product, int amount)
    {
        StoredProducts inventory_row = ProductsHandled.First(p => p.name == product.Name);
        int new_amount = inventory_row.quantity + amount;
        float value = new_amount * product.Value_CP;
        ProductsHandled.Remove(inventory_row);
        ProductsHandled.Add(new StoredProducts(product.Name, new_amount));
    }


    // private


    private void SetComponents()
    {
        MaterialsHandled = new List<StoredMaterials>(materials);
        ProductsHandled = new List<StoredProducts>(products);
        Structure = GetComponent<Structure>();
    }
}
