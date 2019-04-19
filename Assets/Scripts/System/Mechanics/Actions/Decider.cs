using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Decider : MonoBehaviour
{
    public enum State
    {
        None,
        BadlyInjured,
        DamagedFriendlyStructuresSighted,
        FriendsInNeed,
        FriendlyActorsSighted,
        FullLoad,
        Harvesting,
        HasObjective,
        HostileActorsSighted,
        HostileStructuresSighted,
        Idle,
        InCombat,
        Crafting,
        Medic,
        MovingToGoal,
        NeedsRest,
        ReachedGoal,
        Resting,
        UnderAttack,
        Watch
    };
    
    // Inspector settings
    
    public State state;
    public State previous_state;

    // properties

    public bool AchievedAllObjectives { get; set; }
    public bool AttackAtRange { get; set; }
    public List<GameObject> AvailableMeleeTargets { get; set; }
    public List<GameObject> AvailableRangedTargets { get; set; }
    public List<Actor> Enemies { get; set; }
    public List<Actor> Friends { get; set; }
    public List<Actor> FriendsInNeed { get; set; }
    public List<Structure> FriendlyStructures { get; set; }
    public ClaimNode Goal { get; set; }
    public List<Structure> HostileStructures { get; set; }
    public Actor Me { get; set; }
    public List<Objective> Objectives { get; set; }
    public GameObject Target { get; set; }
    public Threat Threat { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }


    // public


    public void ChooseState()
    {
        if (transform == null) {
            return;
        } else if (Medic()) {
            SetState(State.Medic);
        } else if (Resting()) {
            SetState(State.Resting);
        } else if (NeedsRest()) {
            SetState(State.NeedsRest);
        } else if (BadlyInjured()) {
            SetState(State.BadlyInjured);
        } else if (InCombat()) {
            SetState(State.InCombat);
        } else if (UnderAttack()) {
            SetState(State.UnderAttack);
        } else if (HostileActorsSighted()) {
            SetState(State.HostileActorsSighted);
        } else if (CallsForHelp()) {
            SetState(State.FriendsInNeed);
        } else if (HostileStructuresSighted()) {
            SetState(State.HostileStructuresSighted);
        } else if (DamagedFriendlyStructures()) {
            SetState(State.DamagedFriendlyStructuresSighted);
        } else if (Crafting()) {
            SetState(State.Crafting);
        } else if (ReachedGoal()) {
            SetState(State.ReachedGoal);
        } else if (Moving()) {
            SetState(State.MovingToGoal);
        } else if (HasObjective()) {
            SetState(State.HasObjective);
        } else if (FullLoad()) {
            SetState(State.FullLoad);
        } else if (Harvesting()) {
            SetState(State.Harvesting);
        } else if (Watching()) {
            SetState(State.Watch);
        } else {
            SetState(State.Idle);
        }
    }


    public List<Actor> IdentifyFriends()
    {
        Friends = Me.Senses.Actors.Where(actor => IsFriendOrNeutral(actor, true)).ToList();
        return Friends;
    }


    public List<Actor> IdentifyEnemies()
    {
        // needs rest checks for enemies, triggering this very early in decision tree

        ClearTargets();
        Enemies = Me.Senses.Actors.Where(actor => !IsFriendOrNeutral(actor)).ToList();

        for (int i = 0; i < Enemies.Count; i++) {
            if (Enemies[i].Actions.Stealth.IsPerforming) {
                int performance_challenge_rating = Enemies[i].Actions.SkillCheck(true, Proficiencies.Skill.Performance);
                int my_insight_check = Me.Actions.SkillCheck(true, Proficiencies.Skill.Insight);
                if (my_insight_check < performance_challenge_rating) {
                    Debug.Log(Me.name + " failed an insight check with " + my_insight_check + " vs " + performance_challenge_rating);
                    Enemies.Remove(Enemies[i]);
                } else {
                    Debug.Log(Me.name + " succeeded an insight check with " + my_insight_check + " vs " + performance_challenge_rating);
                }
            }
        }

        if (Enemies.Any()) {
            SetEnemyRanges();
        } else {
            Me.Actions.Combat.Engaged = false;
        }
        return Enemies;
    }


    public bool IsFriendOrNeutral(Actor other_unit, bool only_friends = false)
    {
        if (other_unit == null || other_unit == Me) return true;

        if (Threat.Threats.ContainsKey(other_unit)) return false;

        bool faction_hostile = Me.CurrentFaction.IsHostileTo(other_unit.CurrentFaction);

        return only_friends ? other_unit.CurrentFaction == Me.CurrentFaction : !faction_hostile;
    }


    public GameObject TargetEnemy()
    {
        IdentifyEnemies();
        RemoveSanctuaryTargets();

        if (AvailableMeleeTargets.Any() && AvailableMeleeTargets.Contains(Threat.PrimaryThreat()?.gameObject)) {
            TargetMelee();
        } else if (AvailableRangedTargets.Any() && AvailableRangedTargets.Contains(Threat.PrimaryThreat()?.gameObject)) {
            TargetRanged();
        } else {
            Target = null;
        }

        return Target;
    }


    public GameObject TargetMelee(GameObject player_target = null)
    {
        Target = player_target ?? Threat.PrimaryThreat().gameObject ?? AvailableMeleeTargets[0];

        if (Target != null) {
            transform.LookAt(Target.transform);
            AttackAtRange = false;
        }

        return Target;
    }


    public GameObject TargetRanged(GameObject player_target = null)
    {
        Target = player_target ?? Threat.PrimaryThreat().gameObject ?? AvailableRangedTargets[0];

        if (Target != null) {
            transform.LookAt(Target.transform);
            AttackAtRange = true;
        }

        return Target;
    }


    // private


    private bool BadlyInjured(){
        return Me.Health.BadlyInjured();
    }


    private bool CallsForHelp()
    {
        return FriendsInNeed.Count > 0;
    }


    private void ClearTargets()
    {
        AvailableMeleeTargets.Clear();
        AvailableRangedTargets.Clear();
        Target = null;
    }


    private bool Crafting()
    {
        return Industry.CurrentlyCrafting.Contains(Me);
    }


    private bool DamagedFriendlyStructures()
    {
        // TODO: we only want units that repair to see this

        FriendlyStructures = Me.Senses.Structures
                               .Where(structure => structure.Faction == Me.CurrentFaction && structure.CurrentHitPoints < structure.maximum_hit_points)
                               .ToList();

        return FriendlyStructures.Count > 0;
    }


    private bool FullLoad()
    {
        if (!Proficiencies.Instance.IsHarvester(Me)) return false;

        foreach (var pair in Me.Load) {  // there "should" only be at most one pair at any given time
            return pair.Value >= pair.Key.full_harvest;
        }

        return false;
    }


    private bool GiveUpChase()
    {
        // TODO: objectives may need to be added to routes

        Vector3 home = Me.Actions.Movement.Destinations.ContainsKey(Movement.CommonDestination.Home) ? Me.Actions.Movement.Destinations[Movement.CommonDestination.Home] : Vector3.zero;
        Vector3 closest_stop = Me.Route.WorldStops.Any() ? Me.Route.WorldStops.OrderBy(stop => Vector3.Distance(transform.position, stop)).First() : Vector3.zero;
        float smaller_separation = Mathf.Min(Vector3.Distance(home, transform.position), Vector3.Distance(closest_stop, transform.position));

        return smaller_separation > 40f; // if we've strayed too far, forget about them
    }


    private bool Harvesting()
    {
        return Proficiencies.Instance.IsHarvester(Me) && !FullLoad();
    }


    private bool HasObjective()
    {
        return !AchievedAllObjectives && Goal != null && previous_state != State.ReachedGoal;
    }


    private bool HostileActorsSighted()
    {
        IdentifyEnemies();
        return HasObjective() ? Enemies.Count > 0 : !GiveUpChase() && Enemies.Count > 0;
    }


    private bool HostileStructuresSighted()
    {
        HostileStructures = Me.Senses.Structures
                              .Where(structure => Me.CurrentFaction.IsHostileTo(structure.Faction) && structure.CurrentHitPoints > 0)
                              .ToList();

        return HostileStructures.Count > 0;
    }


    private bool InCombat()
    {
        return HasObjective() ? Enemies.Any() && Me.Actions.Combat.Engaged : !GiveUpChase() && Enemies.Any() && Me.Actions.Combat.Engaged;
    }


    private bool IsAThreat(Actor unit)
    {
        return Threat.IsAThreat(unit);
    }


    private bool IsMyAlignment(Actor unit)
    {
        return unit != null && Me.Alignment == unit.Alignment;
    }


    private bool Medic()
    {
        bool can_cast = Me.Magic != null && Me.Magic.HaveSpellSlot(Magic.Level.First);
        bool wounded_friend = IdentifyFriends().Any(friend => friend.Health.BadlyInjured());

        return can_cast && wounded_friend;
    }


    private bool Moving()
    {
        return Me.Actions.Movement.InProgress() && previous_state != State.ReachedGoal;
    }


    private bool NeedsRest()
    {
        bool enemies_abound = HostileActorsSighted();
        bool spent_spell_slots = Me.Magic != null && Me.Magic.UsedSlot;
        bool injured = Me.Health.CurrentHitPoints < Me.Health.MaximumHitPoints;
        return !enemies_abound && (injured || spent_spell_slots);
    }


    private bool ReachedGoal()
    {
        if (Goal != null) {
            float separation = Vector3.Distance(transform.position, Goal.transform.position);
            return previous_state != State.ReachedGoal && separation < Goal.influence_zone_radius;
        } 

        return (previous_state == State.MovingToGoal || previous_state == State.FullLoad || previous_state == State.Idle) && !Me.Actions.Movement.InProgress();
    }


    private bool Resting()
    {
        return (previous_state == State.NeedsRest || previous_state == State.Resting) && NeedsRest() && !Moving();
    }


    private void RemoveSanctuaryTargets()
    {
        if (Sanctuary.ProtectedTargets == null) return;

        List<GameObject> protected_melee_actors = AvailableMeleeTargets
            .Where(target => target.GetComponent<Actor>() != null && Sanctuary.ProtectedTargets.ContainsKey(target.GetComponent<Actor>()) && !Me.Actions.Decider.Threat.Threats.ContainsKey(target.GetComponent<Actor>()))
            .ToList();

        List<GameObject> protected_ranged_actors = AvailableRangedTargets
            .Where(target => target.GetComponent<Actor>() != null && Sanctuary.ProtectedTargets.ContainsKey(target.GetComponent<Actor>()) && !Me.Actions.Decider.Threat.Threats.ContainsKey(target.GetComponent<Actor>()))
            .ToList();

        foreach (var target in protected_melee_actors) {
            if (!Me.Actions.SavingThrow(Proficiencies.Attribute.Wisdom, Sanctuary.ProtectedTargets[target.GetComponent<Actor>()].ChallengeRating)) {
                AvailableMeleeTargets.Remove(target);
            }
        }

        foreach (var target in protected_ranged_actors) {
            if (!Me.Actions.SavingThrow(Proficiencies.Attribute.Wisdom, Sanctuary.ProtectedTargets[target.GetComponent<Actor>()].ChallengeRating)) {
                AvailableMeleeTargets.Remove(target);
            }
        }
    }


    private void SetComponents()
    {
        AttackAtRange = false;
        AvailableMeleeTargets = new List<GameObject>();
        AvailableRangedTargets = new List<GameObject>();
        Enemies = new List<Actor>();
        Friends = new List<Actor>();
        FriendsInNeed = new List<Actor>();
        Goal = null;
        HostileStructures = new List<Structure>();
        Me = GetComponentInParent<Actor>();
        Objectives = new List<Objective>();
        Target = null;
        Threat = GetComponent<Threat>();
        state = State.Idle;
    }


    private void SetEnemyRanges()
    {
        if (Me == null) return;

        float melee_range = Me.Actions.Combat.MeleeRange();
        Weapon ranged_weapon = Me.Actions.Combat.EquippedRangedWeapon;
        Weapon combat_spell = Me.Actions.Combat.CombatSpells.FirstOrDefault(); // TODO: cycle through spells and choose the longest ranged.

        if (Enemies.Any()) {
            AvailableMeleeTargets.AddRange(Enemies
                                           .Where(actor => actor != null && Me.SeparationFrom(actor.transform) <= melee_range)
                                           .OrderBy(actor => actor.Health.CurrentHitPoints)
                                           .Select(actor => actor.gameObject)
                                           .Distinct()
                                           .ToList());

            if (ranged_weapon != null) {
                AvailableRangedTargets.AddRange(Enemies
                                                .Where(actor => actor != null && Me.SeparationFrom(actor.transform) <= ranged_weapon.Range)
                                                .OrderBy(actor => actor.Health.CurrentHitPoints)
                                                .Select(actor => actor.gameObject)
                                                .Distinct()
                                                .ToList());
            }

            if (combat_spell != null) {
                AvailableRangedTargets.AddRange(Enemies
                                                .Where(actor => actor != null && Me.SeparationFrom(actor.transform) <= combat_spell.GetComponent<Spell>().range)
                                                .OrderBy(actor => actor.Health.CurrentHitPoints)
                                                .Select(actor => actor.gameObject)
                                                .Distinct()
                                                .ToList());
            }
        }

        if (Me.Actions.Decider.HostileStructures.Any())
        {
            AvailableMeleeTargets.AddRange(Me.Actions.Decider.HostileStructures
                                           .Where(structure => Me.SeparationFrom(structure.transform) <= melee_range + Me.Actions.Movement.ReachedThreshold)
                                           .Select(structure => structure.gameObject)
                                           .Distinct()
                                           .ToList());

            if (ranged_weapon != null) {
                AvailableRangedTargets.AddRange(Me.Actions.Decider.HostileStructures
                                                .Where(structure => Me.SeparationFrom(structure.transform) <= ranged_weapon.Range + Me.Actions.Movement.ReachedThreshold)
                                                .Select(structure => structure.gameObject)
                                                .Distinct()
                                                .ToList());
            }

            if (combat_spell != null) {
                AvailableRangedTargets.AddRange(Me.Actions.Decider.HostileStructures
                                                .Where(structure => Me.SeparationFrom(structure.transform) <= combat_spell.GetComponent<Spell>().range + Me.Actions.Movement.ReachedThreshold)
                                                .Select(structure => structure.gameObject)
                                                .Distinct()
                                                .ToList());
            }
        }
    }


    private void SetState(State _state)
    {
        previous_state = state;
        state = _state;
    }


    private bool UnderAttack()
    {
        return HasObjective() ? Threat.Threats.Count > 0 : !GiveUpChase() && Threat.Threats.Count > 0;
    }


    private bool Watching()
    {
        // TODO: once a ruin is captured, switch to sentry and attack incoming enemies
        return false;
    }
}
