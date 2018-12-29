using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smite : MonoBehaviour
{
    // properties

    public Actor Actor { get; set; }
    public float Damage { get; set; }
    public int ManaCost { get; set; }
    public Resources Resources { get; set; }


    // public


    public void Cast(Actor _target, bool dispel_effect = false)
    {
        SetComponents();

        if (Resources.CurrentMana >= ManaCost) {

            Resources.CurrentMana -= ManaCost;
            Actor.Resources.UpdateStatBars();

            Damage = Actor.Attack.EquippedMeleeWeapon.damage_maximum * Resources.magic_potency;

            GameObject flare = Instantiate(SpellEffects.Instance.smite_prefab, _target.transform.position, _target.transform.rotation, _target.transform);
            flare.name = "Smite";
            flare.transform.position += new Vector3(0, 3, 0);
            Destroy(flare, 0.5f);

            float damage_inflicted = _target.Defend.DamageAfterDefenses(Damage, Weapon.DamageType.Holy);
            _target.Health.LoseHealth(damage_inflicted, Actor);
        }
    }


    // private


    private void SetComponents()
    {
        Resources = GetComponent<Resources>();
        Actor = GetComponentInParent<Actor>();
        Damage = 0;
        ManaCost = 35;
    }
}
