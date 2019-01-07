using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultRange : MonoBehaviour
{
    // properties

    public Actor Actor { get; set; }
    public Attack Attack { get; set; }
    public int AttackModifier { get; set; }
    public float Damage { get; set; }
    public int DamageModifier { get; set; }
    public GameObject Projectile { get; set; }
    public Resources Resources { get; set; }
    public Actor Target { get; set; }
    public Weapon Weapon { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }


    // public


    public void Strike(Actor _target) {
        // TODO: allow ranged attacks against structure

        if (_target == null) return;
        Target = _target;
        Weapon = Attack.EquippedRangedWeapon;
        Weapon.gameObject.SetActive(true);
        SetModifiers();
        Projectile = Instantiate(Weapon.projectile_prefab, transform.Find("AttackOrigin").transform.position, transform.rotation);
        StartCoroutine(Seek());

        if (Random.Range(1, 21) + AttackModifier > Target.Actions.Defend.ArmorClass) { // Dexterity is already built in to AC
            ApplyDamage();
            DisplayEffects();
        }
    }


    // private


    private void ApplyDamage()
    {
        if (Target.Health != null && Target.Actions.Defend != null && Actor.Actions != null) {
            int damage_roll = Random.Range(0, Weapon.damage_die) + 1;
            Damage = Target.Actions.Defend.DamageAfterDefenses(damage_roll + DamageModifier, Weapon.damage_type);
            Target.Health.LoseHealth(Damage, Actor);
        }
    }


    public void DisplayEffects()
    {
        GameObject _impact = Instantiate(SpellEffects.Instance.physical_strike_prefab, Target.transform.position + new Vector3(1, 4f, 0), SpellEffects.Instance.physical_strike_prefab.transform.rotation);
        _impact.transform.parent = Target.transform;
        _impact.name = "Impact";
        Destroy(_impact, 1f);
    }


    private void SetComponents()
    {
        Actor = GetComponentInParent<Actor>();
        Attack = GetComponent<Attack>();
        Resources = GetComponent<Resources>();
    }


    private void SetModifiers()
    {
        AttackModifier = Actor.Stats.DexterityProficiency + Weapon.attack_bonus + Actions.SuperiorWeapons[Weapon.damage_type];
        DamageModifier = Actor.Stats.DexterityProficiency + Weapon.damage_bonus + Actions.SuperiorWeapons[Weapon.damage_type];

    }


    private IEnumerator Seek()
    {
        while (true) {
            if (Target == null && Projectile != null) {
                Destroy(Projectile);
                yield return null;
            } else if (Projectile != null) {
                float separation = float.MaxValue;
                Vector3 direction = Target.transform.position - transform.position;
                float distance = 10f * Time.deltaTime;

                Projectile.transform.position += distance * direction;
                separation = Vector3.Distance(Target.transform.position, Projectile.transform.position);

                if (separation <= .5f) Destroy(Projectile);
            }

            yield return null;
        }
    }
}
