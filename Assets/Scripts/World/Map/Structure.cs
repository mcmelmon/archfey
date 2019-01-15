using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public enum Purpose { Civic = 0, Commercial = 1, Military = 2, Religious = 3, Residential = 4 };

    // Inspector settings
    public int armor_class = 13;
    public int damage_resistance = 0;
    public int maximum_hit_points = 100;

    public List<Transform> entrances = new List<Transform>();
    public Conflict.Faction owner;
    public Purpose purpose;

    public float revenue_factor;


    // properties

    public int CurrentHitPoints { get; set; }
    public float OriginalY { get; set; }
    public float OriginalYScale { get; set; }
    public Dictionary<Weapon.DamageType, int> Resistances { get; set; }
    public float Revenue { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }

    // public


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


    public void TransactBusiness(float _amount)
    {
        float factored_amount = _amount * revenue_factor;
        Revenue += (owner == Conflict.Faction.Ghaddim) ? Ghaddim.AfterTaxIncome(factored_amount) : Mhoddim.AfterTaxIncome(factored_amount);
    }


    // private


    private int DamageAfterResistance(int _damage, Weapon.DamageType _type)
    {
        return (_damage <= 0 || Resistances == null) ? _damage : (_damage -= _damage * (Resistances[_type] / 100));
    }


    private float CurrentHitPointPercentage()
    {
        return ((float)CurrentHitPoints / (float)maximum_hit_points);
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
        Revenue = 0f;
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
