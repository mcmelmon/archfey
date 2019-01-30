using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{
    public enum Level { First, Second, Third, Fourth, Fifth, Sixth, Seventh, Eighth, Ninth };
    public enum School { Abjuration = 0, Conjuration = 1, Divination = 2, Enchantment = 3, Evocation = 4, Illusion = 5, Necromancy = 6, Transmutation = 7 };

    // properties

    public Dictionary<Level, int> SpellSlots { get; set; }
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


    public void UseSpellSlot(Level _level)
    {
        if (HaveSpellSlot(_level)) {
            SpellsLeft[_level]--;
        }
    }


    // private


    private void SetComponents()
    {
        SpellSlots = new Dictionary<Level, int>
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
