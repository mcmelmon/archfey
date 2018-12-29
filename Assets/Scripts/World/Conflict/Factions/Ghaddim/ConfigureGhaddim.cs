using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConfigureGhaddim 
{
    // Versatile attributes
    public static Dictionary<Soldier.Clasification, int> agility_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> intellect_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> strength_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> will_rating = new Dictionary<Soldier.Clasification, int>();

    // Defense attributes
    public static Dictionary<Soldier.Clasification, int> armor_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> constitution_rating = new Dictionary<Soldier.Clasification, int>();
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
        agility_rating[Soldier.Clasification.Heavy] = 0;
        agility_rating[Soldier.Clasification.Scout] = 0;
        agility_rating[Soldier.Clasification.Striker] = 0;

        intellect_rating[Soldier.Clasification.Heavy] = 0;
        intellect_rating[Soldier.Clasification.Scout] = 0;
        intellect_rating[Soldier.Clasification.Striker] = 0;

        strength_rating[Soldier.Clasification.Heavy] = 0;
        strength_rating[Soldier.Clasification.Scout] = 0;
        strength_rating[Soldier.Clasification.Striker] = 0;

        will_rating[Soldier.Clasification.Heavy] = 0;
        will_rating[Soldier.Clasification.Scout] = 0;
        will_rating[Soldier.Clasification.Striker] = 0;

        // defense
        armor_rating[Soldier.Clasification.Heavy] = 0;
        constitution_rating[Soldier.Clasification.Heavy] = 0;
        force_rating[Soldier.Clasification.Heavy] = 0;

        armor_rating[Soldier.Clasification.Scout] = 0;
        constitution_rating[Soldier.Clasification.Scout] = 0;
        force_rating[Soldier.Clasification.Scout] = 0;

        armor_rating[Soldier.Clasification.Striker] = 0;
        constitution_rating[Soldier.Clasification.Striker] = 0;
        force_rating[Soldier.Clasification.Striker] = 0;

        // health
        starting_health[Soldier.Clasification.Heavy] = 70;
        starting_health[Soldier.Clasification.Scout] = 40;
        starting_health[Soldier.Clasification.Striker] = 50;

        recovery_amount[Soldier.Clasification.Heavy] = 2;
        recovery_amount[Soldier.Clasification.Scout] = 1;
        recovery_amount[Soldier.Clasification.Striker] = 1;
    }


    private static void PopulateResistances()
    {
        resistances[Soldier.Clasification.Heavy] = new Dictionary<Weapon.DamageType, int>
        {
            [Weapon.DamageType.Arcane] = 0,
            [Weapon.DamageType.Blunt] = 10,
            [Weapon.DamageType.Elemental] = 0,
            [Weapon.DamageType.Holy] = -25,
            [Weapon.DamageType.Piercing] = 20,
            [Weapon.DamageType.Poison] = 100,
            [Weapon.DamageType.Slashing] = 10
        };

        resistances[Soldier.Clasification.Scout] = new Dictionary<Weapon.DamageType, int>
        {
            [Weapon.DamageType.Arcane] = 0,
            [Weapon.DamageType.Blunt] = 10,
            [Weapon.DamageType.Elemental] = 0,
            [Weapon.DamageType.Holy] = -25,
            [Weapon.DamageType.Piercing] = 20,
            [Weapon.DamageType.Poison] = 100,
            [Weapon.DamageType.Slashing] = 10
        };

        resistances[Soldier.Clasification.Striker] = new Dictionary<Weapon.DamageType, int>
        {
            [Weapon.DamageType.Arcane] = 0,
            [Weapon.DamageType.Blunt] = 10,
            [Weapon.DamageType.Elemental] = 0,
            [Weapon.DamageType.Holy] = -25,
            [Weapon.DamageType.Piercing] = 20,
            [Weapon.DamageType.Poison] = 100,
            [Weapon.DamageType.Slashing] = 10
        };
    }
}
