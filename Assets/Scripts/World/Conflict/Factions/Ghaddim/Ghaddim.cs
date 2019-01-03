using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghaddim : MonoBehaviour {

    // properties

    public Actor Actor { get; set; }
    public static Threat Threat { get; set; }


    // static


    public static GameObject SpawnUnit(Vector3 _point)
    {
        Ghaddim _ghaddim = Instantiate(Conflict.Instance.ghaddim_prefab, _point, Conflict.Instance.ghaddim_prefab.transform.rotation);  // drop from on high to avoid being inside buildings
        _ghaddim.gameObject.AddComponent<Soldier>();

        return _ghaddim.gameObject;
    }


    // Unity


    private void Awake()
    {
        Actor = GetComponent<Actor>();
        Threat = gameObject.AddComponent<Threat>();
    }


    // public


    public void AddFactionThreat(Actor _foe, float _threat)
    {
        Threat.AddThreat(_foe, _threat);
    }


    public Actor BiggestFactionThreat()
    {
        return Threat.BiggestThreat();
    }


    public bool IsFactionThreat(Actor _sighting)
    {
        return _sighting != null && Threat.IsAThreat(_sighting);
    }


    public void SetStats()
    {
        SetPrimaryAndInnateStats();
        SetDefenseStats();
        SetHealthStats();
        SetOffensiveStats();
    }


    // private


    private void SetDefenseStats()
    {
        Defend defend = GetComponentInChildren<Defend>();
        if (defend == null) return;

        if (GetComponent<Gnoll>() != null)
        {
            defend.ArmorClass = ConfigureGhaddim.armor_class[Soldier.Template.Gnoll];
            defend.SetResistances(ConfigureGhaddim.resistances[Soldier.Template.Gnoll]);
        }
    }


    private void SetHealthStats()
    {
        Health health = GetComponent<Health>();
        if (health == null) return;

        if (GetComponent<Gnoll>() != null)
        {
            health.CurrentHitPoints = (ConfigureGhaddim.starting_health[Soldier.Template.Gnoll]);
            health.HitDice = (ConfigureGhaddim.hit_dice[Soldier.Template.Gnoll]);
            health.HitDiceType = (ConfigureGhaddim.hit_dice_type[Soldier.Template.Gnoll]);
            health.MaximumHitPoints = (ConfigureGhaddim.starting_health[Soldier.Template.Gnoll]);
        }
    }


    private void SetOffensiveStats()
    {
        if (GetComponent<Gnoll>() != null)
        {
            Actor.Actions = ConfigureGhaddim.actions[Soldier.Template.Gnoll];
            Actor.ObjectiveControlRating = ConfigureGhaddim.objective_control_rating[Soldier.Template.Gnoll];
            Actor.Attack.AvailableWeapons = ConfigureGhaddim.available_weapons[Soldier.Template.Gnoll];
        }
    }


    private void SetPrimaryAndInnateStats()
    {
        if (GetComponent<Gnoll>() != null)
        {
            Actor.Movement.Agent.speed = ConfigureGhaddim.speed[Soldier.Template.Gnoll];
            Actor.ObjectiveControlRating = ConfigureGhaddim.objective_control_rating[Soldier.Template.Gnoll];
            Actor.Resources.CurrentMana = ConfigureGhaddim.current_mana[Soldier.Template.Gnoll];
            Actor.Resources.IsCaster = ConfigureGhaddim.is_caster[Soldier.Template.Gnoll];
            Actor.Resources.mana_pool_maximum = ConfigureGhaddim.mana_pool_maximum[Soldier.Template.Gnoll];
            Actor.Senses.Darkvision = ConfigureGhaddim.darkvision_range[Soldier.Template.Gnoll];
            Actor.Senses.PerceptionRange = ConfigureGhaddim.perception_range[Soldier.Template.Gnoll];
            Actor.Stats.CharismaProficiency = ConfigureGhaddim.charisma_proficiency[Soldier.Template.Gnoll];
            Actor.Stats.ConstitutionProficiency = ConfigureGhaddim.constituion_proficiency[Soldier.Template.Gnoll];
            Actor.Stats.DexterityProficiency = ConfigureGhaddim.dexterity_proficiency[Soldier.Template.Gnoll];
            Actor.Stats.IntelligenceProficiency = ConfigureGhaddim.intelligence_proficiency[Soldier.Template.Gnoll];
            Actor.Stats.StrengthProficiency = ConfigureGhaddim.strength_proficiency[Soldier.Template.Gnoll];
            Actor.Stats.WisdomProficiency = ConfigureGhaddim.wisdom_proficiency[Soldier.Template.Gnoll];
        }
    }
}