using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{
    public enum Template
    {
        Base = 0,
        Commoner = 1,
        Gnoll = 2,
        Guard = 3
    };

    public static Dictionary<Template, float> darkvision_range = new Dictionary<Template, float>();
    public static Dictionary<Template, float> perception_range = new Dictionary<Template, float>();
    public static Dictionary<Template, List<Weapon>> available_weapons = new Dictionary<Template, List<Weapon>>();
    public static Dictionary<Template, Dictionary<Weapon.DamageType, int>> resistances = new Dictionary<Template, Dictionary<Weapon.DamageType, int>>();

    // properties

    public static Characters Instance { get; set; }
    public static string[] Templates { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one characters instance!");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // public


    public void GenerateStats()
    {
        BaseCharacterTemplate();
        CreatureTemplates();
    }


    // private


    private static void BaseCharacterTemplate()
    {
        darkvision_range[Template.Base] = 0f;
        perception_range[Template.Base] = 25f;
        resistances[Template.Base] = new Dictionary<Weapon.DamageType, int>
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

        // Gnoll
        darkvision_range[Template.Gnoll] = 10f;
        available_weapons[Template.Gnoll] = new List<Weapon>() { Weapons.Instance.longbow_prefab, Weapons.Instance.spear_prefab };

        //Guard
        available_weapons[Template.Guard] = new List<Weapon>() { Weapons.Instance.longbow_prefab, Weapons.Instance.spear_prefab };
        perception_range[Template.Guard] = 30f;
    }
}