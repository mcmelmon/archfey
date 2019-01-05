using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConfigureGhaddim 
{
    // TODO: refactor into single class

    // Primary attributes
    public static Dictionary<Soldier.Template, int> charisma_proficiency = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> constituion_proficiency = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> dexterity_proficiency = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> intelligence_proficiency = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> strength_proficiency = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> wisdom_proficiency = new Dictionary<Soldier.Template, int>();

    // Innate attributes
    public static Dictionary<Soldier.Template, int> current_mana = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, float> darkvision_range = new Dictionary<Soldier.Template, float>();
    public static Dictionary<Soldier.Template, bool> is_caster = new Dictionary<Soldier.Template, bool>();
    public static Dictionary<Soldier.Template, int> mana_pool_maximum = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, float> perception_range = new Dictionary<Soldier.Template, float>();
    public static Dictionary<Soldier.Template, float> speed = new Dictionary<Soldier.Template, float>();


    // Defense attributes
    public static Dictionary<Soldier.Template, int> armor_class = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, Dictionary<Weapon.DamageType, int>> resistances = new Dictionary<Soldier.Template, Dictionary<Weapon.DamageType, int>>();

    // Health attributes
    public static Dictionary<Soldier.Template, int> hit_dice = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> hit_dice_type = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> starting_health = new Dictionary<Soldier.Template, int>();

    // Offense attributes
    public static Dictionary<Soldier.Template, int> actions_per_round = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, int> objective_control_rating = new Dictionary<Soldier.Template, int>();
    public static Dictionary<Soldier.Template, List<Weapon>> available_weapons = new Dictionary<Soldier.Template, List<Weapon>>();


    public static void GenerateStats()
    {
        PopulateAttributes();
        PopulateResistances();
    }


    // private


    private static void PopulateAttributes()
    {
        // primary
        charisma_proficiency[Soldier.Template.Gnoll] = -2;
        constituion_proficiency[Soldier.Template.Gnoll] = 0;
        dexterity_proficiency[Soldier.Template.Gnoll] = 1;
        intelligence_proficiency[Soldier.Template.Gnoll] = -2;
        strength_proficiency[Soldier.Template.Gnoll] = 2;
        wisdom_proficiency[Soldier.Template.Gnoll] = 0;

        // innate
        current_mana[Soldier.Template.Gnoll] = 0;
        darkvision_range[Soldier.Template.Gnoll] = 10f;
        is_caster[Soldier.Template.Gnoll] = false;
        mana_pool_maximum[Soldier.Template.Gnoll] = 0;
        perception_range[Soldier.Template.Gnoll] = 20f;
        speed[Soldier.Template.Gnoll] = 1.5f;

        // defense
        armor_class[Soldier.Template.Gnoll] = 15;

        // health
        hit_dice[Soldier.Template.Gnoll] = 5;
        hit_dice_type[Soldier.Template.Gnoll] = 8;
        starting_health[Soldier.Template.Gnoll] = 22;

        // offense
        actions_per_round[Soldier.Template.Gnoll] = 1;
        objective_control_rating[Soldier.Template.Gnoll] = 5;

        List<Weapon> weapon_list = new List<Weapon>()
        {
            Weapons.Instance.longbow_prefab,
            Weapons.Instance.spear_prefab
        };
        available_weapons[Soldier.Template.Gnoll] = weapon_list;
    }


    private static void PopulateResistances()
    {
        resistances[Soldier.Template.Gnoll] = new Dictionary<Weapon.DamageType, int>
        {
            [Weapon.DamageType.Acid] = 0,
            [Weapon.DamageType.Bludgeoning] = 0,
            [Weapon.DamageType.Cold] = 0,
            [Weapon.DamageType.Fire] = 0,
            [Weapon.DamageType.Force] = 0,
            [Weapon.DamageType.Lightning] = 0,
            [Weapon.DamageType.Necrotic] = 0,
            [Weapon.DamageType.Piercing] = 0,
            [Weapon.DamageType.Poison] = 0,
            [Weapon.DamageType.Psychic] = 0,
            [Weapon.DamageType.Slashing] = 0,
            [Weapon.DamageType.Thunder] = 0
        };
    }
}
