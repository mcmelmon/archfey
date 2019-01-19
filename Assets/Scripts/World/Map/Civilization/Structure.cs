using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public enum Purpose { Civic = 0, Commercial = 1, Industrial = 2, Military = 3, Residential = 4, Sacred = 5 };

    // Inspector settings
    public Conflict.Faction owner;
    public Purpose purpose;

    public int armor_class = 13;
    public int damage_resistance;
    public int maximum_hit_points = 100;

    public List<Transform> entrances = new List<Transform>();

    public float revenue_factor;
    public float revenue_cp;

    [Serializable]
    public struct InventoriedRawMaterials {
        public Resources.Raw material;
        public int amount;
        public float value_cp;

        public InventoriedRawMaterials(Resources.Raw _material, int _amount, float _value) {
            this.material = _material;
            this.amount = _amount;
            this.value_cp = _value;
        }
    }

    [Serializable]
    public struct InventoriedGoods
    {
        public string product_name;
        public int amount;
        public float value_cp;

        public InventoriedGoods(string _name, int _amount)
        {
            this.product_name = _name;
            this.amount = _amount;
            this.value_cp = Industry.Instance.products.First(p => p.name == _name).market_value_cp * _amount;
        }
    }

    public List<InventoriedRawMaterials> raw_materials = new List<InventoriedRawMaterials>();
    public List<InventoriedGoods> finished_goods = new List<InventoriedGoods>();

    // properties

    public List<Actor> AttachedArtisans { get; set; }
    public int CurrentHitPoints { get; set; }
    public float OriginalY { get; set; }
    public float OriginalYScale { get; set; }
    public Dictionary<Weapon.DamageType, int> Resistances { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
        StartCoroutine(MonitorInventory());
    }

    // public


    public float CurrentHitPointPercentage()
    {
        return ((float)CurrentHitPoints / (float)maximum_hit_points);
    }


    public void GainStructure(int _amount)
    {
        if (CurrentHitPoints == maximum_hit_points) return;

        CurrentHitPoints += _amount;
        if (CurrentHitPoints > maximum_hit_points) CurrentHitPoints = maximum_hit_points;
        UpdateStructure();
    }


    public void LoseStructure(int _amount, Weapon.DamageType _type)
    {
        if (CurrentHitPoints == 0) return;

        int reduced_amount = (_amount - damage_resistance > 0) ? _amount - damage_resistance : 0;
        CurrentHitPoints -= DamageAfterResistance(reduced_amount, _type);
        if (CurrentHitPoints <= 0) CurrentHitPoints = 0;
        UpdateStructure();
    }


    public Transform NearestEntranceTo(Transform _location)
    {
        return entrances.OrderBy(s => Vector3.Distance(transform.position, _location.position)).Reverse().ToList().First();
    }


    public void StoreFinishedGoods(Industry.Product _product, int _amount)
    {
        InventoriedGoods inventory_row = finished_goods.First(fg => fg.product_name == _product.name);
        int new_amount = inventory_row.amount + _amount;
        float value = new_amount * _product.market_value_cp;
        finished_goods.Remove(inventory_row);
        finished_goods.Add(new InventoriedGoods(_product.name, new_amount));
    }


    public void TransactBusiness(Actor _unit, float _amount)
    {
        BookRevenue(_amount);
        StoreGoods(_unit);
    }


    public List<Resources.Raw> Wants()
    {
        var desired_goods = raw_materials.Select(good => good.material);
        return desired_goods.ToList();
    }


    // private


    private void AttachArtisansFor(Industry.Product _product)
    {
        var primary_artisans = AttachedArtisans.Where(aa => aa.Stats.Tools.Contains(_product.primary_tool)).ToList();
        if (primary_artisans.Count == 0) {
            Actor artisan = (owner == Conflict.Faction.Ghaddim) 
                ? Offense.Instance.SpawnToolUser(_product.primary_tool, entrances[0]) 
                         : Defense.Instance.SpawnToolUser(_product.primary_tool, entrances[0]);

            AttachedArtisans.Add(artisan);
        }

        if (_product.secondary_tool != Proficiencies.Tool.None) {
            var secondary_artisans = AttachedArtisans.Where(aa => aa.Stats.Tools.Contains(_product.secondary_tool)).ToList();

            if (secondary_artisans.Count == 0) {
                Actor artisan = (owner == Conflict.Faction.Ghaddim)
                    ? Offense.Instance.SpawnToolUser(_product.secondary_tool, entrances[1])
                             : Defense.Instance.SpawnToolUser(_product.secondary_tool, entrances[1]);

                AttachedArtisans.Add(artisan);
            }
        }
    }


    private void BookRevenue(float _amount)
    {
        float factored_amount = _amount * revenue_factor;
        revenue_cp += (owner == Conflict.Faction.Ghaddim) ? Ghaddim.AfterTaxIncome(factored_amount) : Mhoddim.AfterTaxIncome(factored_amount);
    }


    private int DamageAfterResistance(int _damage, Weapon.DamageType _type)
    {
        return (_damage <= 0 || Resistances == null) ? _damage : (_damage -= _damage * (Resistances[_type] / 100));
    }


    private IEnumerator MonitorInventory()
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
                        AttachArtisansFor(product); // if we have the materials, spawn a tool maker if one is not available


                        if (Industry.Instance.Manufacture(this, product, AttachedArtisans)) {
                            RemoveRawMaterials(product.primary_raw_material, product.primary_materials_required);
                            if (product.secondary_raw_material != Resources.Raw.None)
                                RemoveRawMaterials(product.secondary_raw_material, product.secondary_materials_required);
                        }
                    }
                }
            }
        }
    }


    private void RemoveRawMaterials(Resources.Raw _material, int _amount)
    {
        InventoriedRawMaterials inventory_row = raw_materials.First(r => r.material == _material);
        int new_amount = inventory_row.amount - _amount;
        if (new_amount < 0) new_amount = 0;
        float value = new_amount * Resources.Instance.resource_valuations.First(rv => rv.material == _material).value_cp;
        raw_materials.Remove(inventory_row);
        raw_materials.Add(new InventoriedRawMaterials(_material, new_amount, value));
    }


    private void SetComponents()
    {
        AttachedArtisans = new List<Actor>();
        CurrentHitPoints = maximum_hit_points;
        OriginalY = transform.position.y;
        OriginalYScale = transform.localScale.y;
        Resistances = new Dictionary<Weapon.DamageType, int>  // TODO: override some reistances
        {
            [Weapon.DamageType.Acid] = 0,
            [Weapon.DamageType.Bludgeoning] = 25,
            [Weapon.DamageType.Cold] = 25,
            [Weapon.DamageType.Fire] = -50,
            [Weapon.DamageType.Force] = 0,
            [Weapon.DamageType.Lightning] = 0,
            [Weapon.DamageType.Necrotic] = 0,
            [Weapon.DamageType.Piercing] = 50,
            [Weapon.DamageType.Poison] = 100,
            [Weapon.DamageType.Psychic] = 100,
            [Weapon.DamageType.Slashing] = 25,
            [Weapon.DamageType.Thunder] = 0
        };
    }


    private void StoreGoods(Actor _unit)
    {
        foreach(KeyValuePair<HarvestingNode, int> pair in _unit.Load) {
            InventoriedRawMaterials inventory_row = raw_materials.First(r => r.material == pair.Key.raw_resource);
            int new_amount = inventory_row.amount + pair.Value;
            float value = new_amount * Resources.Instance.resource_valuations.First(rv => rv.material == pair.Key.raw_resource).value_cp; 
            raw_materials.Remove(inventory_row);
            raw_materials.Add(new InventoriedRawMaterials(pair.Key.raw_resource, new_amount, value));
        }

        _unit.Load.Clear();
        _unit.harvesting = Resources.Raw.None;
    }


    private void UpdateStructure()
    {
        if (CurrentHitPoints == maximum_hit_points) return;

        Vector3 scaling = transform.localScale;
        Vector3 position = transform.position;

        switch (CurrentHitPointPercentage()) {
            case float n when (n >= 0.33f && n <= 0.66f):
                scaling.y = OriginalYScale * 0.66f;
                transform.position = new Vector3(transform.position.x, OriginalY * 0.66f, transform.position.z);
                break;
            case float n when (n >= 0.1f && n < 0.33f):
                scaling.y = OriginalYScale * 0.33f;
                transform.position = new Vector3(transform.position.x, OriginalY * 0.33f, transform.position.z);
                break;
            case float n when (n < 0.1f):
                scaling.y = OriginalYScale * 0.01f;
                transform.position = new Vector3(transform.position.x, OriginalY * 0.01f, transform.position.z);
                break;
        }

        transform.localScale = scaling;
    }
}
