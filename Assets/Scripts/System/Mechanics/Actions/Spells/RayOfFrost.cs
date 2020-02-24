using System.Collections;
using System.Linq;
using UnityEngine;

public class RayOfFrost : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public bool Advantage { get; set; }
    public Weapons.DamageType DamageType { get; set; }
    public int Die { get; set; }
    public bool Disadvantage { get; set; }
    public int Level { get; set; }
    public float Range { get; set; }
    public Magic.School School { get; set; }
    public Spellcaster Spellcaster { get; set; }
    // public Proficiencies.Attribute SpellCastingAttribute { get; set; }
    public Actor Target { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }

    // public

    // TODO: specify spell_casting_attribute for other spells
    public void Cast(Actor _target, Proficiencies.Attribute spell_casting_attribute = Proficiencies.Attribute.Intelligence, bool dispel_effect = false)
    {
        if (_target == null) return;
        Target = _target;

        CheckAdvantageAndDisadvantage();
        DrawRay();

        if (Hit()) {
            ApplyDamage();
            GameObject _impact = Instantiate(SpellEffects.Instance.physical_strike_prefab, Target.transform.position, SpellEffects.Instance.physical_strike_prefab.transform.rotation);
            _impact.name = "Impact";
            Destroy(_impact, 3f);
        }
    }

    // private

    private void ApplyDamage()
    {
        int damage_roll = Me.Actions.RollDie(Die, NumberOfDice());
        int damage_inflicted = Target.Actions.Stats.DamageAfterDefenses(damage_roll, DamageType);
        Target.Health.LoseHealth(damage_inflicted, Me);
    }

    // TODO: refactor advantage/disadvantage - though it can be 
    private void CheckAdvantageAndDisadvantage()
    {
        var friends_in_melee = Me.Senses.Actors
                                 .Where(friend => friend != null && Me.Actions.Decider.IsFriendOrNeutral(friend) && Vector3.Distance(transform.position, friend.transform.position) < 2f)
                                 .ToList();

        if (Me.Actions.Decider.AvailableMeleeTargets.Count > 0) {
            Disadvantage = true;
        }

        Advantage |= friends_in_melee.Count > Me.Actions.Decider.AvailableMeleeTargets.Count;
    }

    private void DrawRay()
    {
        Me.Actions.Magic.DrawRay(Me.weapon_transform.position, Target.transform.position);
    }

    private bool Hit()
    {
        Actor target_actor = Target.GetComponent<Actor>();

        int roll = Me.Actions.RollDie(20, 1, Advantage, Disadvantage);

        if (target_actor != null) {
            return  roll + Spellcaster.AttackModifier() > target_actor.Actions.Stats.GetArmorClass();
        }

        return false;
    }

    private int NumberOfDice()
    {
        switch (Me.GetComponent<Spellcaster>().Level) {  // TODO: pick the Spellcaster for this spell by casting attribute
            case int n when (n > 0 && n < 5):
                return 1;
            case int n when (n >= 5 && n < 11):
                return 2;
            case int n when (n >= 11 && n < 17):
                return 3;
            case int n when (n >= 17):
                return 4;
            default:
                return 1;
        }
    }

    private void SetComponents()
    {
        DamageType = Weapons.DamageType.Radiant;
        Die = 8;
        Level = 0;
        Me = GetComponentInParent<Actor>();
        Range = 15f;
        School = Magic.School.Evocation;
        Spellcaster = Me.GetComponent<Spellcaster>();
    }
}
