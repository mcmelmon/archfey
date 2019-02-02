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
        } else if (CallsForHelp()) {
            SetState(State.FriendsInNeed);
        } else if (HostileActorsSighted()) {
            SetState(State.HostileActorsSighted);
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
        return Me.Senses.Actors.Where(IsFriendOrNeutral).ToList();
    }


    public List<Actor> IdentifyEnemies()
    {
        return Me.Senses.Actors.Where(a => !IsFriendOrNeutral(a)).ToList();
    }


    public bool IsFriendOrNeutral(Actor other_unit)
    {
        if (other_unit == null || other_unit == gameObject) return true;
        if (IsMyAlignment(other_unit)) return true;
        if (other_unit.Alignment == Conflict.Alignment.Neutral || other_unit.Alignment == Conflict.Alignment.Unaligned) return true;

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
        if (Me.Actions.OnDamagedFriendlyStructuresSighted == null) return false;

        FriendlyStructures = Me.Senses.Structures
                               .Where(structure => structure.alignment == Me.Alignment && structure.CurrentHitPoints < structure.maximum_hit_points)
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
                              .Where(structure => structure.alignment != Me.Alignment && structure.CurrentHitPoints > 0)
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
        return Me.Magic != null && Me.Magic.HaveSpellSlot(Magic.Level.First) && Me.Senses.Actors.Where(IsFriendOrNeutral).ToList().Where(friend => friend.Health.BadlyInjured() == true).ToList().Count > 0;
    }


    private bool Moving()
    {
        return Me.Actions.Movement.InProgress();
    }


    private bool NeedsRest()
    {
        bool spent_spell_slots = Me.Magic != null && Me.Magic.UsedSlot;
        return !Me.Actions.Attack.Engaged() && !HostileActorsSighted() && (Me.Health.CurrentHitPoints < Me.Health.MaximumHitPoints || spent_spell_slots );
    }


    private bool ReachedGoal()
    {
        return (previous_state == State.MovingToGoal || previous_state == State.FullLoad || previous_state == State.Idle) && !Me.Actions.Movement.InProgress();
    }

    private bool Resting()
    {
        return (previous_state == State.NeedsRest || previous_state == State.Resting) && NeedsRest() && !Me.Actions.Movement.InProgress();
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
