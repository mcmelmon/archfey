using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class ConfigureFey {

    // Versatile attributes
    public static Dictionary<Soldier.Clasification, int> agility_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> strength_rating = new Dictionary<Soldier.Clasification, int>();

    // Defense attributes
    public static Dictionary<Soldier.Clasification, int> armor_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> constitution_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> counter = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> force_rating = new Dictionary<Soldier.Clasification, int>();

    public static Dictionary<Soldier.Clasification, Dictionary<Weapon.DamageType, int>> resistances = new Dictionary<Soldier.Clasification, Dictionary<Weapon.DamageType, int>>();

    // Health attributes
    public static Dictionary<Soldier.Clasification, int> recovery_amount = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> starting_health = new Dictionary<Soldier.Clasification, int>();


    // static


    public static void GenerateStats()
    {
        PopulateAttributes();
        PopulateResistances();
    }


    // private


    private static void PopulateAttributes()
    {
        // versatile
        agility_rating[Soldier.Clasification.Ent] = 0;
        strength_rating[Soldier.Clasification.Ent] = 6;

        // defense
        armor_rating[Soldier.Clasification.Ent] = 4;
        constitution_rating[Soldier.Clasification.Ent] = 0;
        force_rating[Soldier.Clasification.Ent] = 0;

        // health
        recovery_amount[Soldier.Clasification.Ent] = 15;
        starting_health[Soldier.Clasification.Ent] = 500;
    }

    private static void PopulateResistances()
    {
        resistances[Soldier.Clasification.Ent] = new Dictionary<Weapon.DamageType, int>();
        resistances[Soldier.Clasification.Ent][Weapon.DamageType.Arcane] = 0;
        resistances[Soldier.Clasification.Ent][Weapon.DamageType.Blunt] = 50;
        resistances[Soldier.Clasification.Ent][Weapon.DamageType.Elemental] = 0;
        resistances[Soldier.Clasification.Ent][Weapon.DamageType.Piercing] = 60;
        resistances[Soldier.Clasification.Ent][Weapon.DamageType.Poison] = 40;
        resistances[Soldier.Clasification.Ent][Weapon.DamageType.Slashing] = 40;
    }
}
