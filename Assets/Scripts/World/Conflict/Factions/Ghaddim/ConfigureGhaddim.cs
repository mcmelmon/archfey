using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConfigureGhaddim 
{

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
        // TODO: use frickin enums

        // defense
        agility_rating["heavy"] = 0f;
        armor_rating["heavy"] = 0f;
        corporeal_rating["heavy"] = 0.3f;
        counter["heavy"] = 0f;
        force_rating["heavy"] = 0f;

        agility_rating["striker"] = 0.05f;
        armor_rating["striker"] = 0f;
        corporeal_rating["striker"] = 0.1f;
        counter["striker"] = 0f;
        force_rating["striker"] = 0f;

        agility_rating["scout"] = 0.15f;
        armor_rating["scout"] = 0f;
        corporeal_rating["scout"] = 0.1f;
        counter["scout"] = 0f;
        force_rating["scout"] = 0f;

        // health
        starting_health["scout"] = 100f;
        starting_health["striker"] = 130f;
        starting_health["heavy"] = 160f;

        recovery_rate["scout"] = 0.05f;
        recovery_rate["striker"] = 0.075f;
        recovery_rate["heavy"] = 0.1f;
    }


    private static void PopulateResistances()
    {
        resistances["heavy"] = new Dictionary<Weapon.Type, float>();
        resistances["heavy"][Weapon.Type.Arcane] = 0f;
        resistances["heavy"][Weapon.Type.Blunt] = 0f;
        resistances["heavy"][Weapon.Type.Elemental] = 0f;
        resistances["heavy"][Weapon.Type.Piercing] = 0.5f;
        resistances["heavy"][Weapon.Type.Poison] = 1f;
        resistances["heavy"][Weapon.Type.Slashing] = 0f;

        resistances["scout"] = new Dictionary<Weapon.Type, float>();
        resistances["scout"][Weapon.Type.Arcane] = 0.25f;
        resistances["scout"][Weapon.Type.Blunt] = 0f;
        resistances["scout"][Weapon.Type.Elemental] = 0f;
        resistances["scout"][Weapon.Type.Piercing] = 0.5f;
        resistances["scout"][Weapon.Type.Poison] = 1f;
        resistances["scout"][Weapon.Type.Slashing] = 0f;

        resistances["striker"] = new Dictionary<Weapon.Type, float>();
        resistances["striker"][Weapon.Type.Arcane] = 0.25f;
        resistances["striker"][Weapon.Type.Blunt] = 0f;
        resistances["striker"][Weapon.Type.Elemental] = 0f;
        resistances["striker"][Weapon.Type.Piercing] = 0.5f;
        resistances["striker"][Weapon.Type.Poison] = 1f;
        resistances["striker"][Weapon.Type.Slashing] = 0f;
    }
}
