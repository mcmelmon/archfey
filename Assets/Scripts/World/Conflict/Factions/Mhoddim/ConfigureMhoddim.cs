using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConfigureMhoddim 
{

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
        // TODO: use frickin enums

        // defense
        agility_rating[Soldier.Clasification.Heavy] = 0f;
        armor_rating[Soldier.Clasification.Heavy] = 0.2f;
        corporeal_rating[Soldier.Clasification.Heavy] = 0f;
        counter[Soldier.Clasification.Heavy] = 0f;
        force_rating[Soldier.Clasification.Heavy] = 0.1f;

        agility_rating[Soldier.Clasification.Scout] = 0.1f;
        armor_rating[Soldier.Clasification.Scout] = 0.1f;
        corporeal_rating[Soldier.Clasification.Scout] = 0f;
        counter[Soldier.Clasification.Scout] = 5f;
        force_rating[Soldier.Clasification.Scout] = 0f;

        agility_rating[Soldier.Clasification.Striker] = 0.15f;
        armor_rating[Soldier.Clasification.Striker] = 0f;
        corporeal_rating[Soldier.Clasification.Striker] = 0f;
        counter[Soldier.Clasification.Striker] = 0f;
        force_rating[Soldier.Clasification.Striker] = 0f;

        // health
        starting_health[Soldier.Clasification.Heavy] = 130f;
        starting_health[Soldier.Clasification.Scout] = 70f;
        starting_health[Soldier.Clasification.Striker] = 100f;

        recovery_rate[Soldier.Clasification.Heavy] = 0f;
        recovery_rate[Soldier.Clasification.Scout] = 0f;
        recovery_rate[Soldier.Clasification.Striker] = 0f;
    }


    private static void PopulateResistances()
    {
        resistances[Soldier.Clasification.Heavy] = new Dictionary<Weapon.DamageType, float>();
        resistances[Soldier.Clasification.Heavy][Weapon.DamageType.Arcane] = 0.25f;
        resistances[Soldier.Clasification.Heavy][Weapon.DamageType.Blunt] = 0f;
        resistances[Soldier.Clasification.Heavy][Weapon.DamageType.Elemental] = 0f;
        resistances[Soldier.Clasification.Heavy][Weapon.DamageType.Piercing] = 0f;
        resistances[Soldier.Clasification.Heavy][Weapon.DamageType.Poison] = 0f;
        resistances[Soldier.Clasification.Heavy][Weapon.DamageType.Slashing] = 0f;

        resistances[Soldier.Clasification.Scout] = new Dictionary<Weapon.DamageType, float>();
        resistances[Soldier.Clasification.Scout][Weapon.DamageType.Arcane] = 0.25f;
        resistances[Soldier.Clasification.Scout][Weapon.DamageType.Blunt] = 0f;
        resistances[Soldier.Clasification.Scout][Weapon.DamageType.Elemental] = 0f;
        resistances[Soldier.Clasification.Scout][Weapon.DamageType.Piercing] = 0f;
        resistances[Soldier.Clasification.Scout][Weapon.DamageType.Poison] = 0f;
        resistances[Soldier.Clasification.Scout][Weapon.DamageType.Slashing] = 0f;

        resistances[Soldier.Clasification.Striker] = new Dictionary<Weapon.DamageType, float>();
        resistances[Soldier.Clasification.Striker][Weapon.DamageType.Arcane] = 0.25f;
        resistances[Soldier.Clasification.Striker][Weapon.DamageType.Blunt] = 0f;
        resistances[Soldier.Clasification.Striker][Weapon.DamageType.Elemental] = 0f;
        resistances[Soldier.Clasification.Striker][Weapon.DamageType.Piercing] = 0f;
        resistances[Soldier.Clasification.Striker][Weapon.DamageType.Poison] = 0f;
        resistances[Soldier.Clasification.Striker][Weapon.DamageType.Slashing] = 0f;
    }
}
