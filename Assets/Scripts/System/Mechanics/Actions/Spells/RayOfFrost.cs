using System.Collections;
using System.Linq;
using UnityEngine;

public class RayOfFrost : MonoBehaviour
{
    // properties

    public bool Advantage { get; set; }
    public int AttackModifier { get; set; }
    public GameObject Beam { get; set; }
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
            GameObject _impact = Instantiate(SpellEffects.Instance.physical_strike_prefab, Target.transform.position, SpellEffects.Instance.physical_strike_prefab.transform.rotation);
            _impact.name = "Impact";
            Destroy(_impact, 3f);
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
        if (Beam == null) {
            Beam = new GameObject();
            Beam.AddComponent<LineRenderer>();
            StartCoroutine(MoveBeam());
        }
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
            return roll + AttackModifier > target_structure.ArmorClass;
        }

        return false;
    }


    private IEnumerator MoveBeam()
    {
        int tick = 0;
        LineRenderer lr = Beam.GetComponent<LineRenderer>();
        lr.material = Me.GetComponent<Interactable>().highlight_material; // TODO: create ray material
        lr.startWidth = 0.2f;
        lr.endWidth = 0.2f;
        Destroy(Beam, 1f);

        while (Beam != null) {
            tick++;
            Vector3 start = Me == null || Target == null ? Vector3.zero : Me.GetComponent<Collider>().ClosestPointOnBounds(Target.transform.position);
            Vector3 stop = Target == null ? Vector3.zero : Target.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            if (start != Vector3.zero && stop != Vector3.zero) {
                lr.SetPosition(0, start);
                lr.SetPosition(1, stop);
            } else {
                break;
            }
            yield return null;
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
