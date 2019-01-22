using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Proficiencies : MonoBehaviour
{

    public enum Attribute { Charisma = 0, Dexterity = 1, Constitution = 2, Intelligence = 3, Strength = 4, Wisdom = 5 };
    public enum Skill { 
        Acrobatics = 0,
        AnimalHandling = 1,
        Arcana = 2,
        Athletics = 3,
        Deception = 4,
        History = 5,
        Insight = 6,
        Intimidation = 7,
        Investigation = 8,
        Medicine = 9,
        Nature = 10,
        Perception = 11,
        Performance = 12,
        Persuasion = 13,
        Religion = 14,
        SleightOfHand = 15,
        Stealth = 16,
        Survival = 17
    };

    public struct SkillAttribute
    {
        public Skill skill;
        public Attribute attribute;

        public SkillAttribute(Skill _skill, Attribute _attribute)
        {
            this.skill = _skill;
            this.attribute = _attribute;
        }
    }

    public struct ToolSynergy
    {
        public string tool;
        public List<Skill> synergistic_skills;

        public ToolSynergy(string _tool, List<Skill> _skills)
        {
            this.tool = _tool;
            this.synergistic_skills = _skills;
        }
    }

    // properties

    public static Proficiencies Instance { get; set; }
    public static List<SkillAttribute> SkillAttributes { get; set; }
    public static List<ToolSynergy> ToolSynergies { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one proficiencies instance!");
            Destroy(this);
            return;
        }
        Instance = this;
        SetComponents();
    }


    // public


    public bool Artisan(Actor _unit)
    {
        var my_crafting_tools = _unit.Stats.Tools.Where(t => t == "Carpenter").ToList();
        return my_crafting_tools.Count > 0;
    }


    public bool Harvester(Actor _unit)
    {
        var my_harvesting_tools = _unit.Stats.Tools.Where(t => t == "Farmer" || t == "Lumberjack" || t == "Miner").ToList();
        return my_harvesting_tools.Count > 0;
    }


    // private


    private void SetComponents()
    {
        SkillAttributes = new List<SkillAttribute>()
        {
            new SkillAttribute(Skill.Acrobatics, Attribute.Dexterity),
            new SkillAttribute(Skill.AnimalHandling, Attribute.Wisdom),
            new SkillAttribute(Skill.Arcana, Attribute.Intelligence),
            new SkillAttribute(Skill.Athletics, Attribute.Strength),
            new SkillAttribute(Skill.Deception, Attribute.Charisma),
            new SkillAttribute(Skill.History, Attribute.Intelligence),
            new SkillAttribute(Skill.Insight, Attribute.Wisdom),
            new SkillAttribute(Skill.Intimidation, Attribute.Charisma),
            new SkillAttribute(Skill.Investigation, Attribute.Intelligence),
            new SkillAttribute(Skill.Medicine, Attribute.Wisdom),
            new SkillAttribute(Skill.Nature, Attribute.Wisdom),
            new SkillAttribute(Skill.Perception, Attribute.Wisdom),
            new SkillAttribute(Skill.Performance, Attribute.Charisma),
            new SkillAttribute(Skill.Persuasion, Attribute.Charisma),
            new SkillAttribute(Skill.Religion, Attribute.Intelligence),
            new SkillAttribute(Skill.SleightOfHand, Attribute.Dexterity),
            new SkillAttribute(Skill.Stealth, Attribute.Dexterity),
            new SkillAttribute(Skill.Survival, Attribute.Wisdom)
        };

        ToolSynergies = new List<ToolSynergy>()
        {
            new ToolSynergy("Farmer", new List<Skill>{ Skill.AnimalHandling, Skill.Insight, Skill.Nature, Skill.Survival }),
            new ToolSynergy("Lumberjack", new List<Skill>{ Skill.Investigation, Skill.Perception, Skill.Survival }),
            new ToolSynergy("Miner", new List<Skill>{ Skill.Arcana, Skill.History, Skill.Nature }),
        };
    }
}
