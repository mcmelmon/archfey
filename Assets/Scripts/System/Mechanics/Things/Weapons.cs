using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public enum WeaponName
    {
        Battleaxe,
        Blowgun,
        Club,
        Dagger,
        Dart,
        Flail,
        Glaive,
        Greataxe,
        Greatclub,
        Greatsword,
        Halberd,
        Hand_Crossbow,
        Handaxe,
        Heavy_Crossbow,
        Javelin,
        Lance,
        Light_Crossbow,
        Light_Hammer,
        Longbow,
        Longsword,
        Mace,
        Maul,
        Morningstar,
        Net,
        Pike,
        Quarterstaff,
        Rapier,
        Scimitar,
        Shortbow,
        Shortsword,
        Sickle,
        Sling,
        Spear,
        Trident,
        War_Pick,
        Warhammer,
        Whip
    };

    public enum DamageType {
        Acid,
        Bludgeoning,
        Cold,
        Fire,
        Force,
        Lightning,
        Necrotic,
        Piercing,
        Poison,
        Psychic,
        Radiant,
        Slashing,
        Thunder
    };

    public enum WeaponType { Simple_Melee, Simple_Ranged, Martial_Melee, Martial_Ranged };

    // Inspector settings

    public List<Weapon> weapons;


    // properties

    public static Weapons Instance { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one weapons instance");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // public


    public Weapon GetWeaponNamed(WeaponName name, string qualifer = "")
    {
        Weapon the_weapon = qualifer == ""
            ? weapons.First(weapon => weapon.weapon_name == name)
            : weapons.First(weapon => weapon.weapon_name == name && weapon.qualifier == qualifer);
        return the_weapon;
    }
}
