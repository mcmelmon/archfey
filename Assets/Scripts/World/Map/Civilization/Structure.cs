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
    public float revenue;

    [Serializable]
    public struct InventoriedGoods {
        public Resources.Type type;
        public int amount;

        public InventoriedGoods(Resources.Type _type, int _amount) {
            this.type = _type;
            this.amount = _amount;
        }
    }

    public List<InventoriedGoods> inventory_rows = new List<InventoriedGoods>();

    // properties

    public int CurrentHitPoints { get; set; }
    public float OriginalY { get; set; }
    public float OriginalYScale { get; set; }
    public Dictionary<Weapon.DamageType, int> Resistances { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
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


    public void TransactBusiness(Actor _unit, float _amount)
    {
        BookRevenue(_amount);
        StoreGoods(_unit);
    }


    public List<Resources.Type> Wants()
    {
        var desired_goods = inventory_rows.Select(good => good.type);
        return desired_goods.ToList();
    }


    // private


    private void BookRevenue(float _amount)
    {
        float factored_amount = _amount * revenue_factor;
        revenue += (owner == Conflict.Faction.Ghaddim) ? Ghaddim.AfterTaxIncome(factored_amount) : Mhoddim.AfterTaxIncome(factored_amount);
    }


    private int DamageAfterResistance(int _damage, Weapon.DamageType _type)
    {
        return (_damage <= 0 || Resistances == null) ? _damage : (_damage -= _damage * (Resistances[_type] / 100));
    }

    private void SetComponents()
    {
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
        foreach(KeyValuePair<Resource, int> pair in _unit.Load) {
            InventoriedGoods inventory_row = inventory_rows.First(r => r.type == pair.Key.resource_type);
            int amount = inventory_row.amount + pair.Value;
            inventory_rows.Remove(inventory_row);
            inventory_rows.Add(new InventoriedGoods(pair.Key.resource_type, amount));
        }

        _unit.Load.Clear();
        _unit.harvesting = Resources.Type.None;
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
