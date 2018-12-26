using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellcasting : MonoBehaviour
{

    // Inspector settings
    public float maximum_mana_pool;
    public float replenishment_percentage;
    public float spell_potency;

    // properties

    public float CurrentMana { get; set; }


    // Unity


    private void Awake()
    {
        CurrentMana = maximum_mana_pool;
    }


    // public


    public float CurrentManaPercentage()
    {
        return CurrentMana / maximum_mana_pool;
    }
}
