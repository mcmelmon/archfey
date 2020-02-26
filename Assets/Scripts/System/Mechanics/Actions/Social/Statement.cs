using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statement : MonoBehaviour
{
    // Inspector
    [SerializeField] Plot associated_plot = null;
    [SerializeField] string default_text = "";
    [SerializeField] List<Statement> player_responses = new List<Statement>(); // choices a player can use to respond
    [SerializeField] Statement answer_to_response = null;  // answer the npc will give when this statement is chosen as a response

    // properties

    public Actor Me { get; set; }
    public bool SeenByPlayer { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    // public

    public string GetStatementToPlayer()
    {
        // pick one of the npc's statements based on plots, relation to player, race interactions, etc.

        // if (Player.Instance.Me.Senses.InsightCheck(true, Player.Instance.Me.Actions.SkillCheck(true, Proficiencies.Skill.Deception))) {  // TODO: allocate advantage/disadvantage based on race, plot, etc.
        //     return insight_text;
        // } else if (Player.Instance.Me.Senses.PerceptionCheck(true, Player.Instance.Me.Actions.SkillCheck(true, Proficiencies.Skill.Performance))) {
        //     return perception_text;
        // }

        return default_text;
    }

    public List<Statement> PresentResponses()
    {
        // show the player a set of responses based in insight, perception, deception, etc.

        return null;
    }

    // private

    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
    }
}
