using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellcaster : MonoBehaviour
{

    // properties

    public Actor Me { get; set; }
    public int Level { get; set; }
    public Proficiencies.Attribute CastingAttribute { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    // public

    public int AttackModifier()
    {
        return Me.Stats.ProficiencyBonus + Me.Stats.GetAdjustedAttributeModifier(CastingAttribute);  // TODO: plus additional modifiers
    }

    public int DifficultyChallenge()
    {
        return 8 + Me.Stats.GetAdjustedAttributeModifier(CastingAttribute) + Me.Stats.ProficiencyBonus;  // TODO: plus additional modifiers
    }

    // private

    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        Level = 1;
    }
}
