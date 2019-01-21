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
        public string product_name;
        public int amount;
        public float value_cp;

        public StoredProducts(string _name, int _amount)
        {
            this.product_name = _name;
            this.amount = _amount;
            this.value_cp = Industry.Instance.products.First(p => p.name == _name).market_value_cp * _amount;
        }
    }

    public List<StoredMaterials> raw_materials = new List<StoredMaterials>();
    public List<StoredProducts> finished_goods = new List<StoredProducts>();

    // properties

    public Structure Structure { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
        StartCoroutine(MonitorStorage());
    }


    // public


    public void RemoveMaterials(Resources.Raw _material, int _amount)
    {
        StoredMaterials inventory_row = raw_materials.First(r => r.material == _material);
        int new_amount = inventory_row.amount - _amount;
        if (new_amount < 0) new_amount = 0;
        float value = new_amount * Resources.Instance.resource_valuations.First(rv => rv.material == _material).value_cp;
        raw_materials.Remove(inventory_row);
        raw_materials.Add(new StoredMaterials(_material, new_amount, value));
    }


    public void StoreMaterials(Actor _unit)
    {
        foreach (KeyValuePair<HarvestingNode, int> pair in _unit.Load)
        {
            StoredMaterials inventory_row = raw_materials.First(r => r.material == pair.Key.raw_resource);
            int new_amount = inventory_row.amount + pair.Value;
            float value = new_amount * Resources.Instance.resource_valuations.First(rv => rv.material == pair.Key.raw_resource).value_cp;
            raw_materials.Remove(inventory_row);
            raw_materials.Add(new StoredMaterials(pair.Key.raw_resource, new_amount, value));
        }

        _unit.Load.Clear();
        _unit.harvesting = Resources.Raw.None;
    }


    public void StoreProducts(Industry.Product _product, int _amount)
    {
        StoredProducts inventory_row = finished_goods.First(fg => fg.product_name == _product.name);
        int new_amount = inventory_row.amount + _amount;
        float value = new_amount * _product.market_value_cp;
        finished_goods.Remove(inventory_row);
        finished_goods.Add(new StoredProducts(_product.name, new_amount));
    }


    public bool UsefulToMe(Actor _artisan)
    {
        var my_products = Industry.Instance
                                  .products
                                  .Where(p => _artisan.Stats.Tools.Contains(p.primary_tool) || _artisan.Stats.Tools.Contains(p.primary_tool))
                                  .Select(p => p.name)
                                  .ToList();

        foreach (var stored_good in finished_goods) {
            if (my_products.Contains(stored_good.product_name)) {
                return true;
            }
        }

        return false;
    }


    // private


    private IEnumerator MonitorStorage()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.ActionThreshold); // TODO: make this much longer

            List<Resources.Raw> resources = raw_materials.Select(row => row.material).ToList();
            List<Industry.Product> potential_products = Industry.Instance.products
                .Where(p => resources.Contains(p.primary_raw_material)
                       && (Mathf.Approximately(p.secondary_materials_required, 0)
                           || resources.Contains(p.secondary_raw_material))).ToList();

            foreach (var product in potential_products) {
                if (raw_materials.First(r => r.material == product.primary_raw_material).amount > product.primary_materials_required) {
                    if (product.secondary_raw_material == Resources.Raw.None || raw_materials.First(r => r.material == product.secondary_raw_material).amount > product.secondary_materials_required) {
                        SpawnArtisanFor(product); // if we have the materials, spawn a tool maker if one is not available
                    }
                }
            }
        }
    }


    private void SetComponents()
    {
        Structure = GetComponent<Structure>();
    }


    private void SpawnArtisanFor(Industry.Product _product)
    {
        Actor primary_artistan, secondary_artisan;

        var primary_artisans = Structure.AttachedUnits.Where(a => a.Stats.Tools.Contains(_product.primary_tool)).ToList();
        if (primary_artisans.Count == 0) {
            primary_artistan = (Structure.owner == Conflict.Faction.Ghaddim)
                ? Offense.Instance.SpawnToolUser(_product.primary_tool)
                         : Defense.Instance.SpawnToolUser(_product.primary_tool);
            Structure.AttachedUnits.Add(primary_artistan);
        }

        if (_product.secondary_tool != Proficiencies.Tool.None) {
            var secondary_artisans = Structure.AttachedUnits.Where(a => a.Stats.Tools.Contains(_product.secondary_tool)).ToList();

            if (secondary_artisans.Count == 0) {
                secondary_artisan = (Structure.owner == Conflict.Faction.Ghaddim)
                    ? Offense.Instance.SpawnToolUser(_product.secondary_tool)
                             : Defense.Instance.SpawnToolUser(_product.secondary_tool);
                Structure.AttachedUnits.Add(secondary_artisan);
            }
        }
    }
}
