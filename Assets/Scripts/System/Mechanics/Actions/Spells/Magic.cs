using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Magic : MonoBehaviour
{
    public enum Level { First = 1, Second = 2, Third = 3, Fourth = 4, Fifth = 5, Sixth = 6, Seventh = 7, Eighth = 8, Ninth = 9 };
    public enum School { Abjuration, Conjuration, Divination, Enchantment, Evocation, Illusion, Necromancy, Transmutation };

    // properties

    public Dictionary<Level, int> MaximumSpellSlots { get; set; }
    public Dictionary<Level, int> SpellsLeft { get; set; }
    public bool UsedSlot { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }


    // public

    public bool HaveSpellSlot(Level _level)
    {
        return SpellsLeft[_level] > 0;
    }


    public void RecoverSpellLevels()
    {
        if (!UsedSlot) return;

        foreach (Level _level in Enum.GetValues(typeof(Level))) {
            if (SpellsLeft[_level] < MaximumSpellSlots[_level]) {
                SpellsLeft[_level] = MaximumSpellSlots[_level];
                break;  // only recover from one depleted level per rest cyle, starting at First
            }
        }
        var used_levels = SpellsLeft.Where(sl => sl.Value < MaximumSpellSlots[sl.Key]).ToList();

        UsedSlot &= used_levels.Count > 0;
    }


    public void UseSpellSlot(Level _level)
    {
        if (HaveSpellSlot(_level)) {
            SpellsLeft[_level]--;
        }
    }


    // private


    private void SetComponents()
    {
        MaximumSpellSlots = new Dictionary<Level, int>
        {
            [Level.First] = 0,
            [Level.Second] = 0,
            [Level.Third] = 0,
            [Level.Fourth] = 0,
            [Level.Fifth] = 0,
            [Level.Sixth] = 0,
            [Level.Seventh] = 0,
            [Level.Eighth] = 0,
            [Level.Ninth] = 0
        };
        SpellsLeft = new Dictionary<Level, int>
        {
            [Level.First] = 0,
            [Level.Second] = 0,
            [Level.Third] = 0,
            [Level.Fourth] = 0,
            [Level.Fifth] = 0,
            [Level.Sixth] = 0,
            [Level.Seventh] = 0,
            [Level.Eighth] = 0,
            [Level.Ninth] = 0
        };
        UsedSlot = false;
    }
}
