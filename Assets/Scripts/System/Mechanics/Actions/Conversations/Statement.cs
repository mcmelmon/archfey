using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statement : MonoBehaviour
{
    // Inspector
    [SerializeField] Plot associated_plot;
    [SerializeField] bool depends_on_condition;
    [SerializeField] string default_text;
    [SerializeField] string insight_text;
    [SerializeField] string perception_text;

    // properties

    public Actor Me { get; set; }
    public bool ConditionMet { get; set; }
    public bool SeenByPlayer { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    // public

    public string GetStatementToPlayer()
    {
        if (Player.Instance.Me.Senses.InsightCheck(true, Player.Instance.Me.Actions.SkillCheck(true, Proficiencies.Skill.Deception))) {  // TODO: allocate advantage/disadvantage based on race, plot, etc.
            return insight_text;
        } else if (Player.Instance.Me.Senses.PerceptionCheck(true, Player.Instance.Me.Actions.SkillCheck(true, Proficiencies.Skill.Performance))) {
            return perception_text;
        }

        return default_text;
    }

    // private

    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
        ConditionMet = !depends_on_condition;
    }
}
