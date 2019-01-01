using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
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
