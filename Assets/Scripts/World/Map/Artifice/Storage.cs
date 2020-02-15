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
    public List<Resource> materials_accepted = new List<Resource>();
    public List<Product> products_accepted = new List<Product>();

    // properties

    public List<StoredMaterials> MaterialsStored { get; set; }
    public List<StoredProducts> ProductsStored { get; set; }
    public Structure Structure { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }


    // public

    public void DeliverMaterials(Actor _worker)
    {
        for (int i = 0; i < _worker.Inventory.Contents.Count; i++) {
            GameObject item = _worker.Inventory.Contents[i];
            Resource material = item.GetComponent<Resource>();
            Product product = item.GetComponent<Product>();

            if (material != null && materials_accepted.Contains(material)) {
                StoreMaterials(material);
                _worker.Inventory.RemoveFromInventory(item);
            } else if (product != null && products_accepted.Contains(product)) {

            }         
        }

        _worker.IsEncumbered();

        Debug.Log("Stored value: " + StoredValue());
    }

    public float StoredValue()
    {
        float stored_value = 0f;

        foreach (var item in MaterialsStored) {
            Debug.Log("Item: " + item.stored_material);
            Debug.Log("Value: " + item.stored_value_cp);
            stored_value += item.stored_value_cp;
        }

        foreach (var item in ProductsStored) {
            stored_value += item.stored_value_cp;
        }

        return stored_value;
    }


    // private


    private void SetComponents()
    {
        MaterialsStored = new List<StoredMaterials>();
        ProductsStored = new List<StoredProducts>();
        Structure = GetComponent<Structure>();
    }

    private void StoreMaterials(Resource _material)
    {
        float material_value = Resources.Instance.AvailableResources.Where(ar => ar.Name == _material.Name).First().GetComponent<Item>().GetAdjustedValueInCopper();
        Debug.Log("Material value: " + material_value);

        // TODO: store individual game objects like Inventory in rows and then use Linq to get counts

        if (MaterialsStored.Count == 0) {
            MaterialsStored.Add(new StoredMaterials(_material, 1, material_value));
        } else {
            StoredMaterials current_inventory = MaterialsStored.First(r => r.stored_material == _material);
            int new_inventory = current_inventory.stored_amount + 1;
            float new_value = material_value * new_inventory;
            MaterialsStored.Remove(current_inventory); // we can't update the existing row; have to remove the old and add the new
            MaterialsStored.Add(new StoredMaterials(_material, new_inventory, new_value));
        }
    }
}
