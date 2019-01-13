using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decider : MonoBehaviour
{
    public enum State
    {
        AlliesUnderAttack = 1,
        BadlyInjured = 2,
        FriendlyActorsSighted = 3,
        FriendlyStructuresSighted = 4,
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

    public Actor Me { get; set; }
    public List<Actor> Enemies { get; set; }
    public List<Actor> Friends { get; set; }
    public List<Structure> Structures { get; set; }
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
        } else if (InCombat()) {
            SetState(State.InCombat);
        } else if (UnderAttack()) {
            SetState(State.UnderAttack);
        } else if (AlliesUnderAttack()) {
            SetState(State.AlliesUnderAttack);
        } else if (HostileActorsSighted()) {
            SetState(State.HostileActorsSighted);
        } else if (HostileStructuresSighted()) {
            SetState(State.HostileStructuresSighted);
        } else if (FriendlyStructuresSighted()) {
            SetState(State.FriendlyStructuresSighted);
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
        for (int i = 0; i < Me.Senses.Actors.Count; i++) {
            Actor sighting = Me.Senses.Actors[i];
            if (sighting == null) continue;
            if (IsAttackingMyFaction(sighting)) {
                Actor foe = (Me.Faction == Conflict.Faction.Ghaddim) ? Me.Ghaddim.BiggestFactionThreat() : Me.Mhoddim.BiggestFactionThreat();
                if (foe != null) {
                    if (!Enemies.Contains(foe)) Enemies.Add(foe);
                    return true;
                }
            }
        }

        return false;
    }


    private bool FriendlyActorsSighted()
    {
        Friends.Clear();

        for (int i = 0; i < Me.Senses.Actors.Count; i++)
        {
            Actor _friend = Me.Senses.Actors[i];
            if (_friend == null) continue;
            if (_friend != null && IsMyFaction(_friend) && !Friends.Contains(_friend))
            {
                Friends.Add(_friend);
            }
        }

        return Friends.Count > 0;
    }


    private bool FriendlyStructuresSighted()
    {
        Structures.Clear();

        for (int i = 0; i < Me.Senses.Structures.Count; i++)
        {
            Structure _structure = Me.Senses.Structures[i];
            if (_structure == null) continue;
            Structures.Add(_structure);
        }

        return Structures.Count > 0;
    }


    private bool HostileActorsSighted()
    {
        Enemies.Clear();

        for (int i = 0; i < Me.Senses.Actors.Count; i++)
        {
            Actor _hostile = Me.Senses.Actors[i];
            if (_hostile == null) continue;
            if (!IsFriendOrNeutral(_hostile) && !Enemies.Contains(_hostile))
            {
                Enemies.Add(_hostile);
            }
        }

        return Enemies.Count > 0;
    }


    private bool HostileStructuresSighted()
    {
        Structures.Clear();

        for (int i = 0; i < Me.Senses.Structures.Count; i++) {
            Structure _structure = Me.Senses.Structures[i];
            if (_structure == null || _structure.CurrentHitPoints <= 0) continue;
            Structures.Add(_structure);
        }

        return Structures.Count > 0;
    }


    public List<Actor> IdentifyFriends()
    {
        FriendlyActorsSighted();
        return Friends;
    }


    private bool InCombat()
    {
        Actor foe = null;

        if (HostileActorsSighted() && Me.Actions.Attack.Engaged()) {
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
        Me = GetComponentInParent<Actor>();
        Enemies = new List<Actor>();
        Friends = new List<Actor>();
        Structures = new List<Structure>();
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
