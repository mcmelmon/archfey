using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Decider : MonoBehaviour
{
    public enum State
    {
        BadlyInjured = 1,
        DamagedFriendlyStructuresSighted = 2,
        FriendsInNeed = 3,
        FriendlyActorsSighted = 4,
        FullLoad = 5,
        Harvesting = 6,
        HostileActorsSighted = 7,
        HostileStructuresSighted = 8,
        Idle = 9,
        InCombat = 10,
        Crafting = 12,
        MovingToGoal = 13,
        ReachedGoal = 14,
        UnderAttack = 15,
        Watch = 16
    };
    
    // Inspector settings
    
    public State state;
    public State previous_state;

    // properties

    public List<Actor> Enemies { get; set; }
    public List<Actor> Friends { get; set; }
    public List<Actor> FriendsInNeed { get; set; }
    public List<Structure> FriendlyStructures { get; set; }
    public List<Structure> HostileStructures { get; set; }
    public Actor Me { get; set; }
    public Threat Threat { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }


    // public


    public void ChooseState()
    {
        Me.Senses.Sight();
        Enemies = Me.Senses.Actors.Where(a => !IsFriendOrNeutral(a)).ToList();
        Friends = Me.Senses.Actors.Where(IsFriendOrNeutral).ToList();

        Me.Actions.Attack.SetEnemyRanges();

        if (transform == null) {
            return;
        } else if (BadlyInjured()) {
            SetState(State.BadlyInjured);
        } else if (InCombat()) {
            SetState(State.InCombat);
        } else if (UnderAttack()) {
            SetState(State.UnderAttack);
        } else if (CallsForHelp()) {
            SetState(State.FriendsInNeed);
        } else if (HostileActorsSighted()) {
            SetState(State.HostileActorsSighted);
        } else if (DamagedFriendlyStructures()) {
            SetState(State.DamagedFriendlyStructuresSighted);
        } else if (HostileStructuresSighted()) {
            SetState(State.HostileStructuresSighted);
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
        Friends = Me.Senses.Actors.Where(IsFriendOrNeutral).ToList();

        return Friends;
    }


    public List<Actor> IdentifyEnemies()
    {
        Enemies = Me.Senses.Actors.Where(a => !IsFriendOrNeutral(a)).ToList();

        return Enemies;
    }


    public bool IsFriendOrNeutral(Actor _unit)
    {
        if (_unit == null || _unit == gameObject) return true;
        if (IsMyFaction(_unit)) return true;
        if (_unit.GetComponent<Fey>() != null) return true; // fey are neutral until individual units (e.g. Ents) attack (and get added to damagers)

        return false;  // if none of the above, it's probably the other faction and no exceptions apply
    }


    // private


    private bool BadlyInjured() => Me.Health.BadlyInjured();


    private bool CallsForHelp()
    {
        return FriendsInNeed.Count > 0;
    }


    private bool Crafting()
    {
        return Industry.Crafters.Contains(Me);
    }


    private bool DamagedFriendlyStructures()
    {
        FriendlyStructures = Me.Senses.Structures
                               .Where(structure => structure.owner == Me.Faction && structure.CurrentHitPoints < structure.maximum_hit_points)
                               .ToList();

        return FriendlyStructures.Count > 0;
    }


    private bool FullLoad()
    {
        if (!Proficiencies.Instance.Harvester(Me)) return false;

        foreach (var pair in Me.Load) {  // there "should" only be at most one pair at any given time
            return pair.Value >= pair.Key.full_harvest;
        }

        return false;
    }


    private bool Harvesting()
    {
        return Proficiencies.Instance.Harvester(Me) && !FullLoad() && Me.harvesting != "";
    }


    private bool HostileActorsSighted()
    {
        return previous_state != State.FriendsInNeed && Enemies.Count > 0;
    }


    private bool HostileStructuresSighted()
    {
        HostileStructures = Me.Senses.Structures
                              .Where(structure => structure.owner != Me.Faction && structure.CurrentHitPoints > 0)
                              .ToList();

        return HostileStructures.Count > 0;
    }


    private bool InCombat()
    {
        Actor foe = null;

        if (Me.Actions.Attack.Engaged()) {
            foe = Threat.BiggestThreat();
            if (foe != null) {
                if (!Enemies.Contains(foe)) Enemies.Add(foe);
            }
        }
        return foe != null;
    }


    private bool IsAThreat(Actor _unit)
    {
        return Threat.IsAThreat(_unit);
    }


    private bool IsAttackingMyFaction(Actor _unit)
    {
        return Me.Faction != Conflict.Faction.Fey && Me.Faction != Conflict.Faction.None
                                  && (Me.Faction == Conflict.Faction.Ghaddim) ? (Me.Ghaddim.IsFactionThreat(_unit)) : (Me.Mhoddim.IsFactionThreat(_unit));
    }


    private bool IsMyFaction(Actor _unit)
    {
        return _unit != null && Me.Faction == _unit.Faction;
    }


    private bool IsMyRole(Actor _unit)
    {
        return _unit != null && Me.Role == _unit.Role;
    }


    private bool Moving()
    {
        return Me.Actions.Movement.InProgress();
    }


    private bool ReachedGoal()
    {
        return (previous_state == State.MovingToGoal || previous_state == State.FullLoad || previous_state == State.Idle) && !Me.Actions.Movement.InProgress();
    }


    private void SetComponents()
    {
        FriendsInNeed = new List<Actor>();
        Enemies = new List<Actor>();
        Friends = new List<Actor>();
        Me = GetComponentInParent<Actor>();
        HostileStructures = new List<Structure>();
        Threat = GetComponent<Threat>();
        state = State.Idle;
    }


    private void SetState(State _state)
    {
        previous_state = state;
        state = _state;
    }


    private bool UnderAttack()
    {
        if (Threat.Threats.Count > 0) {
            Actor foe = Threat.BiggestThreat();
            if (foe != null) {
                if (!Enemies.Contains(foe)) Enemies.Add(foe);

            }
            return true;
        }

        return false;
    }


    private bool Watching()
    {
        // TODO: once a ruin is captured, switch to sentry and attack incoming enemies
        return false;
    }
}
