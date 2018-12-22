﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConfigureGhaddim 
{
    // Versatile attributes
    public static Dictionary<Soldier.Clasification, int> agility_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> strength_rating = new Dictionary<Soldier.Clasification, int>();

    // Defense attributes
    public static Dictionary<Soldier.Clasification, int> armor_rating = new Dictionary<Soldier.Clasification, int>();
    public static Dictionary<Soldier.Clasification, int> corporeal_rating = new Dictionary<Soldier.Clasification, int>();
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
        agility_rating[Soldier.Clasification.Heavy] = 1;
        agility_rating[Soldier.Clasification.Scout] = 3;
        agility_rating[Soldier.Clasification.Striker] = 2;

        strength_rating[Soldier.Clasification.Heavy] = 3;
        strength_rating[Soldier.Clasification.Scout] = 0;
        strength_rating[Soldier.Clasification.Striker] = 1;

        // defense
        armor_rating[Soldier.Clasification.Heavy] = 0;
        corporeal_rating[Soldier.Clasification.Heavy] = 0;
        counter[Soldier.Clasification.Heavy] = 10;
        force_rating[Soldier.Clasification.Heavy] = 0;

        armor_rating[Soldier.Clasification.Scout] = 0;
        corporeal_rating[Soldier.Clasification.Scout] = 0;
        counter[Soldier.Clasification.Scout] = 5;
        force_rating[Soldier.Clasification.Scout] = 0;

        armor_rating[Soldier.Clasification.Striker] = 0;
        corporeal_rating[Soldier.Clasification.Striker] = 0;
        counter[Soldier.Clasification.Striker] = 5;
        force_rating[Soldier.Clasification.Striker] = 0;

        // health
        starting_health[Soldier.Clasification.Heavy] = 130;
        starting_health[Soldier.Clasification.Scout] = 70;
        starting_health[Soldier.Clasification.Striker] = 100;

        recovery_amount[Soldier.Clasification.Heavy] = 3;
        recovery_amount[Soldier.Clasification.Scout] = 1;
        recovery_amount[Soldier.Clasification.Striker] = 2;
    }


    private static void PopulateResistances()
    {
        resistances[Soldier.Clasification.Heavy] = new Dictionary<Weapon.DamageType, int>();
        resistances[Soldier.Clasification.Heavy][Weapon.DamageType.Arcane] = 0;
        resistances[Soldier.Clasification.Heavy][Weapon.DamageType.Blunt] = 15;
        resistances[Soldier.Clasification.Heavy][Weapon.DamageType.Elemental] = 0;
        resistances[Soldier.Clasification.Heavy][Weapon.DamageType.Piercing] = 10;
        resistances[Soldier.Clasification.Heavy][Weapon.DamageType.Poison] = 70;
        resistances[Soldier.Clasification.Heavy][Weapon.DamageType.Slashing] = 5;

        resistances[Soldier.Clasification.Scout] = new Dictionary<Weapon.DamageType, int>();
        resistances[Soldier.Clasification.Scout][Weapon.DamageType.Arcane] = 0;
        resistances[Soldier.Clasification.Scout][Weapon.DamageType.Blunt] = 10;
        resistances[Soldier.Clasification.Scout][Weapon.DamageType.Elemental] = 0;
        resistances[Soldier.Clasification.Scout][Weapon.DamageType.Piercing] = 10;
        resistances[Soldier.Clasification.Scout][Weapon.DamageType.Poison] = 70;
        resistances[Soldier.Clasification.Scout][Weapon.DamageType.Slashing] = 5;

        resistances[Soldier.Clasification.Striker] = new Dictionary<Weapon.DamageType, int>();
        resistances[Soldier.Clasification.Striker][Weapon.DamageType.Arcane] = 0;
        resistances[Soldier.Clasification.Striker][Weapon.DamageType.Blunt] = 0;
        resistances[Soldier.Clasification.Striker][Weapon.DamageType.Elemental] = 0;
        resistances[Soldier.Clasification.Striker][Weapon.DamageType.Piercing] = 15;
        resistances[Soldier.Clasification.Striker][Weapon.DamageType.Poison] = 70;
        resistances[Soldier.Clasification.Striker][Weapon.DamageType.Slashing] = 10;
    }
}
