using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultRange : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public int AttackModifier { get; set; }
    public float Damage { get; set; }
    public int DamageModifier { get; set; }
    public GameObject Projectile { get; set; }
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
        Weapon = Me.Actions.Attack.EquippedRangedWeapon;
        Weapon.gameObject.SetActive(true);
        SetModifiers();
        Projectile = Instantiate(Weapon.projectile_prefab, transform.Find("AttackOrigin").transform.position, transform.rotation);
        StartCoroutine(Seek());

        if (Hit()) {
            ApplyDamage();
            DisplayEffects(_target.transform.position);
        }
    }


    // private


    private void ApplyDamage()
    {
        if (Target.Health != null && Target.Actions.Defend != null && Me.Actions != null) {
            int damage_roll = Random.Range(0, Weapon.damage_die) + 1;
            Damage = Target.Actions.Defend.DamageAfterDefenses(damage_roll + DamageModifier, Weapon.damage_type);
            Target.Health.LoseHealth(Damage, Me);
        }
    }


    public void DisplayEffects(Vector3 _location)
    {
        GameObject _impact = Instantiate(SpellEffects.Instance.physical_strike_prefab, _location + new Vector3(1, 4f, 0), SpellEffects.Instance.physical_strike_prefab.transform.rotation);
        _impact.transform.parent = transform;
        _impact.name = "Impact";
        Destroy(_impact, 1f);
    }


    public bool Hit()
    {
        Actor target_actor = Target.GetComponent<Actor>();
        Structure target_structure = Target.GetComponent<Structure>();

        if (target_actor != null) {
            return Random.Range(1, 21) + AttackModifier > target_actor.Actions.Defend.ArmorClass;
        } else if (target_structure != null) {
            return Random.Range(1, 21) + AttackModifier > target_structure.armor_class;
        }

        return false;
    }


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
    }


    private void SetModifiers()
    {
        AttackModifier = Me.Stats.DexterityProficiency + Weapon.attack_bonus + Actions.SuperiorWeapons[Weapon.damage_type];
        DamageModifier = Me.Stats.DexterityProficiency + Weapon.damage_bonus + Actions.SuperiorWeapons[Weapon.damage_type];

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
