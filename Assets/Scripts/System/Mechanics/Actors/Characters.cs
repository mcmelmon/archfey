using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{

    // Inspector settings
    public GameObject player_prefab;


    public enum Template
    {
        Base,
        Commoner,
        Gnoll,
        Guard,
        Player
    };

    public static Dictionary<Template, float> darkvision_range = new Dictionary<Template, float>();
    public static Dictionary<Template, float> perception_range = new Dictionary<Template, float>();
    public static Dictionary<Template, List<Weapon>> available_weapons = new Dictionary<Template, List<Weapon>>();
    public static Dictionary<Template, Dictionary<Weapons.DamageType, int>> resistances = new Dictionary<Template, Dictionary<Weapons.DamageType, int>>();

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
        GenerateStats();
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
        perception_range[Template.Base] = 20f;
        resistances[Template.Base] = new Dictionary<Weapons.DamageType, int>
        {
            [Weapons.DamageType.Acid] = 0,
            [Weapons.DamageType.Bludgeoning] = 0,
            [Weapons.DamageType.Cold] = 0,
            [Weapons.DamageType.Fire] = 0,
            [Weapons.DamageType.Force] = 0,
            [Weapons.DamageType.Lightning] = 0,
            [Weapons.DamageType.Necrotic] = 0,
            [Weapons.DamageType.Piercing] = 0,
            [Weapons.DamageType.Poison] = 0,
            [Weapons.DamageType.Psychic] = 0,
            [Weapons.DamageType.Radiant] = 0,
            [Weapons.DamageType.Slashing] = 0,
            [Weapons.DamageType.Thunder] = 0
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

        // Player
        darkvision_range[Template.Player] = 10f;
        available_weapons[Template.Player] = new List<Weapon>() { Weapons.Instance.longbow_prefab, Weapons.Instance.spear_prefab };
    }
}