using System.Linq;
using UnityEngine;

public class RayOfFrost : MonoBehaviour
{
    // properties

    public bool Advantage { get; set; }
    public int AttackModifier { get; set; }
    public bool Critical { get; set; }
    public int DamageModifier { get; set; }
    public Weapons.DamageType DamageType { get; set; }
    public int Die { get; set; }
    public bool Disadvantage { get; set; }
    public int Level { get; set; }
    public Actor Me { get; set; }
    public int NumberOfDice { get; set; }
    public float Range { get; set; }
    public Magic.School School { get; set; }
    public Proficiencies.Attribute SpellCastingAttribute { get; set; }
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
        SpellCastingAttribute = spell_casting_attribute;

        CheckAdvantageAndDisadvantage();
        DisplayEffects();

        if (Hit()) {
            ApplyDamage();
        }
    }


    // private


    private void ApplyDamage()
    {
        int damage_roll = Me.Actions.RollDie(Die, NumberOfDice);
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


    private void DisplayEffects()
    {
        GameObject beam = new GameObject();
        beam.transform.position = Me.GetInteractionPoint(Target);
        beam.AddComponent<LineRenderer>();
        LineRenderer lr = beam.GetComponent<LineRenderer>();
        lr.material = Me.GetComponent<Interactable>().highlight_material; // TODO: create ray material
        lr.startColor = Color.blue;
        lr.endColor = Color.blue;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, Me.weapon_transform.position);
        lr.SetPosition(1, Target.GetInteractionPoint(Me));
        Destroy(beam, 1f);
    }


    private bool Hit()
    {
        Actor target_actor = Target.GetComponent<Actor>();
        Structure target_structure = Target.GetComponent<Structure>();
        Critical = false;

        int roll = Me.Actions.RollDie(20, 1, Advantage, Disadvantage);
        
        if (roll >= Me.Actions.Combat.CriticalRangeStart) Critical = true;

        if (target_actor != null) {
            return roll + AttackModifier > target_actor.Actions.Stats.GetArmorClass();
        } else if (target_structure != null) {
            return roll + AttackModifier > target_structure.armor_class;
        }

        return false;
    }


    private void SetComponents()
    {
        DamageType = Weapons.DamageType.Radiant;
        Die = 8;
        Level = 0;
        Me = GetComponentInParent<Actor>();
        Range = 15f;
        School = Magic.School.Evocation;
        switch (Me.Stats.Level) {
            case int n when (n > 0 && n < 5):
                NumberOfDice = 1;
                break;
            case int n when (n >= 5 && n < 11):
                NumberOfDice = 2;
                break;
            case int n when (n >= 11 && n < 17):
                NumberOfDice = 3;
                break;
            case int n when (n >= 17):
                NumberOfDice = 4;
                break;
        }
    }

    private void SetModifiers()
    {
        AttackModifier = Me.Stats.ProficiencyBonus + Me.Stats.GetAdjustedAttributeScore(SpellCastingAttribute);
        DamageModifier = 0; // but may eventually include class feature bonuses
    }
}
