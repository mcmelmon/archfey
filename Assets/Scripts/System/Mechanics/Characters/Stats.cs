using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    public enum Alignments { LawfulGood, LawfulNeutral, LawfulEvil, NeutralGood, Neutral, NeutralEvil, ChaoticGood, ChaoticNeutral, ChaoticEvil }
    public enum Families { Abberation, Beast, Celestial, Construct, Dragon, Elemental, Fey, Fiend, Giant, Humanoid, Monstrosity, Ooze, Plant, Swarm, Undead };
    public enum ResistanceLevels { None, Resistant, Immune, Vulnerable }
    public enum Sizes { Tiny, Small, Medium, Large, Huge, Gargantuan }
    public enum Subfamilies { None, Dwarf, Elf, Gnoll, Gnome, Goblinoid, Grimlock, Human, Kobold, Lizardfolk, Merfolk, Orc, Sahuagin, Shapechanger };

    // Inspector settings

    public Slider health_bar;
    public Slider temporary_health_bar;
    public Transform stat_bars;

    [Header("Stat Block")]
    [Space(5)]
    [SerializeField] int strength = 10;
    [SerializeField] int dexterity = 10;
    [SerializeField] int constitution = 10;
    [SerializeField] int intelligence = 10;
    [SerializeField] int wisdom = 10;
    [SerializeField] int charisma = 10;

    [Space(10)]
    [SerializeField] Alignments alignment = Alignments.Neutral;
    [SerializeField] Families family = Families.Humanoid;
    [SerializeField] Subfamilies subfamily = Subfamilies.Human;
    [SerializeField] int armor_class = 10;
    [SerializeField] int hit_dice = 1;
    [SerializeField] int hit_dice_type = 6;
    [SerializeField] int speed = 30;
    [SerializeField] Sizes size = Sizes.Medium;
    [SerializeField] int action_count = 1;
    [SerializeField] int proficiency_bonus = 2;
    [SerializeField] List<Proficiencies.Armor> armor = new List<Proficiencies.Armor>();
    [SerializeField] List<Proficiencies.Attribute> saving_throws = new List<Proficiencies.Attribute>();
    [SerializeField] List<Proficiencies.Skill> skillset = new List<Proficiencies.Skill>();
    [SerializeField] List<Proficiencies.Tool> toolset = new List<Proficiencies.Tool>();
    [SerializeField] List<Proficiencies.Weapon> weapons = new List<Proficiencies.Weapon>();



    // properties

    public Actor Me { get; set; }
    public Alignments Alignment { get; set; }
    public List<Proficiencies.Armor> ArmorProficiencies { get; set; }
    public Dictionary<Proficiencies.Attribute, int> Attributes { get; set; }
    public Dictionary<Proficiencies.Attribute, int> AttributeAdjustments { get; set; }
    public int BaseArmorClass { get; set; } // TODO: build up AC from equipment and dex
    public List<Proficiencies.Skill> ExpertiseInSkills { get; set; }
    public List<Proficiencies.Tool> ExpertiseInTools { get; set; }
    public Families Family { get; set; }
    public Dictionary<Weapons.DamageType, ResistanceLevels> Resistances { get; set; }
    public int ProficiencyBonus { get; set; }
    public List<Proficiencies.Attribute> SavingThrows { get; set; }
    public Sizes Size { get; set; }
    public int Speed { get; set; }
    public Subfamilies Subfamily { get; set; }
    public List<Proficiencies.Skill> Skills { get; set; }
    public List<Proficiencies.Tool> Tools { get; set; }
    public List<Proficiencies.Weapon> WeaponProficiencies { get; set; }


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
        int damage_taken = _damage;

        switch(Resistances[_type]) {
            case ResistanceLevels.Resistant:
                damage_taken = _damage / 2;
                break;
            case ResistanceLevels.Immune:
                damage_taken = 0;
                break;
            case ResistanceLevels.Vulnerable:
                damage_taken = _damage * 2;
                break;
            default:
                break;
        }

        return damage_taken;
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
        ArmorProficiencies = new List<Proficiencies.Armor>(armor);
        ExpertiseInSkills = new List<Proficiencies.Skill>();
        ExpertiseInTools = new List<Proficiencies.Tool>();
        Family = family;
        ProficiencyBonus = proficiency_bonus;
        SavingThrows = new List<Proficiencies.Attribute>(saving_throws);
        Size = size;
        Skills = new List<Proficiencies.Skill>(skillset);
        Speed = speed;
        Subfamily = subfamily;
        Tools = new List<Proficiencies.Tool>(toolset);
        WeaponProficiencies = new List<Proficiencies.Weapon>(weapons);

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

        Resistances = new Dictionary<Weapons.DamageType, ResistanceLevels> {
            [Weapons.DamageType.Acid] = ResistanceLevels.None,
            [Weapons.DamageType.Bludgeoning] = ResistanceLevels.None,
            [Weapons.DamageType.Cold] = ResistanceLevels.None,
            [Weapons.DamageType.Fire] = ResistanceLevels.None,
            [Weapons.DamageType.Force] = ResistanceLevels.None,
            [Weapons.DamageType.Lightning] = ResistanceLevels.None,
            [Weapons.DamageType.Necrotic] = ResistanceLevels.None,
            [Weapons.DamageType.Piercing] = ResistanceLevels.None,
            [Weapons.DamageType.Poison] = ResistanceLevels.None,
            [Weapons.DamageType.Psychic] = ResistanceLevels.None,
            [Weapons.DamageType.Radiant] = ResistanceLevels.None,
            [Weapons.DamageType.Slashing] = ResistanceLevels.None,
            [Weapons.DamageType.Thunder] = ResistanceLevels.None
        };
    }

    private void SetDependentComponents()
    {
        if (Me.Actions != null) Me.Actions.Combat.AttacksPerAction = action_count;

        BaseArmorClass = armor_class;

        Me.Health.AddHitDice(hit_dice_type, hit_dice);
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
