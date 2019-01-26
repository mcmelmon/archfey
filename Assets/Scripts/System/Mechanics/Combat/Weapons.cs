using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public enum DamageType
    {
        Acid = 0,
        Bludgeoning = 1,
        Cold = 2,
        Fire = 3,
        Force = 4,
        Lightning = 5,
        Necrotic = 6,
        Piercing = 7,
        Poison = 8,
        Psychic = 9,
        Radiant = 10,
        Slashing = 11,
        Thunder = 12
    };

    // Inspector settings

    public Weapon club_prefab;
    public Weapon longbow_prefab;
    public Weapon spear_prefab;

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
}
