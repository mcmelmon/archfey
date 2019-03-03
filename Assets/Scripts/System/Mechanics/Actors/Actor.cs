using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


public class Actor : MonoBehaviour
{
    public const int rested_at = 5;

    // properties

    public Actions Actions { get; set; }
    public Conflict.Alignment Alignment { get; set; }
    public Dialog Dialog { get; set; }
    public Faction Faction { get; set; }
    public Health Health { get; set; }
    public Interactable Interactions { get; set; }
    public Dictionary<HarvestingNode, int> Load { get; set; }
    public Magic Magic { get; set; }
    public Actor Me { get; set; }
    public int RestCounter { get; set; }
    public Senses Senses { get; set; }
    public Stats Stats { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public IEnumerator GetStatsFromServer(string name)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/stat_blocks/" + name + ".json");
        JSON_StatBlock stat_block = new JSON_StatBlock();
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
            stat_block = JsonUtility.FromJson<JSON_StatBlock>(www.downloadHandler.text);
        }

        Stats.ArmorClass = stat_block.armor_class;
        Stats.AttributeProficiency[Proficiencies.Attribute.Charisma] = stat_block.charisma_proficiency;
        Stats.AttributeProficiency[Proficiencies.Attribute.Constitution] = stat_block.constituion_proficiency;
        Stats.AttributeProficiency[Proficiencies.Attribute.Dexterity] = stat_block.dexterity_proficiency;
        Stats.AttributeProficiency[Proficiencies.Attribute.Intelligence] = stat_block.intelligence_proficiency;
        Stats.AttributeProficiency[Proficiencies.Attribute.Strength] = stat_block.strength_proficiency;
        Stats.AttributeProficiency[Proficiencies.Attribute.Wisdom] = stat_block.wisdom_proficiency;
        Stats.ProficiencyBonus = stat_block.proficiency_bonus;
        Stats.Family = stat_block.family;
        Stats.Size = stat_block.size;

        Actions.ActionsPerRound = stat_block.actions_per_round;
        Actions.Movement.Speed = stat_block.speed;
        Actions.Movement.Agent.speed = stat_block.speed;
        switch (Stats.Size) {
            case "Tiny":
                Actions.Movement.ReachedThreshold = 0.5f;
                break;
            case "Small":
                Actions.Movement.ReachedThreshold = 1f;
                break;
            case "Medium":
                Actions.Movement.ReachedThreshold = 1.5f;
                break;
            case "Large":
                Actions.Movement.ReachedThreshold = 3f;
                break;
            case "Huge":
                Actions.Movement.ReachedThreshold = 5f;
                break;
            case "Gargantuan":
                Actions.Movement.ReachedThreshold = 8f;
                break;
            default:
                Actions.Movement.ReachedThreshold = 1.5f;
                break;
        }

        Health.HitDice = stat_block.hit_dice;
        Health.HitDiceType = stat_block.hit_dice_type;

        Health.SetCurrentAndMaxHitPoints();
    }


    public bool IsPlayer()
    {
        return Me == Player.Instance.Me;
    }


    public Vector3 MoveToInteractionPoint(Actor other_actor)
    {
        Vector3 toward_approach = (other_actor.transform.position - transform.position).normalized * (Me.Actions.Movement.ReachedThreshold + other_actor.Actions.Movement.ReachedThreshold);

        return GetComponent<Collider>().ClosestPointOnBounds(other_actor.transform.position) + toward_approach;
    }


    // private


    private void SetComponents()
    {
        Actions = GetComponentInChildren<Actions>();
        Alignment = Conflict.Alignment.Unaligned;
        Dialog = GetComponent<Dialog>();
        Health = GetComponent<Health>();
        Interactions = GetComponent<Interactable>();
        Load = new Dictionary<HarvestingNode, int>();
        Me = this;
        RestCounter = 0;
        Senses = GetComponent<Senses>();
        Stats = GetComponent<Stats>();

        if (GetComponent<Faction>() != null) Faction = GetComponent<Faction>();
    }


    public class JSON_StatBlock
    {
        public int proficiency_bonus;
        public int charisma_proficiency;
        public int constituion_proficiency;
        public int dexterity_proficiency;
        public int intelligence_proficiency;
        public int strength_proficiency;
        public int wisdom_proficiency;
        public int actions_per_round;
        public int armor_class;
        public int hit_dice;
        public int hit_dice_type;
        public int starting_hit_dice;
        public float speed;
        public string family;
        public string size;
    }
}