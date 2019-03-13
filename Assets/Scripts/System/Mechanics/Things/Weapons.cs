using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public enum DamageType
    {
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

    // Inspector settings

    public List<GameObject> weapons;


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


    public Weapon GetWeaponNamed(string name)
    {
        return weapons.First(weapon => weapon.GetComponent<Weapon>().weapon_name == name).GetComponent<Weapon>();
    }
}
