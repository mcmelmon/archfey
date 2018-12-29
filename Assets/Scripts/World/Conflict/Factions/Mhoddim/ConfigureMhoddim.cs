using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConfigureMhoddim 
{
    // Primary attributes
    public static Dictionary<Soldier.Clasification, int> agility_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> constituion_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> intellect_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> strength_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> will_rating = new Dictionary<Soldier.Clasification, int>();

    // Defense attributes
    public static Dictionary<Soldier.Clasification, int> armor_rating = new Dictionary<Soldier.Clasification, int>();
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

        // primary
        agility_rating[Soldier.Clasification.Heavy] = 0;
        agility_rating[Soldier.Clasification.Scout] = 0;
        agility_rating[Soldier.Clasification.Striker] = 0;

        constituion_rating[Soldier.Clasification.Heavy] = 0;
        constituion_rating[Soldier.Clasification.Scout] = 0;
        constituion_rating[Soldier.Clasification.Striker] = 0;

        intellect_rating[Soldier.Clasification.Heavy] = 0;
        intellect_rating[Soldier.Clasification.Scout] = 0;
        intellect_rating[Soldier.Clasification.Striker] = 0;

        strength_rating[Soldier.Clasification.Heavy] = 1;
        strength_rating[Soldier.Clasification.Scout] = 0;
        strength_rating[Soldier.Clasification.Striker] = 2;

        will_rating[Soldier.Clasification.Heavy] = 3;
        will_rating[Soldier.Clasification.Scout] = 0;
        will_rating[Soldier.Clasification.Striker] = 0;

        // defense
        armor_rating[Soldier.Clasification.Heavy] = 0;
        force_rating[Soldier.Clasification.Heavy] = 0;

        armor_rating[Soldier.Clasification.Scout] = 0;
        force_rating[Soldier.Clasification.Scout] = 0;

        armor_rating[Soldier.Clasification.Striker] = 0;
        force_rating[Soldier.Clasification.Striker] = 0;

        // health
        starting_health[Soldier.Clasification.Heavy] = 60;
        starting_health[Soldier.Clasification.Scout] = 30;
        starting_health[Soldier.Clasification.Striker] = 40;

        recovery_amount[Soldier.Clasification.Heavy] = 0;
        recovery_amount[Soldier.Clasification.Scout] = 0;
        recovery_amount[Soldier.Clasification.Striker] = 0;
    }


    private static void PopulateResistances()
    {
        resistances[Soldier.Clasification.Heavy] = new Dictionary<Weapon.DamageType, int>
        {
            [Weapon.DamageType.Arcane] = 20,
            [Weapon.DamageType.Blunt] = 0,
            [Weapon.DamageType.Elemental] = 15,
            [Weapon.DamageType.Holy] = 15,
            [Weapon.DamageType.Piercing] = 0,
            [Weapon.DamageType.Poison] = 0,
            [Weapon.DamageType.Slashing] = 0
        };

        resistances[Soldier.Clasification.Scout] = new Dictionary<Weapon.DamageType, int>
        {
            [Weapon.DamageType.Arcane] = 20,
            [Weapon.DamageType.Blunt] = 0,
            [Weapon.DamageType.Elemental] = 15,
            [Weapon.DamageType.Holy] = 15,
            [Weapon.DamageType.Piercing] = 0,
            [Weapon.DamageType.Poison] = 0,
            [Weapon.DamageType.Slashing] = 0
        };

        resistances[Soldier.Clasification.Striker] = new Dictionary<Weapon.DamageType, int>
        {
            [Weapon.DamageType.Arcane] = 20,
            [Weapon.DamageType.Blunt] = 0,
            [Weapon.DamageType.Elemental] = 15,
            [Weapon.DamageType.Holy] = 15,
            [Weapon.DamageType.Piercing] = 0,
            [Weapon.DamageType.Poison] = 0,
            [Weapon.DamageType.Slashing] = 0
        };
    }
}
