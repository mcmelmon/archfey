using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gnoll : MonoBehaviour
{
    // properties

    public Actor Actor { get; set; }
    public float PerceptionRange { get; set; }
    public int PerceptionRating { get; set; }
    public float Speed { get; set; }

    // Unity


    private void Start()
    {
        SetComponents();
        SetStats();
    }


    // private


    private void SetComponents()
    {
        PerceptionRange = 10f;
        PerceptionRating = 0;
        Speed = 1.5f;

        Actor = GetComponent<Actor>();
        Actor.Movement.Agent.speed = Speed;
        Actor.ObjectiveControlRating = 5;
        Actor.Resources.CurrentMana = 0;
        Actor.Resources.IsCaster = false;
        Actor.Resources.mana_pool_maximum = 0;
        Actor.Senses.Darkvision = 10f;
        Actor.Senses.PerceptionRating = PerceptionRating;
        Actor.Senses.SetRange(PerceptionRange);
        Actor.SuperiorWeapons = new Dictionary<Weapon.DamageType, int>
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


    private void SetStats()
    {
        // can't do in Actor until the Heavy component has been attached

        Actor.Ghaddim.SetStats();  // Gnolls are Ghaddim
    }
}
