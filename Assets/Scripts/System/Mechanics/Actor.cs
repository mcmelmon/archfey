using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Actor : MonoBehaviour
{
    // Inspector settings
    public Resources.Raw harvesting = Resources.Raw.None;
    public int harvested_amount = 0;
    public int experience_points;

    // properties

    public Actions Actions { get; set; }
    public Conflict.Faction Faction { get; set; }
    public Fey Fey { get; set; }
    public Ghaddim Ghaddim { get; set; }
    public Health Health { get; set; }
    public Dictionary<HarvestingNode, int> Load { get; set; }
    public Mhoddim Mhoddim { get; set; }
    public Conflict.Role Role { get; set; }
    public Senses Senses { get; set; }
    public float Size { get; set; }
    public Stats Stats { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public IEnumerator GetStatsFromServer(string name)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/stat_blocks.json?name=" + name);
        StatBlock stat_block = new StatBlock();
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            stat_block = JsonUtility.FromJson<StatBlock>(www.downloadHandler.text);
        }

        Actions.ActionsPerRound = stat_block.actions_per_round;
        Actions.Movement.Speed = stat_block.speed;
        Actions.Movement.Agent.speed = stat_block.speed;

        Health.HitDice = stat_block.hit_dice;
        Health.HitDiceType = stat_block.hit_dice_type;

        Stats.ArmorClass = stat_block.armor_class;
        Stats.CharismaProficiency = stat_block.charisma_proficiency;
        Stats.ConstitutionProficiency = stat_block.constituion_proficiency;
        Stats.DexterityProficiency = stat_block.dexterity_proficiency;
        Stats.IntelligenceProficiency = stat_block.intelligence_proficiency;
        Stats.StrengthProficiency = stat_block.strength_proficiency;
        Stats.WisdomProficiency = stat_block.wisdom_proficiency;
        Stats.ProficiencyBonus = stat_block.proficiency_bonus;
    }


    // private


    private void SetComponents()
    {
        Actions = GetComponentInChildren<Actions>();
        Fey = GetComponent<Fey>();
        Ghaddim = GetComponent<Ghaddim>();
        Health = GetComponent<Health>();
        Mhoddim = GetComponent<Mhoddim>();
        Load = new Dictionary<HarvestingNode, int>();
        Role = Conflict.Role.None;  // offense and defense set this role for mortals
        Senses = GetComponent<Senses>();
        Size = GetComponent<Renderer>().bounds.extents.magnitude;
        Stats = GetComponent<Stats>();

        Faction = (Fey != null) ? Conflict.Faction.Fey : (Ghaddim != null) ? Conflict.Faction.Ghaddim : Conflict.Faction.Mhoddim;
    }
}