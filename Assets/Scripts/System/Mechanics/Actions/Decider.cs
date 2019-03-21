﻿using System.Collections;
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

    public bool AttackAtRange { get; set; }
    public List<GameObject> AvailableMeleeTargets { get; set; }
    public List<GameObject> AvailableRangedTargets { get; set; }
    public List<Actor> Enemies { get; set; }
    public List<Actor> Friends { get; set; }
    public List<Actor> FriendsInNeed { get; set; }
    public List<Structure> FriendlyStructures { get; set; }
    public List<Structure> HostileStructures { get; set; }
    public Actor Me { get; set; }
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

        if (Enemies.Any()) SetEnemyRanges();
        return Enemies;
    }


    public bool IsFriendOrNeutral(Actor other_unit, bool only_friends = false)
    {
        if (other_unit == null || other_unit == Me) return true;

        if (Threat.Threats.ContainsKey(other_unit)) return false;

        bool faction_hostile = Me.Faction.IsHostileTo(other_unit.Faction);

        return only_friends ? other_unit.Faction == Me.Faction : !faction_hostile;
    }


    public GameObject TargetEnemy()
    {
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
        FriendlyStructures = Me.Senses.Structures
                               .Where(structure => structure.Faction == Me.Faction && structure.CurrentHitPoints < structure.maximum_hit_points)
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


    private bool Harvesting()
    {
        return Proficiencies.Instance.IsHarvester(Me) && !FullLoad();
    }


    private bool HostileActorsSighted()
    {
        return Enemies.Count > 0;  // InCombat comes earlier and calls IdentifyEnemy
    }


    private bool HostileStructuresSighted()
    {
        HostileStructures = Me.Senses.Structures
                              .Where(structure => Me.Faction.IsHostileTo(structure.Faction) && structure.CurrentHitPoints > 0)
                              .ToList();

        return HostileStructures.Count > 0;
    }


    private bool InCombat()
    {
        IdentifyEnemies();
        return Enemies.Any() && Me.Actions.Combat.Engaged;
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
        return Me.Magic != null && Me.Magic.HaveSpellSlot(Magic.Level.First) && IdentifyFriends().Any(friend => friend.Health.BadlyInjured());
    }


    private bool Moving()
    {
        return Me.Actions.Movement.InProgress();
    }


    private bool NeedsRest()
    {
        bool spent_spell_slots = Me.Magic != null && Me.Magic.UsedSlot;
        return !Me.Actions.Combat.Engaged && !HostileActorsSighted() && (Me.Health.CurrentHitPoints < Me.Health.MaximumHitPoints || spent_spell_slots );
    }


    private bool ReachedGoal()
    {
        return (previous_state == State.MovingToGoal || previous_state == State.FullLoad || previous_state == State.Idle) && !Me.Actions.Movement.InProgress();
    }


    private bool Resting()
    {
        return (previous_state == State.NeedsRest || previous_state == State.Resting) && NeedsRest() && !Me.Actions.Movement.InProgress();
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
        Me = GetComponentInParent<Actor>();
        HostileStructures = new List<Structure>();
        Target = null;
        Threat = GetComponent<Threat>();
        state = State.Idle;
    }


    private void SetEnemyRanges()
    {
        if (Me == null) return;

        float melee_range = Me.Actions.Combat.MeleeRange();
        Weapon ranged_weapon = Me.Actions.Combat.EquippedRangedWeapon;

        if (Enemies.Any()) {
            AvailableMeleeTargets.AddRange(Enemies
                                           .Where(actor => actor != null && Me.SeparationFrom(actor) <= melee_range)
                                           .OrderBy(actor => actor.Health.CurrentHitPoints)
                                           .Select(actor => actor.gameObject)
                                           .Distinct()
                                           .ToList());

            if (ranged_weapon != null) {
                AvailableRangedTargets.AddRange(Enemies
                                                .Where(actor => actor != null && Me.SeparationFrom(actor) <= ranged_weapon.Range)
                                                .OrderBy(actor => actor.Health.CurrentHitPoints)
                                                .Select(actor => actor.gameObject)
                                                .Distinct()
                                                .ToList());
            }
        }

        if (Me.Actions.Decider.HostileStructures.Any())
        {
            AvailableMeleeTargets.AddRange(Me.Actions.Decider.HostileStructures
                                           .Where(structure => Vector3.Distance(transform.position, structure.GetInteractionPoint(Me)) <= melee_range + Me.Actions.Movement.ReachedThreshold)
                                           .Select(structure => structure.gameObject)
                                           .Distinct()
                                           .ToList());

            if (ranged_weapon != null) {
                AvailableRangedTargets.AddRange(Me.Actions.Decider.HostileStructures
                                                .Where(structure => Vector3.Distance(transform.position, structure.GetInteractionPoint(Me)) <= ranged_weapon.Range + Me.Actions.Movement.ReachedThreshold)
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
        return Threat.Threats.Count > 0;
    }


    private bool Watching()
    {
        // TODO: once a ruin is captured, switch to sentry and attack incoming enemies
        return false;
    }
}
