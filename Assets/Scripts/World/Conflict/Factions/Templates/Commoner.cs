using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commoner : MonoBehaviour {

    // properties

    public Actor Actor { get; set; }


    // Unity


    private void Start()
    {
        SetStats();
    }


    // private


    private void SetStats()
    {
        // can't do in Actor until the Commoner component has been attached
        Actor = GetComponent<Actor>();
        Actor.Mhoddim.SetStats();
        Actor.Attack.EquipMeleeWeapon();
        Actor.Attack.EquipRangedWeapon();
    }
}
