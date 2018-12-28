using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour
{

    // Inspector settings
    public float energy_haste_factor;
    public float energy_pool_maximum;
    public float energy_potency;
    public float energy_replenishment_rate;
    public float mana_haste_factor;
    public float mana_pool_maximum;
    public float magic_potency;
    public float mana_replenishment_rate;

    // properties

    public float CurrentEnergy { get; set; }
    public float CurrentMana { get; set; }


    // Unity


    private void Awake()
    {
        CurrentEnergy = energy_pool_maximum;
        CurrentMana = mana_pool_maximum;
    }


    // public


    public float CurrentEnergyPercentage()
    {
        return CurrentEnergy / energy_pool_maximum;
    }


    public float CurrentManaPercentage()
    {
        return CurrentMana / mana_pool_maximum;
    }
}
