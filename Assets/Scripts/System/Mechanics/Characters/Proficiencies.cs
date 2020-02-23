using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Proficiencies : MonoBehaviour
{
    public enum Armor { Light, Medium, Heavy, Shield }

    public enum Attribute { Charisma, Dexterity, Constitution, Intelligence, Strength, Wisdom, None }

    public enum Skill { 
        Acrobatics,
        AnimalHandling,
        Arcana,
        Athletics,
        Deception,
        History,
        Insight,
        Intimidation,
        Investigation,
        Medicine,
        Nature,
        Perception,
        Performance,
        Persuasion,
        Religion,
        SleightOfHand,
        Stealth,
        Survival,
        None
    }

    public enum Tool {
        Alchemist,
        Brewer,
        Calligrapher,
        Carpenter,
        Cartographer,
        Cobbler,
        Cook,
        Disguise,
        Farmer,
        Forger,
        Glassblower,
        Herbalist,
        Jeweler,
        Leatherworker,
        Lumberjack,
        Mason,
        Miner,
        Painter,
        Poisoner,
        Potter,
        Skinner,
        Smith,
        Thief,
        Tinker,
        Vehicle,
        Weaver
    }

    public enum Weapon { Simple, Martial }

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

    public List<Tool> GatheringTools { get; set; }
    public static Proficiencies Instance { get; set; }
    public static List<SkillAttribute> SkillAttributes { get; set; }

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

    public Attribute GetAttributeForSkill(Skill skill)
    {
        return SkillAttributes.First(sa => sa.skill == skill).attribute;
    }


    public bool IsArtisan(Actor _unit)
    {
        List<Tool> my_crafting_tools = _unit.Stats.Tools.Where(tool => !GatheringTools.Contains(tool)).ToList();
        return my_crafting_tools.Count > 0;
    }


    public bool IsHarvester(Actor _unit)
    {
        List<Tool> my_harvesting_tools = _unit.Stats.Tools.Where(tool => GatheringTools.Contains(tool)).ToList();
        return my_harvesting_tools.Count > 0;
    }


    // private


    private void SetComponents()
    {
        GatheringTools = new List<Tool> { Tool.Herbalist, Tool.Lumberjack, Tool.Miner, Tool.Skinner };
        SkillAttributes = new List<SkillAttribute>() {
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
    }
}
