using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{

    public enum Template {
        Base = 0,
        Commoner = 1,
        Gnoll = 2,
        Guard = 3
    };


    public static Dictionary<Characters.Template, int> charisma_proficiency = new Dictionary<Characters.Template, int>();
    public static Dictionary<Characters.Template, int> constituion_proficiency = new Dictionary<Characters.Template, int>();
    public static Dictionary<Characters.Template, int> dexterity_proficiency = new Dictionary<Characters.Template, int>();
    public static Dictionary<Characters.Template, int> intelligence_proficiency = new Dictionary<Characters.Template, int>();
    public static Dictionary<Characters.Template, int> strength_proficiency = new Dictionary<Characters.Template, int>();
    public static Dictionary<Characters.Template, int> wisdom_proficiency = new Dictionary<Characters.Template, int>();

    public static Dictionary<Characters.Template, int> actions_per_round = new Dictionary<Characters.Template, int>();
    public static Dictionary<Characters.Template, int> armor_class = new Dictionary<Characters.Template, int>();
    public static Dictionary<Characters.Template, float> darkvision_range = new Dictionary<Characters.Template, float>();
    public static Dictionary<Characters.Template, int> hit_dice = new Dictionary<Characters.Template, int>();
    public static Dictionary<Characters.Template, int> hit_dice_type = new Dictionary<Characters.Template, int>();
    public static Dictionary<Characters.Template, float> perception_range = new Dictionary<Characters.Template, float>();
    public static Dictionary<Characters.Template, float> speed = new Dictionary<Characters.Template, float>();
    public static Dictionary<Characters.Template, int> starting_hit_dice = new Dictionary<Characters.Template, int>();


    public static Dictionary<Characters.Template, List<Weapon>> available_weapons = new Dictionary<Characters.Template, List<Weapon>>();
    public static Dictionary<Characters.Template, int> objective_control_rating = new Dictionary<Characters.Template, int>();
    public static Dictionary<Characters.Template, Dictionary<Weapon.DamageType, int>> resistances = new Dictionary<Characters.Template, Dictionary<Weapon.DamageType, int>>();


    public static void GenerateStats()
    {
        BaseCharacterTemplate();
        CreatureTemplates();
    }


    // private


    private static void BaseCharacterTemplate()
    {
        charisma_proficiency[Characters.Template.Base] = 0;
        constituion_proficiency[Characters.Template.Base] = 0;
        dexterity_proficiency[Characters.Template.Base] = 0;
        intelligence_proficiency[Characters.Template.Base] = 0;
        strength_proficiency[Characters.Template.Base] = 0;
        wisdom_proficiency[Characters.Template.Base] = 0;

        actions_per_round[Characters.Template.Base] = 1;
        armor_class[Characters.Template.Base] = 10;
        darkvision_range[Characters.Template.Base] = 0f;
        hit_dice[Characters.Template.Base] = 1;
        hit_dice_type[Characters.Template.Base] = 8;
        perception_range[Characters.Template.Base] = 20f;
        speed[Characters.Template.Base] = 1.5f;

        resistances[Characters.Template.Base] = new Dictionary<Weapon.DamageType, int>
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


    private static void CreatureTemplates()
    {
        // Commoner
        available_weapons[Template.Commoner] = new List<Weapon>() { Weapons.Instance.club_prefab };
        objective_control_rating[Template.Commoner] = 10;

        // Gnoll
        charisma_proficiency[Template.Gnoll] = -2;
        dexterity_proficiency[Template.Gnoll] = 1;
        intelligence_proficiency[Template.Gnoll] = -2;
        strength_proficiency[Template.Gnoll] = 2;
        darkvision_range[Template.Gnoll] = 10f;
        armor_class[Template.Gnoll] = 15;
        hit_dice[Template.Gnoll] = 5;
        hit_dice_type[Template.Gnoll] = 8;
        available_weapons[Template.Gnoll] = new List<Weapon>() { Weapons.Instance.longbow_prefab, Weapons.Instance.spear_prefab };
        objective_control_rating[Template.Gnoll] = 5;

        //Guard
        constituion_proficiency[Template.Guard] = 1;
        dexterity_proficiency[Template.Guard] = 1;
        strength_proficiency[Template.Guard] = 1;
        armor_class[Template.Guard] = 16;
        available_weapons[Template.Guard] = new List<Weapon>() { Weapons.Instance.longbow_prefab, Weapons.Instance.spear_prefab };
        hit_dice[Characters.Template.Guard] = 2;
        objective_control_rating[Template.Guard] = 5;
        perception_range[Characters.Template.Guard] = 25f;

    }
}
