using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Magic : MonoBehaviour
{
    public enum CastingClass { Warlock }
    public enum Level { Cantrip = 0, First = 1, Second = 2, Third = 3, Fourth = 4, Fifth = 5, Sixth = 6, Seventh = 7, Eighth = 8, Ninth = 9 }
    public enum School { Abjuration, Conjuration, Divination, Enchantment, Evocation, Illusion, Necromancy, Transmutation }

    // properties

    public Actor Me { get; set; }
    public List<Spellcaster> Spellcasting { get; set; }
    public bool UsedASlot { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    // public

    public bool CanHeal()
    {
        foreach (var spellcaster in Spellcasting) {
            if (GetComponent<CureWounds>() != null && spellcaster.HaveSpellSlot(Level.First)) {
                return true;
            }
        }

        return false;
    }

    public void RecoverSpellSlots()
    {
        if (!UsedASlot) return;

        foreach (var spellcaster in Spellcasting) {
           foreach (Level _level in Enum.GetValues(typeof(Level))) {
                if (spellcaster.SpellsLeft[_level] < spellcaster.MaximumSpellSlots[_level]) {
                    spellcaster.SpellsLeft[_level] = spellcaster.MaximumSpellSlots[_level];
                    break;  // only recover from one depleted level per rest cyle, starting at First
                }
            }         
        }

        var used_levels = Spellcasting.Select(caster => caster.SpellsLeft.Where(sl => sl.Value < caster.MaximumSpellSlots[sl.Key])).ToList();

        UsedASlot &= used_levels.Count > 0;
    }

    // private

    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();
        UsedASlot = false;

        if (Me.GetComponents<Spellcaster>() != null) {
            Spellcasting = new List<Spellcaster>(Me.GetComponents<Spellcaster>());
        }
    }
}
