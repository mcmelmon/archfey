using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warlock : MonoBehaviour
{
    public enum Boons { Blade, Book, Chain }
    public enum EldritchInvocations { 
        AgonizingBlast = 0,
        ArmorOfShadows = 2,
        AscendantStep = 3,
        BeastSpeech = 4,
        BeguilingInfluence = 5
    }

    public enum Pacts { Archfey }

    // properties

    public Actor Me { get; set; }
    public Boons Boon { get; set; }
    public List<EldritchInvocations> Invocations { get; set; }
    public int Level { get; set; }
    public Dictionary<Magic.Level, GameObject> MysticArcanum { get; set; }
    public Pacts Pact { get; set; }
    public List<Proficiencies.Skill> Skills { get; set; }
    public Spellcaster Spellcaster { get; set; }
    public Magic.Level SpellSlotLevel { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    private void Start() {
        SetAdditionalComponents();
    }

    // private

    private Magic.Level GetSpellSlotLevel()
    {
        switch (Level) {
            case int n when (n > 0 && n < 3):
                return Magic.Level.First;
            case int n when (n >= 3 && n < 5):
                return Magic.Level.Second;
            case int n when (n >= 5 && n < 7):
                return Magic.Level.Third;
            case int n when (n >= 7 && n < 9):
                return Magic.Level.Fourth;
            case int n when (n >= 9):
                return Magic.Level.Fifth;
            default:
                return Magic.Level.First;
        }
    }

    private void SetAdditionalComponents()
    {
        Me.Health.AddHitDice(8, Level);
        Me.Stats.ArmorProficiencies.Add(Proficiencies.Armor.Light);
        Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Charisma);
        Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Constitution);
        Me.Actions.Magic.gameObject.AddComponent<EldritchBlast>();
    }

    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        Invocations = new List<EldritchInvocations>();
        Level = 1;
        MysticArcanum = new Dictionary<Magic.Level, GameObject>();
        Pact = Pacts.Archfey;
        Spellcaster = Me.gameObject.AddComponent<Spellcaster>();
        Spellcaster.CastingAttribute = Proficiencies.Attribute.Charisma;
        Spellcaster.CastingClass = Magic.CastingClass.Warlock;
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
