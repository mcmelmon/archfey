using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class ConfigureFey {

    // Defense attributes
    public static Dictionary<Soldier.Clasification, float> agility_rating = new Dictionary<Soldier.Clasification, float>();
    public static Dictionary<Soldier.Clasification, float> armor_rating = new Dictionary<Soldier.Clasification, float>();
    public static Dictionary<Soldier.Clasification, float> corporeal_rating = new Dictionary<Soldier.Clasification, float>();
    public static Dictionary<Soldier.Clasification, float> counter = new Dictionary<Soldier.Clasification, float>();
    public static Dictionary<Soldier.Clasification, float> force_rating = new Dictionary<Soldier.Clasification, float>();

    public static Dictionary<Soldier.Clasification, Dictionary<Weapon.DamageType, float>> resistances = new Dictionary<Soldier.Clasification, Dictionary<Weapon.DamageType, float>>();

    // Health attributes
    public static Dictionary<Soldier.Clasification, float> recovery_rate = new Dictionary<Soldier.Clasification, float>();
    public static Dictionary<Soldier.Clasification, float> starting_health = new Dictionary<Soldier.Clasification, float>();


    // static


    public static void GenerateStats()
    {
        PopulateAttributes();
        PopulateResistances();
    }


    // private


    private static void PopulateAttributes()
    {
        // defense
        agility_rating[Soldier.Clasification.Ent] = 0f;
        armor_rating[Soldier.Clasification.Ent] = 0.35f;
        corporeal_rating[Soldier.Clasification.Ent] = 0f;
        counter[Soldier.Clasification.Ent] = 5f;
        force_rating[Soldier.Clasification.Ent] = 0f;

        // health
        recovery_rate[Soldier.Clasification.Ent] = 0.075f;
        starting_health[Soldier.Clasification.Ent] = 500;
    }

    private static void PopulateResistances()
    {
        resistances[Soldier.Clasification.Ent] = new Dictionary<Weapon.DamageType, float>();
        resistances[Soldier.Clasification.Ent][Weapon.DamageType.Arcane] = 0f;
        resistances[Soldier.Clasification.Ent][Weapon.DamageType.Blunt] = 0.5f;
        resistances[Soldier.Clasification.Ent][Weapon.DamageType.Elemental] = 0f;
        resistances[Soldier.Clasification.Ent][Weapon.DamageType.Piercing] = .2f;
        resistances[Soldier.Clasification.Ent][Weapon.DamageType.Poison] = .2f;
        resistances[Soldier.Clasification.Ent][Weapon.DamageType.Slashing] = .4f;
    }
}
