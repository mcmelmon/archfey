using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


public class Actor : MonoBehaviour
{
    public const int rested_at = 5;

    // Inspector settings

    public LayerMask ground_layer;

    // properties

    public Actions Actions { get; set; }
    public Conflict.Alignment Alignment { get; set; }
    public Dialog Dialog { get; set; }
    public int ExhaustionLevel { get; set; } // TODO: create exhaustion class
    public Faction Faction { get; set; }
    public Health Health { get; set; }
    public Interactable Interactions { get; set; }
    public Dictionary<HarvestingNode, int> Load { get; set; }
    public Magic Magic { get; set; }
    public Actor Me { get; set; }
    public int RestCounter { get; set; }
    public Route Route { get; set; }
    public Senses Senses { get; set; }
    public Stats Stats { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public Vector3 GetInteractionPoint(Actor other_unit)
    {
        Vector3 toward_approach = (other_unit.transform.position - transform.position).normalized * (Me.Actions.Movement.ReachedThreshold + other_unit.Actions.Movement.ReachedThreshold);

        return GetComponent<Collider>().ClosestPointOnBounds(other_unit.transform.position) + toward_approach;
    }


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

        Senses.Darkvision = stat_block.darkvision;

        Stats.BaseAttributes[Proficiencies.Attribute.Charisma] = stat_block.charisma_proficiency;
        Stats.BaseAttributes[Proficiencies.Attribute.Constitution] = stat_block.constituion_proficiency;
        Stats.BaseAttributes[Proficiencies.Attribute.Dexterity] = stat_block.dexterity_proficiency;
        Stats.BaseAttributes[Proficiencies.Attribute.Intelligence] = stat_block.intelligence_proficiency;
        Stats.BaseAttributes[Proficiencies.Attribute.Strength] = stat_block.strength_proficiency;
        Stats.BaseAttributes[Proficiencies.Attribute.Wisdom] = stat_block.wisdom_proficiency;
        Stats.ProficiencyBonus = stat_block.proficiency_bonus;
        Stats.Family = stat_block.family;
        Stats.Size = stat_block.size;

        Actions.Attack.AttacksPerAction = stat_block.multiattack ? 2 : 1;
        Actions.Movement.BaseSpeed = stat_block.speed;
        Actions.Movement.Agent.speed = stat_block.speed;
        switch (Stats.Size) {
            case "Tiny":
                Actions.Movement.ReachedThreshold = 1.5f;
                break;
            case "Small":
                Actions.Movement.ReachedThreshold = 2f;
                break;
            case "Medium":
                Actions.Movement.ReachedThreshold = 2.5f;
                break;
            case "Large":
                Actions.Movement.ReachedThreshold = 4f;
                break;
            case "Huge":
                Actions.Movement.ReachedThreshold = 5f;
                break;
            case "Gargantuan":
                Actions.Movement.ReachedThreshold = 8f;
                break;
            default:
                Actions.Movement.ReachedThreshold = 2.5f;
                break;
        }

        Stats.BaseArmorClass = stat_block.armor_class; // TODO: build up AC from equipment and dex

        Health.HitDice = stat_block.hit_dice;
        Health.HitDiceType = stat_block.hit_dice_type;

        Health.SetCurrentAndMaxHitPoints();
    }


    public bool IsGrounded()
    {
        CapsuleCollider my_collider = GetComponent<CapsuleCollider>();
        Vector3 my_base = new Vector3(my_collider.bounds.center.x, my_collider.bounds.min.y, my_collider.bounds.center.z);
        return Physics.CheckCapsule(my_collider.bounds.center, my_base, my_collider.radius * .9f, ground_layer);
    }


    public bool IsPlayer()
    {
        return Me == Player.Instance.Me;
    }


    public float SeparationFrom(Actor other_unit)
    {
        Vector3 their_interaction_point = other_unit.GetInteractionPoint(Me);
        float separation = Vector3.Distance(transform.position, their_interaction_point) - Me.Actions.Movement.ReachedThreshold;
        return separation;
    }


    // private


    private void SetComponents()
    {
        Actions = GetComponentInChildren<Actions>();
        Alignment = Conflict.Alignment.Unaligned;
        Dialog = GetComponent<Dialog>();
        ExhaustionLevel = 0;
        Health = GetComponent<Health>();
        Interactions = GetComponent<Interactable>();
        Load = new Dictionary<HarvestingNode, int>();
        Me = this;
        RestCounter = 0;
        Route = GetComponent<Route>();
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
        public int armor_class;
        public int hit_dice;
        public int hit_dice_type;
        public int starting_hit_dice;
        public float speed;
        public string family;
        public string size;
        public bool darkvision;
        public bool multiattack;
    }
}