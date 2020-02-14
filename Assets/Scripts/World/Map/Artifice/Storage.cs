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
        public Resource stored_material;
        public int stored_amount;
        public float stored_value_cp;

        public StoredMaterials(Resource _material, int _amount, float _value)
        {
            this.stored_material = _material;
            this.stored_amount = _amount;
            this.stored_value_cp = _value;
        }
    }

    [Serializable]
    public struct StoredProducts
    {
        public string stored_product;
        public int stored_amount;
        public float stored_value_cp;

        public StoredProducts(string _name, int _amount, int _value)
        {
            this.stored_product = _name;
            this.stored_amount = _amount;
            this.stored_value_cp = _value;
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


    public void RemoveMaterials(Resource _material, int _amount)
    {
        StoredMaterials inventory_row = MaterialsHandled.First(r => r.stored_material == _material);
        int new_amount = inventory_row.stored_amount - _amount;
        float material_value = Resources.Instance.AvailableResources.Where(ar => ar.Name == inventory_row.stored_material.Name).First().ValueInCopper;
        if (new_amount < 0) new_amount = 0;
        float value = material_value * new_amount;
        MaterialsHandled.Remove(inventory_row); // we can't update the existing row; have to remove the old and add the new
        MaterialsHandled.Add(new StoredMaterials(_material, new_amount, value)); 
    }


    public void StoreMaterials(Actor _worker)
    {
        // TODO: have a single update materials function for add and remove
        
        Resource delivered_material = _worker.Load.First().Key;
        float material_value = Resources.Instance.AvailableResources.Where(ar => ar.Name == delivered_material.Name).First().ValueInCopper;
        int delivered_quantity = _worker.Load.First().Value;

        StoredMaterials current_inventory = MaterialsHandled.First(r => r.stored_material == delivered_material);
        int new_inventory = current_inventory.stored_amount + delivered_quantity;
        float new_value = material_value * new_inventory;
        MaterialsHandled.Remove(current_inventory); // we can't update the existing row; have to remove the old and add the new
        MaterialsHandled.Add(new StoredMaterials(delivered_material, new_inventory, new_value));

        _worker.Load.Clear(); // TODO: allow harvesters to have multiple materials
    }


    public void StoreProducts(Actor worker)
    {
        // StoredProducts inventory_row = ProductsHandled.First(p => p.stored_product == product.Name);
        // int new_amount = inventory_row.stored_amount + amount;
        // float value = new_amount * product.Value_CP;
        // ProductsHandled.Remove(inventory_row);
        // ProductsHandled.Add(new StoredProducts(product.Name, new_amount));
    }


    // private


    private void SetComponents()
    {
        MaterialsHandled = new List<StoredMaterials>(materials);
        ProductsHandled = new List<StoredProducts>(products);
        Structure = GetComponent<Structure>();
    }
}
