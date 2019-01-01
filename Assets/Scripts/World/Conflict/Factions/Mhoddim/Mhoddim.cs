using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mhoddim : MonoBehaviour {

    // properties

    public Stats Stats { get; set; }
    public static Threat Threat { get; set; }


    // static


    public static GameObject SpawnUnit(Vector3 _point)
    {
        Mhoddim _mhoddim = Instantiate(Conflict.Instance.mhoddim_prefab, _point + new Vector3(0, 10, 0), Conflict.Instance.mhoddim_prefab.transform.rotation);  // drop from on high to avoid being in buildings etc.
        _mhoddim.gameObject.AddComponent<Soldier>();

        if (_mhoddim.GetComponent<NavMeshAgent>() == null) {
            Debug.Log("No agent.");
        }

        return _mhoddim.gameObject;
    }


    // Unity

    private void Awake()
    {
        Threat = gameObject.AddComponent<Threat>();
        Stats = GetComponent<Stats>();
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
        SetPrimaryStats();
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
        if (GetComponent<Commoner>() != null) GetComponent<Actor>().Actions = ConfigureMhoddim.actions[Soldier.Template.Commoner];
    }


    private void SetPrimaryStats()
    {
        if (GetComponent<Commoner>() != null)
        {
            Stats.CharismaProficiency = ConfigureMhoddim.charisma_proficiency[Soldier.Template.Commoner];
            Stats.ConstitutionProficiency = ConfigureMhoddim.constituion_proficiency[Soldier.Template.Commoner];
            Stats.DexterityProficiency = ConfigureMhoddim.dexterity_proficiency[Soldier.Template.Commoner];
            Stats.IntelligenceProficiency = ConfigureMhoddim.intelligence_proficiency[Soldier.Template.Commoner];
            Stats.StrengthProficiency = ConfigureMhoddim.strength_proficiency[Soldier.Template.Commoner];
            Stats.WisdomProficiency = ConfigureMhoddim.wisdom_proficiency[Soldier.Template.Commoner];
        }
    }
}