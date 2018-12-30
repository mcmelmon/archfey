using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smite : MonoBehaviour
{
    // properties

    public Actor Actor { get; set; }
    public float Damage { get; set; }
    public int ManaCost { get; set; }
    public float Range { get; set; }
    public Resources Resources { get; set; }
    public Stats Stats { get; set; }
    public Actor Target { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }

    // public


    public void Cast(Actor _target, bool dispel_effect = false)
    {
        if (_target == null) return;
        Target = _target;

        if (Resources.CurrentMana >= ManaCost) {
            ApplyDamage();
            DisplayEffects();
            AdjustMana();
        }
    }


    // private


    private void AdjustMana()
    {
        Resources.DecreaseMana(ManaCost);
        Actor.Resources.UpdateStatBars();
    }


    private void ApplyDamage()
    {
        Damage = 3 * (Actor.Attack.EquippedMeleeWeapon.damage_maximum + Actor.Attack.AttackRating) * Resources.magic_potency;
        float damage_inflicted = Target.Defend.DamageAfterDefenses(Damage, Weapon.DamageType.Holy);
        Target.Health.LoseHealth(damage_inflicted, Actor);
    }


    private void DisplayEffects()
    {
        GameObject flare = Instantiate(SpellEffects.Instance.smite_prefab, Target.transform.position, Target.transform.rotation, Target.transform);
        flare.name = "Smite";
        flare.transform.position += new Vector3(0, 3, 0);
        Destroy(flare, 0.5f);
    }


    private void SetComponents()
    {
        Resources = GetComponent<Resources>();
        Actor = GetComponentInParent<Actor>();
        Stats = GetComponentInParent<Stats>();
        Damage = 0;
        ManaCost = 40;
        Range = 20f;
    }
}
