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
        HasObjective = 4,
        HostilesSighted = 5,
        Idle = 6,
        InCombat = 7,
        OnWatch = 8,
        ReachedObective = 9,
        UnderAttack = 10,
    };

    public State state;

    // properties

    public Actor Actor { get; set; }
    public List<Actor> Enemies { get; set; }
    public List<Actor> Friends { get; set; }
    public Threat Threat { get; set; }


    // Unity

    private void Awake()
    {
        Actor = GetComponent<Actor>();
        Enemies = new List<Actor>();
        Friends = new List<Actor>();
        Threat = gameObject.AddComponent<Threat>();
        state = State.Idle;
    }


    public void FinishedRoute()
    {
        Actor.Movement.Route = null;
        Actor.Movement.ResetPath();
    }


    public bool IsFriendOrNeutral(Actor _unit)
    {
        if (_unit == null || _unit == gameObject) return true;
        if (IsMyRole(_unit)) return true;  // but don't automatically return false if not my role (I might be a scout, it might be fey)
        if (_unit.GetComponent<Fey>() != null) return true; // fey are neutral until individual units (e.g. Ents) attack (and get added to damagers)

        return false;  // if none of the above, it's probably the other faction and no exceptions apply
    }


    public void SetState()
    {
        Actor.Senses.Sight();

        if (InCombat())
        {
            Actor _enemy = Threat.BiggestThreat();
            if (_enemy != null)
            {
                if (!Enemies.Contains(_enemy)) Enemies.Add(_enemy);
                state = State.InCombat;
            }
            else
            {
                state = State.Idle;
            }
            return;
        }
        else if (UnderAttack())
        {
            Actor _enemy = Threat.BiggestThreat();
            if (_enemy != null)
            {
                if (!Enemies.Contains(_enemy)) Enemies.Add(_enemy);
                state = State.UnderAttack;
            }
            else
            {
                state = State.Idle;
            }
            return;
        }
        else if (AlliesUnderAttack())
        {
            Actor _enemy = (Actor.Faction == Conflict.Faction.Ghaddim) ? Actor.Ghaddim.BiggestFactionThreat() : Actor.Mhoddim.BiggestFactionThreat();

            if (_enemy != null)
            {
                if (!Enemies.Contains(_enemy)) Enemies.Add(_enemy);
                state = State.AlliesUnderAttack;
            }
            else
            {
                state = State.Idle;
            }
            return;
        }
        else if (HostilesSighted())
        {
            state = State.HostilesSighted;
            return;
        }
        else if (HasObjective())
        {
            state = State.HasObjective;
            return;
        }
        else if (OnWatch())
        {
            state = State.OnWatch;
            return;
        }
        else
        {
            state = State.Idle;
            return;
        }
    }


    // private


    private bool AlliesUnderAttack()
    {
        for (int i = 0; i < Actor.Senses.Sightings.Count; i++)
        {
            Actor sighting = Actor.Senses.Sightings[i];
            if (sighting == null) continue;
            if (IsAttackingMyFaction(sighting)) return true;
        }

        return false;
    }


    private bool FriendliesSighted()
    {
        Friends.Clear();

        for (int i = 0; i < Actor.Senses.Sightings.Count; i++)
        {
            Actor _friend = Actor.Senses.Sightings[i];
            if (_friend == null) continue;
            if (_friend != null && IsMyFaction(_friend) && !Friends.Contains(_friend))
            {
                Friends.Add(_friend);
            }
        }

        return Friends.Count > 0;
    }


    private bool HasObjective()
    {
        return Actor.Movement != null && Actor.Movement.Route != null && !Actor.Movement.Route.Completed();
    }


    private bool HostilesSighted()
    {
        Enemies.Clear();

        for (int i = 0; i < Actor.Senses.Sightings.Count; i++)
        {
            Actor _hostile = Actor.Senses.Sightings[i];
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
        return HostilesSighted() && Actor.Attack.Engaged();
    }


    private bool IsAThreat(Actor _unit)
    {
        return Threat.IsAThreat(_unit);
    }


    private bool IsAttackingMyFaction(Actor _unit)
    {
        return Actor.Faction != Conflict.Faction.Fey && Actor.Faction != Conflict.Faction.None
                                  && (Actor.Faction == Conflict.Faction.Ghaddim) ? (Actor.Ghaddim.IsFactionThreat(_unit)) : (Actor.Mhoddim.IsFactionThreat(_unit));
    }


    private bool IsMyFaction(Actor _unit)
    {
        return _unit != null && Actor.Faction == _unit.Faction;
    }


    private bool IsMyRole(Actor _unit)
    {
        return _unit != null && Actor.Role == _unit.Role;
    }


    private bool OnWatch()
    {
        // TODO: once a ruin is captured, switch to sentry and attack incoming enemies
        return false;
    }


    private void ReachedObjective()
    {
        // TODO: move on to the next
    }


    private bool UnderAttack()
    {
        return Threat.GetThreats().Count > 0;
    }
}
