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
    public float amber_pool_maximum;

    // properties

    public float CurrentAmber { get; set; }
    public float CurrentEnergy { get; set; }
    public float CurrentMana { get; set; }


    // Unity


    private void Awake()
    {
        CurrentEnergy = energy_pool_maximum;
        CurrentMana = mana_pool_maximum;
        CurrentAmber = 0;
        ManageAbilityResources();
    }


    private void Start()
    {
    }

    // public


    public float CurrentAmberPercentage()
    {
        return CurrentAmber / amber_pool_maximum;
    }


    public float CurrentEnergyPercentage()
    {
        return CurrentEnergy / energy_pool_maximum;
    }


    public float CurrentManaPercentage()
    {
        return CurrentMana / mana_pool_maximum;
    }


    // private


    private void ManageAbilityResources()
    {
        StartCoroutine(RegenerateAmber());
        StartCoroutine(RegenerateEnergy());
        StartCoroutine(RegenerateMana());
    }


    private IEnumerator RegenerateAmber()
    {
        while (CurrentAmber < amber_pool_maximum)
        {
            CurrentAmber++;
            CommandBarOne.Instance.amber_bar.value = CurrentAmberPercentage();
            yield return new WaitForSeconds(Turn.action_threshold);
        }
    }


    private IEnumerator RegenerateEnergy()
    {
        while (CurrentEnergy < energy_pool_maximum)
        {
            CurrentEnergy += energy_pool_maximum * energy_replenishment_rate;
            yield return new WaitForSeconds(Turn.action_threshold);
        }
    }


    private IEnumerator RegenerateMana()
    {
        while (CurrentMana < mana_pool_maximum) {
            CurrentMana += mana_pool_maximum * mana_replenishment_rate;
            CommandBarOne.Instance.mana_bar.value = CurrentMana;
            yield return new WaitForSeconds(Turn.action_threshold);
        }
    }
}
