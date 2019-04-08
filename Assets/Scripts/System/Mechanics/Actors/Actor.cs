using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Actor : MonoBehaviour
{
    public const int rested_at = 5;

    // Inspector settings

    public LayerMask ground_layer;
    public Transform offhand_transform;
    public Transform weapon_transform;

    // properties

    public Actions Actions { get; set; }
    public Conflict.Alignment Alignment { get; set; }
    public Dialog Dialog { get; set; }
    public int ExhaustionLevel { get; set; } // TODO: create exhaustion class
    public Faction CurrentFaction { get; set; }
    public List<Faction> Factions { get; set; }
    public Health Health { get; set; }
    public Interactable Interactions { get; set; }
    public Inventory Inventory { get; set; }
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


    public void ChangeFaction(Faction new_faction)
    {
        if (!Factions.Contains(CurrentFaction)) Factions.Add(CurrentFaction);
        Me.CurrentFaction = new_faction;
        Renderer rend = GetComponent<Renderer>();
        rend.sharedMaterial.SetColor("_BaseColor", new_faction.colors);
    }


    public Vector3 GetHarassPoint(Actor other_unit)
    {
        if (other_unit == null || Me == null) return Vector3.zero;

        if (other_unit.Actions.Combat.EquippedRangedWeapon != null) {
            float melee_range = other_unit.Actions.Combat.MeleeRange();
            float long_range = other_unit.Actions.Combat.EquippedRangedWeapon.Range;
            float target_range = (long_range - melee_range) + (melee_range * 0.66f); // the edge of long range is too far, but within melee is too close
            Vector3 harass_point = (other_unit.transform.position - transform.position).normalized * target_range;
            Vector3 their_bottom = other_unit.GetComponent<Collider>().bounds.min;
            return new Vector3((transform.position + harass_point).x, their_bottom.y, (transform.position + harass_point).z);
        }
        return GetInteractionPoint(other_unit);
    }


    public Vector3 GetInteractionPoint(Actor other_unit)
    {
        // The point on Me that other_unit will move to so that I am in their range
        Vector3 toward_approach = (other_unit.transform.position - transform.position).normalized * other_unit.Actions.Movement.ReachedThreshold;
        Vector3 interaction_point = GetComponent<Collider>().ClosestPointOnBounds(other_unit.transform.position) + toward_approach;
        Vector3 their_bottom = other_unit.GetComponent<Collider>().bounds.min;

        return new Vector3(interaction_point.x, their_bottom.y, interaction_point.z);
    }


    public IEnumerator GetStatsFromServer(string name)
    {
        if (name == "Sebbie") name = "Goblin";
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

        Actions.Combat.AttacksPerAction = stat_block.multiattack ? 2 : 1;
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
                Actions.Movement.ReachedThreshold = 3f;
                break;
            case "Huge":
                Actions.Movement.ReachedThreshold = 3.5f;
                break;
            case "Gargantuan":
                Actions.Movement.ReachedThreshold = 4f;
                break;
            default:
                Actions.Movement.ReachedThreshold = 2.5f;
                break;
        }

        Stats.BaseArmorClass = stat_block.armor_class;

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


    public float SeparationFrom(Transform target)
    {
        Actor actor = target.GetComponent<Actor>();
        Structure structure = target.GetComponent<Structure>();
        Vector3 their_closest_point_to_me = Vector3.zero;
        Vector3 their_bottom = Vector3.zero;
        Vector3 my_closest_point_to_them = Vector3.zero;
        Vector3 my_bottom = Vector3.zero;
        Vector3 structure_interaction_point = Vector3.zero;
        float separation = float.MaxValue;

        if (actor != null) {
            their_closest_point_to_me = actor.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            their_bottom = actor.GetComponent<Collider>().bounds.min;
            my_closest_point_to_them = GetComponent<Collider>().ClosestPointOnBounds(actor.transform.position);
            my_bottom = GetComponent<Collider>().bounds.min;
            separation = Vector3.Distance(new Vector3(my_closest_point_to_them.x, my_bottom.y, my_closest_point_to_them.z), new Vector3(their_closest_point_to_me.x, their_bottom.y, their_closest_point_to_me.z));
        }
        else if (structure != null)
        {
            structure_interaction_point = structure.GetInteractionPoint(Me);
            separation = Vector3.Distance(structure_interaction_point, transform.position);
        }

        return separation;
    }


    // private


    private void SetComponents()
    {
        Actions = GetComponentInChildren<Actions>();
        Alignment = Conflict.Alignment.Unaligned;
        CurrentFaction = null;
        Dialog = GetComponent<Dialog>();
        ExhaustionLevel = 0;
        Factions = new List<Faction>();
        Health = GetComponent<Health>();
        Interactions = GetComponent<Interactable>();
        Inventory = GetComponent<Inventory>();
        Load = new Dictionary<HarvestingNode, int>();
        Me = this;
        RestCounter = 0;
        Route = GetComponent<Route>();
        Senses = GetComponent<Senses>();
        Stats = GetComponent<Stats>();

        if (GetComponent<Faction>() != null) CurrentFaction = GetComponent<Faction>();
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