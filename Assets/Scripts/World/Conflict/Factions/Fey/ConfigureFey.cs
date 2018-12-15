using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class ConfigureFey {

    // Defense attributes
    public static Dictionary<string, float> agility_rating = new Dictionary<string, float>();
    public static Dictionary<string, float> armor_rating = new Dictionary<string, float>();
    public static Dictionary<string, float> corporeal_rating = new Dictionary<string, float>();
    public static Dictionary<string, float> counter = new Dictionary<string, float>();
    public static Dictionary<string, float> force_rating = new Dictionary<string, float>();

    public static Dictionary<string, Dictionary<Weapon.Type, float>> resistances = new Dictionary<string, Dictionary<Weapon.Type, float>>();

    // Health attributes
    public static Dictionary<string, float> recovery_rate = new Dictionary<string, float>();
    public static Dictionary<string, float> starting_health = new Dictionary<string, float>();


    // static


    public static void Populate()
    {
        PopulateAttributes();
        PopulateResistances();
    }


    // private


    private static void PopulateAttributes()
    {
        // defense
        agility_rating["ent"] = 0f;
        armor_rating["ent"] = 0.35f;
        corporeal_rating["ent"] = 0f;
        counter["ent"] = 5f;
        force_rating["ent"] = 0f;

        // health
        recovery_rate["ent"] = 0.075f;
        starting_health["ent"] = 500;
    }

    private static void PopulateResistances()
    {
        resistances["ent"] = new Dictionary<Weapon.Type, float>();
        resistances["ent"][Weapon.Type.Arcane] = 0f;
        resistances["ent"][Weapon.Type.Blunt] = 0.5f;
        resistances["ent"][Weapon.Type.Elemental] = 0f;
        resistances["ent"][Weapon.Type.Piercing] = .2f;
        resistances["ent"][Weapon.Type.Poison] = .2f;
        resistances["ent"][Weapon.Type.Slashing] = .4f;
    }
}
