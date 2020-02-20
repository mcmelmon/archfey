﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    public enum Alignments { LawfulGood, LawfulNeutral, LawfulEvil, NeutralGood, Neutral, NeutralEvil, ChaoticGood, ChaoticNeutral, ChaoticEvil }
    public enum Families { Abberation, Beast, Celestial, Construct, Dragon, Elemental, Fey, Fiend, Giant, Humanoid, Monstrosity, Ooze, Plant, Swarm, Undead };
    public enum Sizes { Tiny, Small, Medium, Large, Huge, Gargantuan }
    public enum Subfamilies { None, Dwarf, Elf, Gnoll, Gnome, Goblinoid, Grimlock, Human, Kobold, Lizardfolk, Merfolk, Orc, Sahuagin, Shapechanger };

    // Inspector settings

    public Slider health_bar;
    public Slider temporary_health_bar;
    public Transform stat_bars;

    [Header("Stat Block")]
    [Space(5)]
    [SerializeField] int strength;
    [SerializeField] int dexterity;
    [SerializeField] int constitution;
    [SerializeField] int intelligence;
    [SerializeField] int wisdom;
    [SerializeField] int charisma;

    [Space(10)]
    [SerializeField] Alignments alignment;
    [SerializeField] Families family;
    [SerializeField] Subfamilies subfamily;
    [SerializeField] int armor_class;
    [SerializeField] int hit_dice;
    [SerializeField] int hit_dice_type;
    [SerializeField] int speed = 30;
    [SerializeField] Sizes size;
    [SerializeField] int action_count = 1;
    [SerializeField] int proficiency_bonus = 2;
    [SerializeField] List<Proficiencies.Skill> skillset;
    [SerializeField] List<Proficiencies.Tool> toolset;



    // properties

    public Alignments Alignment { get; set; }
    public List<string> ClassFeatures { get; set; }
    public Actor Me { get; set; }
    public Families Family { get; set; }
    public Sizes Size { get; set; }
    public Subfamilies Subfamily { get; set; }
    public int Level { get; set; }

    public int BaseArmorClass { get; set; } // TODO: build up AC from equipment and dex
    public Dictionary<Proficiencies.Attribute, int> Attributes { get; set; }
    public Dictionary<Proficiencies.Attribute, int> AttributeAdjustments { get; set; }
    public List<Proficiencies.Skill> ExpertiseInSkills { get; set; }
    public List<Proficiencies.Tool> ExpertiseInTools { get; set; }
    public Dictionary<Weapons.DamageType, int> Resistances { get; set; }
    public int ProficiencyBonus { get; set; }
    public List<Proficiencies.Attribute> SavingThrows { get; set; }
    public List<Proficiencies.Skill> Skills { get; set; }
    public List<Proficiencies.Tool> Tools { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
        StartCoroutine(StatBarsFaceCamera());
        StartCoroutine(ManageStatBars());
    }

    private void Start() {
        SetDependentComponents();
    }


    private void OnValidate()
    {
        if (BaseArmorClass > 30) BaseArmorClass = 30;
        if (BaseArmorClass < 1) BaseArmorClass = 1;
    }


    // public


    public void AdjustAttribute(Proficiencies.Attribute attribute, int adjustment)
    {
        if (AttributeAdjustments[attribute] < adjustment) {
            AttributeAdjustments[attribute] = adjustment;
            // TODO: recalculate hit points and armor class if appropriate
        }
    }


    public float CarryingCapacity()
    {
        float encumbered_weight;

        switch (Size) {
            case Sizes.Tiny:
                encumbered_weight = GetAdjustedAttribute(Proficiencies.Attribute.Strength) * 7.5f;
                break;
            case Sizes.Small:
                encumbered_weight = GetAdjustedAttribute(Proficiencies.Attribute.Strength) * 15f;
                break;
            case Sizes.Medium:
                encumbered_weight = GetAdjustedAttribute(Proficiencies.Attribute.Strength) * 15f;
                break;
            case Sizes.Large:
                encumbered_weight = GetAdjustedAttribute(Proficiencies.Attribute.Strength) * 30f;
                break;
            case Sizes.Huge:
                encumbered_weight = GetAdjustedAttribute(Proficiencies.Attribute.Strength) * 60f;
                break;
            case Sizes.Gargantuan:
                encumbered_weight = GetAdjustedAttribute(Proficiencies.Attribute.Strength) * 120f;
                break;
            default:
                encumbered_weight = GetAdjustedAttribute(Proficiencies.Attribute.Strength) * 15f;
                break;
        }

        return encumbered_weight;
    }


    public int DamageAfterDefenses(int _damage, Weapons.DamageType _type)
    {
        return DamageAfterResistance(_damage, _type);
    }


    public float DragPushLiftCapacity()
    {
        return CarryingCapacity() * 2;
    }


    public int GetAdjustedAttribute(Proficiencies.Attribute attribute)
    {
        return Mathf.Clamp(Attributes[attribute] + AttributeAdjustments[attribute], 0, 30);
    }

    public int GetAdjustedAttributeModifier(Proficiencies.Attribute attribute)
    {
        return Mathf.Clamp((GetAdjustedAttribute(attribute) - 10)/2, -5, 10);
    }


    public int GetArmorClass()
    {
        int armor_class = Mathf.Max(BaseArmorClass, Me.Actions.Combat.EquippedArmor.ArmorClass(GetAdjustedAttributeModifier(Proficiencies.Attribute.Dexterity)));
        Armor shield = Me.Actions.Combat.EquippedOffhand?.GetComponent<Armor>();

        return shield != null ? armor_class + shield.ArmorClassEnhancement : armor_class;
    }


    public void UpdateStatBars()
    {
        if (health_bar != null) {
            health_bar.value = Me.Health.CurrentHealthPercentage();
            if (health_bar.value >= 1) {
                health_bar.gameObject.SetActive(false);
            } else {
                health_bar.gameObject.SetActive(true);
            }
        }

        if (temporary_health_bar != null) {
            temporary_health_bar.value = Me.Health.CurrentTemporaryHealthPercentage();
            if (temporary_health_bar.value < 0.05f) {
                temporary_health_bar.gameObject.SetActive(false);
            } else {
                temporary_health_bar.gameObject.SetActive(true);
            }
        }
    }


    // private


    private int DamageAfterResistance(int _damage, Weapons.DamageType _type)
    {
        return (_damage <= 0 || Resistances == null) ? _damage : (_damage = _damage * (Resistances[_type] / 100));
    }


    private void ManageResources()
    {
        //TODO: StartCoroutine(RegainSpells());
    }


    private IEnumerator ManageStatBars()
    {
        while (true) {
            if (Me.Health != null && Me.Health.MaximumHitPoints > 0) {
                UpdateStatBars();
            }

            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();

        Alignment = alignment;
        ClassFeatures = new List<string>();
        ExpertiseInSkills = new List<Proficiencies.Skill>();
        ExpertiseInTools = new List<Proficiencies.Tool>();
        Family = family;
        Level = 1;
        ProficiencyBonus = proficiency_bonus;
        SavingThrows = new List<Proficiencies.Attribute>();
        Size = size;
        Skills = new List<Proficiencies.Skill>(skillset);
        Subfamily = subfamily;
        Tools = new List<Proficiencies.Tool>(toolset);

        Attributes = new Dictionary<Proficiencies.Attribute, int>
        {
            [Proficiencies.Attribute.Charisma] = charisma,
            [Proficiencies.Attribute.Constitution] = constitution,
            [Proficiencies.Attribute.Dexterity] = dexterity,
            [Proficiencies.Attribute.Intelligence] = intelligence,
            [Proficiencies.Attribute.Strength] = strength,
            [Proficiencies.Attribute.Wisdom] = wisdom
        };

        AttributeAdjustments = new Dictionary<Proficiencies.Attribute, int>
        {
            [Proficiencies.Attribute.Charisma] = 0,
            [Proficiencies.Attribute.Constitution] = 0,
            [Proficiencies.Attribute.Dexterity] = 0,
            [Proficiencies.Attribute.Intelligence] = 0,
            [Proficiencies.Attribute.Strength] = 0,
            [Proficiencies.Attribute.Wisdom] = 0
        };
    }

    private void SetDependentComponents()
    {
        Me.Actions.Combat.AttacksPerAction = action_count;
        Me.Actions.Movement.BaseSpeed = speed / 10;
        Me.Actions.Movement.Agent.speed = speed / 10;
        switch (Size) {
            case Sizes.Tiny:
                Me.Actions.Movement.Agent.stoppingDistance = 1.5f;
                break;
            case Sizes.Small:
                Me.Actions.Movement.Agent.stoppingDistance = 2f;
                break;
            case Sizes.Medium:
                Me.Actions.Movement.Agent.stoppingDistance = 2.5f;
                break;
            case Sizes.Large:
                Me.Actions.Movement.Agent.stoppingDistance = 3f;
                break;
            case Sizes.Huge:
                Me.Actions.Movement.Agent.stoppingDistance = 3.5f;
                break;
            case Sizes.Gargantuan:
                Me.Actions.Movement.Agent.stoppingDistance = 4f;
                break;
            default:
                Me.Actions.Movement.Agent.stoppingDistance = 2.5f;
                break;
        }

        BaseArmorClass = armor_class;

        Me.Health.HitDice = hit_dice;
        Me.Health.HitDiceType = hit_dice_type;

        Me.Health.SetCurrentAndMaxHitPoints();
    }


    private IEnumerator StatBarsFaceCamera()
    {
        while (true)
        {
            if (Player.Instance != null) {
                Vector3 stats_position = transform.position;
                Vector3 player_position = Player.Instance.viewport.transform.position;

                Quaternion rotation = Quaternion.LookRotation(player_position - stats_position, Vector3.up);
                stat_bars.rotation = rotation;
            }

            yield return null;
        }
    }
}
