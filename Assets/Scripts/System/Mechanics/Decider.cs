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
        HostileActorsSighted = 5,
        HostileStructuresSighted = 6,
        Idle = 7,
        InCombat = 8,
        MovingToGoal = 9,
        PerformingTask = 10,
        ReachedGoal = 11,
        UnderAttack = 12,
        Watch = 13
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
        Me.Actions.Attack.EnemyAtMeleeOrRange();

        if (transform == null) {
            return;
        } else if (InCombat()) {
            SetState(State.InCombat);
        } else if (UnderAttack()) {
            SetState(State.UnderAttack);
        } else if (HostileActorsSighted()) {
            SetState(State.HostileActorsSighted);
        } else if (CallsForHelp()) {
            SetState(State.FriendsInNeed);
        }else if (DamagedFriendlyStructures()) {
            SetState(State.DamagedFriendlyStructuresSighted);
        } else if (HostileStructuresSighted()) {
            SetState(State.HostileStructuresSighted);
        } else if (Moving()) {
            SetState(State.MovingToGoal);
        } else if (ReachedGoal()) {
            SetState(State.ReachedGoal);
        } else if (Watching()) {
            SetState(State.Watch);
        } else {
            SetState(State.Idle);
        }
    }


    public List<Actor> IdentifyFriends()
    {
        FriendlyActorsSighted();
        return Friends;
    }


    public bool IsFriendOrNeutral(Actor _unit)
    {
        if (_unit == null || _unit == gameObject) return true;
        if (IsMyFaction(_unit)) return true;
        if (_unit.GetComponent<Fey>() != null) return true; // fey are neutral until individual units (e.g. Ents) attack (and get added to damagers)

        return false;  // if none of the above, it's probably the other faction and no exceptions apply
    }


    // private


    private bool CallsForHelp()
    {
        return FriendsInNeed.Count > 0;
    }


    private bool FriendlyActorsSighted()
    {
        Friends = Me.Senses.Actors
                    .Where(IsFriendOrNeutral)
                    .ToList();

        return Friends.Count > 0;
    }


    private bool DamagedFriendlyStructures()
    {
        FriendlyStructures = Me.Senses.Structures
                               .Where(structure => structure.owner == Me.Faction && structure.CurrentHitPoints < structure.maximum_hit_points)
                               .ToList();

        return FriendlyStructures.Count > 0;
    }


    private bool HostileActorsSighted()
    {
        Enemies = Me.Senses.Actors
                    .Where(actor => !IsFriendOrNeutral(actor))
                    .ToList();

        return Enemies.Count > 0;
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
        return Me.Actions.Movement != null && Me.Actions.Movement.InProgress();
    }


    private bool Watching()
    {
        // TODO: once a ruin is captured, switch to sentry and attack incoming enemies
        return false;
    }


    private bool ReachedGoal()
    {
        return Me.Actions.Movement != null && !Me.Actions.Movement.InProgress();
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
}
