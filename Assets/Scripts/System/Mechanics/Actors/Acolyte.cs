using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Acolyte : MonoBehaviour
{
    // properties

    public Actor Me { get; set; }
    public CureWounds CureWounds { get; set; }
    public SacredFlame SacredFlame { get; set; }
    public Sanctuary Sanctuary { get; set; }
    public Spellcaster Spellcaster { get; set; }

    // Unity

    private void Start()
    {
        SetComponents();
    }

    // public

    public void OnBadlyInjured()
    {
        if (Spellcaster.HaveSpellSlot(Magic.Level.First)) {
            Spellcaster.UseSpellSlot(Magic.Level.First);
            CureWounds.Cast(Me);
        }
    }

    public void OnFriendsInNeed()
    {
        if (Me.Actions.Decider.FriendsInNeed.First() != null) {
            Me.Actions.Movement.SetDestination(Me.Actions.Decider.FriendsInNeed.First().transform);
        }
        AttackWithSpell();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }

    public void OnHostileActorsSighted()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.CloseWithEnemies();
        if (!CastSanctuary()) AttackWithSpell();
    }

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

    public void OnNeedsRest()
    {
        Me.Actions.SheathWeapon();
        Me.Actions.Movement.Home();
    }

    public void OnReachedGoal()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        OnIdle();
    }

    private void OnResting()
    {
        Me.Actions.SheathWeapon();

        if (Me.RestCounter == Actor.rested_at) {
            Me.Health.RecoverHealth(Me.Actions.RollDie(Me.Health.LargestHitDie(), 1));
            if (Me.Actions.Magic != null) Me.Actions.Magic.RecoverSpellSlots();
            Me.RestCounter = 0;
        } else {
            Me.RestCounter++;
        }
    }

    public void OnUnderAttack()
    {
        AttackWithSpell();
        Me.RestCounter = 0;
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
        if (Spellcaster.HaveSpellSlot(Magic.Level.First) && !Sanctuary.ProtectedTargets.ContainsKey(Me)) {
            Sanctuary.Cast(Me);
            return true;
        }

        return false;
    }

    private void SetActions()
    {
        Me.Actions.OnBadlyInjured = OnBadlyInjured;
        // Me.Actions.OnCrafting = OnCrafting;
        // Me.Actions.OnDamagedFriendlyStructuresSighted = OnDamagedFriendlyStructuresSighted;
        // Me.Actions.OnFriendlyActorsSighted = OnFriendlyActorsSighted;
        Me.Actions.OnFriendsInNeed = OnFriendsInNeed;
        // Me.Actions.OnFullLoad = OnFullLoad;
        // Me.Actions.OnHarvesting = OnHarvesting;
        // Me.Actions.OnHasObjective = OnHasObjective;
        Me.Actions.OnHostileActorsSighted = OnHostileActorsSighted;
        // Me.Actions.OnHostileStructuresSighted = OnHostileStructuresSighted;
        Me.Actions.OnIdle = OnIdle;
        Me.Actions.OnInCombat = OnInCombat;
        Me.Actions.OnMedic = OnMedic;
        // Me.Actions.OnMovingToGoal = OnMovingToGoal;
        Me.Actions.OnNeedsRest = OnNeedsRest;
        Me.Actions.OnReachedGoal = OnReachedGoal;
        Me.Actions.OnResting = OnResting;
        Me.Actions.OnUnderAttack = OnUnderAttack;
        // Me.Actions.OnWatch = OnWatch;
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



        Spellcaster = Me.gameObject.AddComponent<Spellcaster>();
        Spellcaster.CastingAttribute = Proficiencies.Attribute.Wisdom;
        Spellcaster.MaximumSpellSlots[Magic.Level.First] = 3;
        Spellcaster.SpellsLeft[Magic.Level.First] = 3;
        CureWounds = Me.Actions.Magic.gameObject.AddComponent<CureWounds>();
        SacredFlame = Me.Actions.Magic.gameObject.AddComponent<SacredFlame>();
        Sanctuary = Me.Actions.Magic.gameObject.AddComponent<Sanctuary>();
    }

    private bool TreatWounded()
    {
        var nearby_wounded = Me.Senses.Actors
                               .Where(f => Me.Actions.Decider.IsFriendOrNeutral(f) && f.Health.BadlyInjured() && Vector3.Distance(transform.position, f.transform.position) < Me.Actions.Movement.StoppingDistance())
                               .ToList();

        if (nearby_wounded.Count > 0) {
            Actor wounded = nearby_wounded.OrderBy(f => f.Health.CurrentHealthPercentage()).Reverse().ToList().First();

            if (wounded != null && Spellcaster.HaveSpellSlot(Magic.Level.First)) {
                Spellcaster.UseSpellSlot(Magic.Level.First);
                CureWounds.Cast(wounded);
                return true;
            }
        }

        return false;
    }
}