using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spellcaster : MonoBehaviour
{

    // properties

    public Actor Me { get; set; }
    public Magic.CastingClass CastingClass { get; set; }
    public int Level { get; set; }
    public Proficiencies.Attribute CastingAttribute { get; set; }
    public Dictionary<Magic.Level, int> MaximumSpellSlots { get; set; }
    public Dictionary<Magic.Level, int> SpellsLeft { get; set; }
    public bool UsedASlot { get; set; }

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

    public bool HaveSpellSlot(Magic.Level _level)
    {
        // return true if the spellcaster has a spell slot of the specified level or higher
        foreach (KeyValuePair<Magic.Level, int> level_spells in SpellsLeft.Where(sl => (int)sl.Key >= (int)_level)) {
            if (level_spells.Value > 0) return true;
        }
        return false;
    }

    public void UseSpellSlot(Magic.Level _level)
    {
        if (HaveSpellSlot(_level)) SpellsLeft[_level]--;
    }

    // private

    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        Level = 1;

        MaximumSpellSlots = new Dictionary<Magic.Level, int> {
            [Magic.Level.Cantrip] = 0,
            [Magic.Level.First] = 0,
            [Magic.Level.Second] = 0,
            [Magic.Level.Third] = 0,
            [Magic.Level.Fourth] = 0,
            [Magic.Level.Fifth] = 0,
            [Magic.Level.Sixth] = 0,
            [Magic.Level.Seventh] = 0,
            [Magic.Level.Eighth] = 0,
            [Magic.Level.Ninth] = 0
        };

        SpellsLeft = new Dictionary<Magic.Level, int> {
            [Magic.Level.First] = 0,
            [Magic.Level.Second] = 0,
            [Magic.Level.Third] = 0,
            [Magic.Level.Fourth] = 0,
            [Magic.Level.Fifth] = 0,
            [Magic.Level.Sixth] = 0,
            [Magic.Level.Seventh] = 0,
            [Magic.Level.Eighth] = 0,
            [Magic.Level.Ninth] = 0
        };
        UsedASlot = false;
    }
}
