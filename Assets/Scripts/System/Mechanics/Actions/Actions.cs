using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actions : MonoBehaviour
{
    // properties

    public bool CanTakeAction { get; set; }
    public bool CanTakeBonusAction { get; set; }
    public Combat Combat { get; set; }
    public Decider Decider { get; set; }
    public bool InCombat { get; set; }
    public Actor Me { get; set; }
    public Movement Movement { get; set; }
    public Stats Stats { get; set; }
    public Stealth Stealth { get; set; }

    // action properties

    public Action OnBadlyInjured { get; set; }
    public Action OnCrafting { get; set; }
    public Action OnDamagedFriendlyStructuresSighted { get; set; }
    public Action OnFriendlyActorsSighted { get; set; }
    public Action OnFriendsInNeed { get; set; }
    public Action OnFullLoad { get; set; }
    public Action OnHarvesting { get; set; }
    public Action OnHasObjective { get; set; }
    public Action OnHostileActorsSighted { get; set; }
    public Action OnHostileStructuresSighted { get; set; }
    public Action OnIdle { get; set; }
    public Action OnInCombat { get; set; }
    public Action OnMedic { get; set; }
    public Action OnMovingToGoal { get; set; }
    public Action OnNeedsRest { get; set; }
    public Action OnReachedGoal { get; set; }
    public Action OnResting { get; set; }
    public Action OnUnderAttack { get; set; }
    public Action OnWatch { get; set; }

    // Unity

    void Awake()
    {
        SetComponents();
    }

    // public

    public void ActOnTurn()
    {
        CanTakeAction |= (Me == Player.Instance.Me);
        CanTakeBonusAction |= (Me == Player.Instance.Me);

        if (Stealth.IsHiding) {
            Stealth.Hide(); // re-up the Stealth CR for the round; TODO: account for obscurity at the new location, etc
        }

        if (Stealth.IsPerforming) {
            Stealth.Performance(); // re-up the Performance CR
        }

        Decider.ChooseState();

        switch (Decider.state) {
            case Decider.State.BadlyInjured:
                OnBadlyInjured?.Invoke();
                break;
            case Decider.State.Crafting:
                OnCrafting?.Invoke();
                break;
            case Decider.State.FriendsInNeed:
                OnFriendsInNeed?.Invoke();
                break;
            case Decider.State.FriendlyActorsSighted:
                OnFriendlyActorsSighted?.Invoke();
                break;
            case Decider.State.DamagedFriendlyStructuresSighted:
                OnDamagedFriendlyStructuresSighted?.Invoke();
                break;
            case Decider.State.FullLoad:
                OnFullLoad?.Invoke();
                break;
            case Decider.State.Harvesting:
                OnHarvesting?.Invoke();
                break;
            case Decider.State.HasObjective:
                OnHasObjective?.Invoke();
                break;
            case Decider.State.HostileActorsSighted:
                OnHostileActorsSighted?.Invoke();
                break;
            case Decider.State.HostileStructuresSighted:
                OnHostileStructuresSighted?.Invoke();
                break;
            case Decider.State.Idle:
                OnIdle?.Invoke();
                break;
            case Decider.State.InCombat:
                OnInCombat?.Invoke();
                break;
            case Decider.State.Medic:
                OnMedic?.Invoke();
                break;
            case Decider.State.MovingToGoal:
                OnMovingToGoal?.Invoke();
                break;
            case Decider.State.NeedsRest:
                OnNeedsRest?.Invoke();
                break;
            case Decider.State.ReachedGoal:
                OnReachedGoal?.Invoke();
                break;
            case Decider.State.Resting:
                OnResting?.Invoke();
                break;
            case Decider.State.UnderAttack:
                OnUnderAttack?.Invoke();
                break;
            case Decider.State.Watch:
                OnWatch?.Invoke();
                break;
            default:
                OnIdle?.Invoke();
                break;
        }
    }

    public void Attack(bool offhand = false, bool player_target = false)
    {
        if (Decider.Target == null) {
            Combat.Engaged = false;
            return;
        }

        for (int i = 0; i < Combat.AttacksPerAction; i++) {
            Combat.StrikeEnemy(Decider.Target, Decider.AttackAtRange, offhand, player_target);
        }

        Me.RestCounter = 0;
    }

    public int AttributeCheck(bool active, Proficiencies.Attribute attribute, bool advantage = false, bool disadvatnage = false)
    {
        // Don't compare with a challenge rating here, because this may be to create the challenge rating (e.g. a grapple)
        int proficiency_bonus = Me.Stats.ProficiencyBonus;
        int attribute_bonus = Me.Stats.GetAdjustedAttributeModifier(attribute);

        int roll = active ? RollDie(20, 1, advantage, disadvatnage) : 10;

        return roll + attribute_bonus;
    }

    public void CallForHelp()
    {
        List<Actor> friends = Decider.IdentifyFriends();

        // TODO: this should be possible as a Select, but there are type problems, and may be a problem if 
        // actors are destroyed in process
        for (int i = 0; i < friends.Count; i++) {
            if (Me == null) break;
            if (friends[i] != null && !friends[i].Actions.Decider.FriendsInNeed.Contains(Me)) {
                if (friends[i].GetComponent<Guard>() != null || Me.GetComponent<Guard>() != null)
                    friends[i].Actions.Decider.FriendsInNeed.Add(Me);
            }
        }
    }

    public void CloseWithEnemies()
    {
        if (transform == null) return;

        Actor nearest_enemy = Decider.TargetEnemy()?.GetComponent<Actor>();

        if (nearest_enemy != null && !Me.Actions.Combat.IsWithinMeleeRange(nearest_enemy.transform)) {
            StartCoroutine(Movement.TrackUnit(nearest_enemy));
        }
    }

    public void FleeFromEnemies()
    {
        if (Me == null) return;

        SheathWeapon();

        Vector3 run_away_from = Vector3.zero;

        var enemies = Me.Actions.Decider.IdentifyEnemies();

        if (enemies.Count > 0) {
            var _enemy = enemies.OrderBy(e => Vector3.Distance(transform.position, e.transform.position)).First();
            Vector3 run_away_direction = (transform.position - _enemy.transform.position).normalized;
            Vector3 run_away_to = transform.position + (run_away_direction * Movement.Agent.speed * Movement.Agent.speed);
            Movement.SetDestination(run_away_to);
        }
    }

    public void KeepEnemiesAtRange()
    {
        if (transform == null) return;

        Actor nearest_enemy = Decider.TargetEnemy()?.GetComponent<Actor>();

        if (nearest_enemy != null && (Me.Actions.Combat.IsWithinMeleeRange(nearest_enemy.transform) || !Me.Actions.Combat.IsWithinAttackRange(nearest_enemy.transform))) {
            StartCoroutine(Movement.HarassUnit(nearest_enemy));
        }
    }

    public int RollDie(int dice_type, int number_of_rolls, bool advantage = false, bool disadvantage = false)
    {
        int die_roll = 0;

        if (number_of_rolls > 1 || (advantage && disadvantage) || (!advantage && !disadvantage)) {
            // advantage only applies in situations with one roll

            for (int i = 0; i < number_of_rolls; i++) {
                int this_roll = UnityEngine.Random.Range(1, dice_type + 1);
                die_roll += this_roll;
            }
        } else if (advantage) {
            die_roll = Mathf.Max(UnityEngine.Random.Range(1, dice_type + 1), UnityEngine.Random.Range(1, dice_type + 1));
        } else if (disadvantage) {
            die_roll = Mathf.Min(UnityEngine.Random.Range(1, dice_type + 1), UnityEngine.Random.Range(1, dice_type + 1));
        }

        return die_roll;
    }

    public bool SavingThrow(Proficiencies.Attribute attribute, int challenge_rating, bool advantage = false, bool disadvantage = false)
    {
        int proficiency_bonus = Me.Stats.SavingThrows.Contains(attribute) ? Me.Stats.ProficiencyBonus : 0;
        int attribute_bonus = Me.Stats.GetAdjustedAttributeModifier(attribute);
        int bonus = proficiency_bonus + attribute_bonus;

        int die_roll = RollDie(20, 1, advantage, disadvantage);

        return die_roll + bonus > challenge_rating;
    }

    public bool Search()
    {
        // TODO: get a list of anything interesting within range
        // compare the difficulty of finding it with the skill check roll

        // Use Senses.Investigation or Perception, whichever is higher

        return false;
    }

    public void SheathWeapon()
    {
        Combat.Engaged = false;
        if (Combat.EquippedMeleeWeapon != null) Combat.EquippedMeleeWeapon.gameObject.SetActive(false);
        if (Combat.EquippedRangedWeapon != null) Combat.EquippedRangedWeapon.gameObject.SetActive(false);
        if (Combat.EquippedOffhand != null) Combat.EquippedOffhand.gameObject.SetActive(false);
    }

    public int SkillCheck(bool active, Proficiencies.Skill skill, bool advantage = false, bool disadvatnage = false)
    {
        // Don't compare with the challenge rating in here because we may be rolling to create the challenge rating (e.g. trying to hide or bluff)

        bool proficient = Me.Stats.Skills.Contains(skill);
        bool expertise = Me.Stats.ExpertiseInSkills.Contains(skill);
        int proficiency_bonus = expertise ? Me.Stats.ProficiencyBonus * 2 : Me.Stats.ProficiencyBonus;
        int attribute_bonus = Me.Stats.GetAdjustedAttributeModifier(Proficiencies.Instance.GetAttributeForSkill(skill));
        int bonus = proficient ? proficiency_bonus + attribute_bonus : attribute_bonus;

        int roll = active ? RollDie(20, 1, advantage, disadvatnage) : 10;

        return roll + bonus;
    }

    public int ToolCheck(Proficiencies.Tool tool, bool advantage = false, bool disadvatnage = false)
    {
        // We may be rolling to create the challenge rating (e.g. a forgery), so don't check for "success" here
        bool proficient = Me.Stats.Tools.Contains(tool);
        bool expertise = Me.Stats.ExpertiseInTools.Contains(tool);
        int proficiency_bonus = expertise ? Me.Stats.ProficiencyBonus * 2 : Me.Stats.ProficiencyBonus;

        int roll = RollDie(20, 1, advantage, disadvatnage);

        return roll + proficiency_bonus;
    }

    // private

    private void SetComponents()
    {
        Combat = GetComponentInChildren<Combat>();
        Decider = GetComponent<Decider>();
        Stats = GetComponentInParent<Stats>();
        Stealth = GetComponentInParent<Stealth>();
        Me = GetComponentInParent<Actor>();
        Movement = GetComponent<Movement>();
        CanTakeAction = true;
    }
}
