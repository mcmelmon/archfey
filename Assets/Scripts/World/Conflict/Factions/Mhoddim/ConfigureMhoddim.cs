using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConfigureMhoddim 
{

    // Defense attributes
    public static Dictionary<string, float> agility_rating = new Dictionary<string, float>();
    public static Dictionary<string, float> armor_rating = new Dictionary<string, float>();
    public static Dictionary<string, float> corporeal_rating = new Dictionary<string, float>();
    public static Dictionary<string, float> counter = new Dictionary<string, float>();
    public static Dictionary<string, float> force_rating = new Dictionary<string, float>();

    public static Dictionary<string, Dictionary<Weapon.DamageType, float>> resistances = new Dictionary<string, Dictionary<Weapon.DamageType, float>>();

    // Health attributes
    public static Dictionary<string, float> recovery_rate = new Dictionary<string, float>();
    public static Dictionary<string, float> starting_health = new Dictionary<string, float>();


    // static



    public static void GenerateStats()
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
        armor_rating["heavy"] = 0.2f;
        corporeal_rating["heavy"] = 0f;
        counter["heavy"] = 0f;
        force_rating["heavy"] = 0.1f;

        agility_rating["striker"] = 0.1f;
        armor_rating["striker"] = 0.1f;
        corporeal_rating["striker"] = 0f;
        counter["striker"] = 5f;
        force_rating["striker"] = 0f;

        agility_rating["scout"] = 0.15f;
        armor_rating["scout"] = 0f;
        corporeal_rating["scout"] = 0f;
        counter["scout"] = 0f;
        force_rating["scout"] = 0f;

        // health
        starting_health["heavy"] = 130f;
        starting_health["scout"] = 70f;
        starting_health["striker"] = 100f;

        recovery_rate["heavy"] = 0f;
        recovery_rate["scout"] = 0f;
        recovery_rate["striker"] = 0f;
    }


    private static void PopulateResistances()
    {
        resistances["heavy"] = new Dictionary<Weapon.DamageType, float>();
        resistances["heavy"][Weapon.DamageType.Arcane] = 0.25f;
        resistances["heavy"][Weapon.DamageType.Blunt] = 0f;
        resistances["heavy"][Weapon.DamageType.Elemental] = 0f;
        resistances["heavy"][Weapon.DamageType.Piercing] = 0f;
        resistances["heavy"][Weapon.DamageType.Poison] = 0f;
        resistances["heavy"][Weapon.DamageType.Slashing] = 0f;

        resistances["scout"] = new Dictionary<Weapon.DamageType, float>();
        resistances["scout"][Weapon.DamageType.Arcane] = 0.25f;
        resistances["scout"][Weapon.DamageType.Blunt] = 0f;
        resistances["scout"][Weapon.DamageType.Elemental] = 0f;
        resistances["scout"][Weapon.DamageType.Piercing] = 0f;
        resistances["scout"][Weapon.DamageType.Poison] = 0f;
        resistances["scout"][Weapon.DamageType.Slashing] = 0f;

        resistances["striker"] = new Dictionary<Weapon.DamageType, float>();
        resistances["striker"][Weapon.DamageType.Arcane] = 0.25f;
        resistances["striker"][Weapon.DamageType.Blunt] = 0f;
        resistances["striker"][Weapon.DamageType.Elemental] = 0f;
        resistances["striker"][Weapon.DamageType.Piercing] = 0f;
        resistances["striker"][Weapon.DamageType.Poison] = 0f;
        resistances["striker"][Weapon.DamageType.Slashing] = 0f;
    }
}
