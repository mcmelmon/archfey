using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decider : MonoBehaviour
{
    public enum State
    {
        AlliesUnderAttack = 1,
        BadlyInjured = 2,
        FriendliesSighted = 3,
        HostilesSighted = 4,
        Idle = 5,
        InCombat = 6,
        MovingToGoal = 7,
        PerformingTask = 8,
        ReachedGoal = 9,
        UnderAttack = 10,
        Watch = 11,
    };
    
    // Inspector settings
    
    public State state;
    public State previous_state;

    // properties

    public Actor Me { get; set; }
    public List<Actor> Enemies { get; set; }
    public List<Actor> Friends { get; set; }
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

        if (InCombat()) {
            Actor _enemy = Threat.BiggestThreat();
            if (_enemy != null) {
                if (!Enemies.Contains(_enemy)) Enemies.Add(_enemy);
                SetState(State.InCombat);
            } else {
                Me.Actions.Movement.Route = null;
                SetState(State.Idle);
            }
        } else if (UnderAttack()) {
            Actor _enemy = Threat.BiggestThreat();
            if (_enemy != null) {
                if (!Enemies.Contains(_enemy)) Enemies.Add(_enemy);
                previous_state = state;
                SetState(State.UnderAttack);
            } else {
                Me.Actions.Movement.Route = null;
                SetState(State.Idle);
            }
        } else if (AlliesUnderAttack()) {
            Actor _enemy = (Me.Faction == Conflict.Faction.Ghaddim) ? Me.Ghaddim.BiggestFactionThreat() : Me.Mhoddim.BiggestFactionThreat();

            if (_enemy != null) {
                if (!Enemies.Contains(_enemy)) Enemies.Add(_enemy);
                SetState(State.AlliesUnderAttack);
            } else {
                Me.Actions.Movement.Route = null;
                SetState(State.Idle);
            }
        } else if (HostilesSighted()) {
            SetState(State.HostilesSighted);
        } else if (ReachedGoal()) {
            SetState(State.ReachedGoal);
        } else if (Moving()) {
            SetState(State.MovingToGoal);
        } else if (Watching()) {
            SetState(State.Watch);
        } else {
            SetState(State.Idle);
        }
    }


    public bool IsFriendOrNeutral(Actor _unit)
    {
        if (_unit == null || _unit == gameObject) return true;
        if (IsMyRole(_unit)) return true;  // but don't automatically return false if not my role (I might be a scout, it might be fey)
        if (_unit.GetComponent<Fey>() != null) return true; // fey are neutral until individual units (e.g. Ents) attack (and get added to damagers)

        return false;  // if none of the above, it's probably the other faction and no exceptions apply
    }


    // private


    private bool AlliesUnderAttack()
    {
        for (int i = 0; i < Me.Senses.Sightings.Count; i++)
        {
            Actor sighting = Me.Senses.Sightings[i];
            if (sighting == null) continue;
            if (IsAttackingMyFaction(sighting)) return true;
        }

        return false;
    }


    private bool FriendliesSighted()
    {
        Friends.Clear();

        for (int i = 0; i < Me.Senses.Sightings.Count; i++)
        {
            Actor _friend = Me.Senses.Sightings[i];
            if (_friend == null) continue;
            if (_friend != null && IsMyFaction(_friend) && !Friends.Contains(_friend))
            {
                Friends.Add(_friend);
            }
        }

        return Friends.Count > 0;
    }


    private bool HostilesSighted()
    {
        Enemies.Clear();

        for (int i = 0; i < Me.Senses.Sightings.Count; i++)
        {
            Actor _hostile = Me.Senses.Sightings[i];
            if (_hostile == null) continue;
            if (!IsFriendOrNeutral(_hostile) && !Enemies.Contains(_hostile))
            {
                Enemies.Add(_hostile);
            }
        }

        return Enemies.Count > 0;
    }


    public List<Actor> IdentifyFriends()
    {
        FriendliesSighted();
        return Friends;
    }


    private bool InCombat()
    {
        return HostilesSighted() && Me.Actions.Attack.Engaged();
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
        Me = GetComponentInParent<Actor>();
        Enemies = new List<Actor>();
        Friends = new List<Actor>();
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
}
