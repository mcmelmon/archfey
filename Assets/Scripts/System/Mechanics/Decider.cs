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
        HostileActorsSighted,
        HostileStructuresSighted,
        Idle,
        InCombat,
        Crafting,
        Medic,
        MovingToGoal,
        ReachedGoal,
        UnderAttack,
        Watch
    };
    
    // Inspector settings
    
    public State state;
    public State previous_state;

    // properties

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
        Me.Actions.Attack.SetEnemyRanges();

        if (transform == null) {
            return;
        } else if (BadlyInjured()) {
            SetState(State.BadlyInjured);
        } else if (Medic()) {
            SetState(State.Medic);
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
        return Me.Senses.Actors.Where(IsFriendOrNeutral).ToList();
    }


    public List<Actor> IdentifyEnemies()
    {
        return Me.Senses.Actors.Where(a => !IsFriendOrNeutral(a)).ToList();
    }


    public bool IsFriendOrNeutral(Actor _unit)
    {
        if (_unit == null || _unit == gameObject) return true;
        if (IsMyFaction(_unit)) return true;
        if (_unit.GetComponent<Fey>() != null) return true; // fey are neutral until individual units (e.g. Ents) attack (and get added to damagers)

        return false;  // if none of the above, it's probably the other faction and no exceptions apply
    }


    // private


    private bool BadlyInjured(){
        return Me.Health.BadlyInjured();
    }


    private bool CallsForHelp()
    {
        return FriendsInNeed.Count > 0;
    }


    private bool Crafting()
    {
        return Industry.CurrentlyCrafting.Contains(Me);
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
        return previous_state != State.FriendsInNeed && Me.Senses.Actors.Where(a => !IsFriendOrNeutral(a)).ToList().Count > 0;
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
        // TODO: actually attack the chosen foe; currently, Attack just chooses an available target

        Actor foe = null;

        if (Me.Actions.Attack.Engaged()) foe = Threat.PrimaryThreat();
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


    private bool Medic()
    {
        return Me.Senses.Actors.Where(IsFriendOrNeutral).ToList().Where(friend => friend.Health.BadlyInjured() == true).ToList().Count > 0;
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
        return Threat.Threats.Count > 0;
    }


    private bool Watching()
    {
        // TODO: once a ruin is captured, switch to sentry and attack incoming enemies
        return false;
    }
}
