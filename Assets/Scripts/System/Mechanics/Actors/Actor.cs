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
    public Faction Faction { get; set; }
    public Health Health { get; set; }
    public List<Actor> Interactors { get; set; }
    public Dictionary<HarvestingNode, int> Load { get; set; }
    public Magic Magic { get; set; }
    public Actor Me { get; set; }
    public int RestCounter { get; set; }
    public Senses Senses { get; set; }
    public float Size { get; set; }
    public Stats Stats { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
        StartCoroutine(FaceInteractor());
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

        Actions.ActionsPerRound = stat_block.actions_per_round;
        Actions.Movement.Speed = stat_block.speed;
        Actions.Movement.Agent.speed = stat_block.speed;

        Health.HitDice = stat_block.hit_dice;
        Health.HitDiceType = stat_block.hit_dice_type;

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

        Health.SetCurrentAndMaxHitPoints();
    }


    public void InitiateDialog(Actor other_actor)
    {
        Debug.Log("Hello, " + other_actor.Stats.name);
    }


    public void InteractWith(Actor other_actor)
    {
        if (Interactors.Contains(other_actor)) return;
        Interactors.Clear(); // for now; in future, interact with player first, then an npc
        Interactors.Add(other_actor);
        other_actor.InteractWith(Me);
    }


    public void InteractWith(Structure structure)
    {

    }


    public Vector3 MoveToInteractionPoint(Vector3 from_point)
    {
        Vector3 toward_approach = (from_point - transform.position).normalized * Movement.ReachedThreshold;

        return GetComponent<Collider>().ClosestPointOnBounds(from_point) + toward_approach;
    }


    public bool WithinDialogRange(Actor other_actor)
    {
        return Interactors.Any(actor => Vector3.Distance(transform.position, actor.transform.position) < (Movement.ReachedThreshold + actor.Size) * 2);
    }


    // private


    private IEnumerator FaceInteractor()
    {
        while (true) {
            if (Interactors.Any(WithinDialogRange)) {
                Vector3 new_facing = Vector3.RotateTowards(transform.forward, Interactors.First().transform.position - transform.position, 90f, 0f);
                transform.rotation = Quaternion.LookRotation(new_facing);
            }
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private void SetComponents()
    {
        Actions = GetComponentInChildren<Actions>();
        Alignment = Conflict.Alignment.Unaligned;
        Health = GetComponent<Health>();
        Interactors = new List<Actor>();
        Load = new Dictionary<HarvestingNode, int>();
        Me = this;
        RestCounter = 0;
        Senses = GetComponent<Senses>();
        Size = GetComponent<Renderer>().bounds.extents.magnitude;
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