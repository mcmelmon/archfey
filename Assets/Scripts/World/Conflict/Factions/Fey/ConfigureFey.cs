using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class ConfigureFey {

    // Primary attributes
    public static Dictionary<Soldier.Template, int> agility_rating = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> constitution_rating = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> intellect_rating = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> strength_rating = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> will_rating = new Dictionary<Soldier.Template, int>();

    // Defense attributes
    public static Dictionary<Soldier.Template, int> armor_rating = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> force_rating = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, Dictionary<Weapon.DamageType, int>> resistances = new Dictionary<Soldier.Template, Dictionary<Weapon.DamageType, int>>();

    // Health attributes
    public static Dictionary<Soldier.Template, int> recovery_amount = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> starting_health = new Dictionary<Soldier.Template, int>();


    // static


    public static void GenerateStats()
    {
        PopulateAttributes();
        PopulateResistances();
    }


    // private


    private static void PopulateAttributes()
    {

    }


    private static void PopulateResistances()
    {

    }
}
