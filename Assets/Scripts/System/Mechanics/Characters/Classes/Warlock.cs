using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warlock : MonoBehaviour
{
    public enum Boons { Blade, Book, Chain }
    public enum Pacts { Archfey }

    // properties

    public Actor Me { get; set; }
    public Boons Boon { get; set; }
    public int Level { get; set; }
    public Pacts Pact { get; set; }
    public List<Proficiencies.Skill> Skills { get; set; }
    public Spellcaster Spellcaster { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    private void Start() {
        SetAdditionalComponents();
    }

    // private

    private void SetAdditionalComponents()
    {
        Me.Health.AddHitDice(8, Level);
        Me.Stats.ArmorProficiencies.Add(Proficiencies.Armor.Light);
        Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Charisma);
        Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Constitution);
    }

    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        Level = 1;
        Pact = Pacts.Archfey;
        Spellcaster = Me.gameObject.AddComponent<Spellcaster>();
        Spellcaster.CastingAttribute = Proficiencies.Attribute.Charisma;
        Spellcaster.Level = Level;

        Skills = new List<Proficiencies.Skill> {
            Proficiencies.Skill.Arcana,
            Proficiencies.Skill.Deception,
            Proficiencies.Skill.History,
            Proficiencies.Skill.Intimidation,
            Proficiencies.Skill.Investigation,
            Proficiencies.Skill.Nature,
            Proficiencies.Skill.Religion
        };
    }
}
