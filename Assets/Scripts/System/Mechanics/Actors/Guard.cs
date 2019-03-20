using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Guard : MonoBehaviour, IAct
{

    // properties

    public Actor Me { get; set; }


    // Unity


    private void Start()
    {
        SetStats();
    }


    // public


    public void OnBadlyInjured()
    {
        Me.Actions.Attack();
        FindShrine();
    }


    public void OnCrafting() { }


    public void OnFriendlyActorsSighted() { }

    public void OnDamagedFriendlyStructuresSighted() { }


    public void OnFriendsInNeed()
    {
        Me.Actions.Movement.SetDestination(Me.Actions.Decider.FriendsInNeed.First().transform);
        Me.Actions.Attack();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public void OnFullLoad() { }


    public void OnHarvesting() { }


    public void OnInCombat()
    {
        Me.Actions.CallForHelp();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public void OnHostileActorsSighted()
    {
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
    }


    public void OnHostileStructuresSighted()
    {
        if (Me.Actions.Decider.HostileStructures.Count > 0) {
            Structure target = Me.Actions.Decider.HostileStructures[Random.Range(0, Me.Actions.Decider.HostileStructures.Count)];
            Me.Actions.Movement.SetDestination(target.GetInteractionPoint(Me));
        }

        Me.Actions.Attack();
    }


    public void OnIdle()
    {
        Me.Actions.SheathWeapon();
        Me.Actions.Movement.Home();
    }


    public void OnMedic()
    {

    }


    public void OnMovingToGoal()
    {
        Me.Actions.SheathWeapon();
    }


    public void OnNeedsRest()
    {
        Me.Actions.SheathWeapon();
        Me.Actions.Movement.SetDestination(Me.Actions.Movement.Destinations[Movement.CommonDestination.Home]);
    }


    public void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public void OnUnderAttack()
    {
        Me.Actions.Decider.FriendsInNeed.Clear();
        Me.Actions.CloseWithEnemies();
        Me.Actions.Attack();
        Me.RestCounter = 0;
    }


    public void OnWatch()
    {
        Me.Actions.Movement.ResetPath();
        Me.Actions.Attack();
    }


    // private


    private void FindShrine()
    {
        Structure nearest_sacred_structure = new List<Structure>(FindObjectsOfType<Structure>())
            .Where(s => s.alignment == Me.Alignment && s.purpose == Structure.Purpose.Sacred)
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        Me.Actions.Movement.SetDestination(nearest_sacred_structure.transform);
    }


    private void SetStats()
    {
        Me = GetComponent<Actor>();
        StartCoroutine(Me.GetStatsFromServer(this.GetType().Name));
        SetAdditionalStats();
        Me.Actions.Movement.AddDestination(Movement.CommonDestination.Home, transform.position);
    }


    private void SetAdditionalStats()
    {
        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.Chain_Shirt));
        Me.Actions.Combat.EquipShield(Armors.Instance.GetArmorNamed(Armors.ArmorName.Shield));
        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Spear));
        Me.Actions.Combat.EquipRangedWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Longbow)); 
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];

        Me.Stats.Skills.Add(Proficiencies.Skill.Perception);
        Me.Stats.Skills.Add(Proficiencies.Skill.Intimidation);
    }
}