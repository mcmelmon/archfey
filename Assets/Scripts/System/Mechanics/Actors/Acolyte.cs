using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Acolyte : MonoBehaviour, IAct
{
    // properties

    public Actor Me { get; set; }
    public CureWounds CureWounds { get; set; }
    public SacredFlame SacredFlame { get; set; }
    public Sanctuary Sanctuary { get; set; }


    // Unity


    private void Start()
    {
        SetComponents();
    }


    // public


    public void OnBadlyInjured()
    {
        if (Me.Magic.HaveSpellSlot(Magic.Level.First)) {
            Me.Magic.UseSpellSlot(Magic.Level.First);
            CureWounds.Cast(Me);
        }
    }


    public void OnCrafting() { }


    public void OnDamagedFriendlyStructuresSighted() { }


    public void OnFriendlyActorsSighted() { }


    public void OnFriendsInNeed()
    {
        if (Me.Actions.Decider.FriendsInNeed.First() != null) {
            Me.Actions.Movement.SetDestination(Me.Actions.Decider.FriendsInNeed.First().transform);
        }
        AttackWithSpell();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public void OnFullLoad() { }


    public void OnHarvesting() { }


    public void OnHasObjective() { }


    public void OnHostileActorsSighted()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.CloseWithEnemies();
        if (!CastSanctuary()) AttackWithSpell();
    }


    public void OnHostileStructuresSighted() { }


    public void OnInCombat()
    {
        AttackWithSpell();
    }


    public void OnIdle()
    {
        Me.Actions.SheathWeapon();
        Me.Actions.Movement.Home();
    }


    public void OnMedic()
    {
        Actor wounded = Me.Senses.Actors
                          .Where(friend => Me.Actions.Decider.IsFriendOrNeutral(friend) && friend.Health.BadlyInjured() == true)
                          .ToList()
                          .First();

        Me.Actions.Movement.SetDestination(wounded.transform);
        StartCoroutine(Me.Actions.Movement.TrackUnit(wounded));

        if (!TreatWounded()) AttackWithSpell();
    }


    public void OnMovingToGoal() { }


    public void OnNeedsRest()
    {
        Me.Actions.SheathWeapon();
        Me.Actions.Movement.Home();
    }


    public void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public void OnUnderAttack()
    {
        AttackWithSpell();
        Me.RestCounter = 0;
    }


    public void OnWatch()
    {
        // call for help after running away
    }


    // private


    private void AttackWithSpell()
    {
        Actor target = Me.Actions.Decider.Threat.PrimaryThreat();
        
        if (target != null && Vector3.Distance(target.transform.position, transform.position) < SacredFlame.Range) {
            SacredFlame.Cast(target);
        }
    }


    private bool CastSanctuary()
    {
        if (Me.Magic.HaveSpellSlot(Magic.Level.First) && !Sanctuary.ProtectedTargets.ContainsKey(Me)) {
            Sanctuary.Cast(Me);
            return true;
        }

        return false;
    }


    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        SetAdditionalStats();
    }


    private void SetAdditionalStats()
    {
        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.None));
        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Club));
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];

        CureWounds = gameObject.AddComponent<CureWounds>();
        SacredFlame = gameObject.AddComponent<SacredFlame>();
        Sanctuary = gameObject.AddComponent<Sanctuary>();

        Me.Magic = gameObject.AddComponent<Magic>();
        Me.Magic.MaximumSpellSlots[Magic.Level.First] = 3;
        Me.Magic.SpellsLeft[Magic.Level.First] = 3;

        Me.Stats.Skills.Add(Proficiencies.Skill.Medicine);
        Me.Stats.ExpertiseInSkills.Add(Proficiencies.Skill.Medicine);
        Me.Stats.Skills.Add(Proficiencies.Skill.Religion);
    }


    private bool TreatWounded()
    {
        var nearby_wounded = Me.Senses.Actors
                               .Where(f => Me.Actions.Decider.IsFriendOrNeutral(f) && f.Health.BadlyInjured() && Vector3.Distance(transform.position, f.transform.position) < Me.Actions.Movement.ReachedThreshold)
                               .ToList();

        if (nearby_wounded.Count > 0) {
            Actor wounded = nearby_wounded.OrderBy(f => f.Health.CurrentHealthPercentage()).Reverse().ToList().First();

            if (wounded != null && Me.Magic.HaveSpellSlot(Magic.Level.First)) {
                Me.Magic.UseSpellSlot(Magic.Level.First);
                CureWounds.Cast(wounded);
                return true;
            }
        }

        return false;
    }
}