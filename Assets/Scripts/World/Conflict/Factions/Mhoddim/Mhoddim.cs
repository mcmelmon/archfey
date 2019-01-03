using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mhoddim : MonoBehaviour {

    // properties

    public Actor Actor { get; set; }
    public static Threat Threat { get; set; }


    // static


    public static GameObject SpawnUnit(Vector3 _point)
    {
        Mhoddim _mhoddim = Instantiate(Conflict.Instance.mhoddim_prefab, _point, Conflict.Instance.mhoddim_prefab.transform.rotation);  // drop from on high to avoid being in buildings etc.
        _mhoddim.gameObject.AddComponent<Soldier>();

        if (_mhoddim.GetComponent<NavMeshAgent>() == null) {
            Debug.Log("No agent.");
        }

        return _mhoddim.gameObject;
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

        if (GetComponent<Commoner>() != null) {
            defend.ArmorClass = ConfigureMhoddim.armor_class[Soldier.Template.Commoner];
            defend.SetResistances(ConfigureMhoddim.resistances[Soldier.Template.Commoner]);
        }
    }


    private void SetHealthStats()
    {
        Health health = GetComponent<Health>();
        if (health == null) return;

        if (GetComponent<Commoner>() != null) {
            health.CurrentHitPoints = (ConfigureMhoddim.starting_health[Soldier.Template.Commoner]);
            health.HitDice = (ConfigureMhoddim.hit_dice[Soldier.Template.Commoner]);
            health.HitDiceType = (ConfigureMhoddim.hit_dice_type[Soldier.Template.Commoner]);
            health.MaximumHitPoints = (ConfigureMhoddim.starting_health[Soldier.Template.Commoner]);
        }
    }


    private void SetOffensiveStats()
    {
        if (GetComponent<Commoner>() != null) {
            Actor.Actions = ConfigureMhoddim.actions[Soldier.Template.Commoner];
            Actor.ObjectiveControlRating = ConfigureMhoddim.objective_control_rating[Soldier.Template.Commoner];
            Actor.Attack.AvailableWeapons = ConfigureMhoddim.available_weapons[Soldier.Template.Commoner];
        }
    }


    private void SetPrimaryAndInnateStats()
    {
        if (GetComponent<Commoner>() != null)
        {
            Actor.Movement.Agent.speed = ConfigureMhoddim.speed[Soldier.Template.Commoner];
            Actor.ObjectiveControlRating = ConfigureMhoddim.objective_control_rating[Soldier.Template.Commoner];
            Actor.Resources.CurrentMana = ConfigureMhoddim.current_mana[Soldier.Template.Commoner];
            Actor.Resources.IsCaster = ConfigureMhoddim.is_caster[Soldier.Template.Commoner];
            Actor.Resources.mana_pool_maximum = ConfigureMhoddim.mana_pool_maximum[Soldier.Template.Commoner];
            Actor.Senses.Darkvision = ConfigureMhoddim.darkvision_range[Soldier.Template.Commoner];
            Actor.Senses.PerceptionRange = ConfigureMhoddim.perception_range[Soldier.Template.Commoner];
            Actor.Stats.CharismaProficiency = ConfigureMhoddim.charisma_proficiency[Soldier.Template.Commoner];
            Actor.Stats.ConstitutionProficiency = ConfigureMhoddim.constituion_proficiency[Soldier.Template.Commoner];
            Actor.Stats.DexterityProficiency = ConfigureMhoddim.dexterity_proficiency[Soldier.Template.Commoner];
            Actor.Stats.IntelligenceProficiency = ConfigureMhoddim.intelligence_proficiency[Soldier.Template.Commoner];
            Actor.Stats.StrengthProficiency = ConfigureMhoddim.strength_proficiency[Soldier.Template.Commoner];
            Actor.Stats.WisdomProficiency = ConfigureMhoddim.wisdom_proficiency[Soldier.Template.Commoner];
        }
    }
}