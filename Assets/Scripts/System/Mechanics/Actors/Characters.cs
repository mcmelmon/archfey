using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{
    // TODO: Obsolete, delete

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
        BaseCharacterTemplate();
    }

    // private

    private static void BaseCharacterTemplate()
    {
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
}