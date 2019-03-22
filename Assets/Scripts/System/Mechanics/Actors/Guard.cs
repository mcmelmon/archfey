using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Guard : TemplateMelee
{
    // Unity


    private void Start()
    {
        SetStats();
    }


    // public


    public override void OnBadlyInjured()
    {
        Me.Actions.Attack();
        FindShrine();
    }


    public override void OnFriendsInNeed()
    {
        Me.Actions.Movement.SetDestination(Me.Actions.Decider.FriendsInNeed.First().transform);
        Me.Actions.Attack();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public override void OnInCombat()
    {
        Me.Actions.CallForHelp();
        base.OnInCombat();
    }



    public void OnIdle()
    {
        Me.Actions.SheathWeapon();
        Me.Actions.Movement.Home();
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