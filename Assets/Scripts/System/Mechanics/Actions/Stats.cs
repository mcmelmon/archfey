using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    // Inspector settings

    public Slider health_bar;
    public Slider temporary_health_bar;
    public Slider rage_bar;
    public Transform stat_bars;

    // properties

    public List<string> ClassFeatures { get; set; }
    public Actor Me { get; set; }
    public string Family { get; set; }
    public string Size { get; set; }
    public int Level { get; set; }

    public int BaseArmorClass { get; set; } // TODO: build up AC from equipment and dex
    public Dictionary<Proficiencies.Attribute, int> AttributeAdjustments { get; set; }
    public Dictionary<Proficiencies.Attribute, int> BaseAttributes { get; set; }
    public List<Proficiencies.Skill> ExpertiseInSkills { get; set; }
    public List<Proficiencies.Tool> ExpertiseInTools { get; set; }
    public float RageDuration { get; set; }
    public float RageTick { get; set; }
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


    public int DamageAfterDefenses(int _damage, Weapons.DamageType _type)
    {
        return DamageAfterResistance(_damage, _type);
    }


    public int GetAdjustedAttributeScore(Proficiencies.Attribute attribute)
    {
        return Mathf.Clamp(BaseAttributes[attribute] + AttributeAdjustments[attribute], -5, 10);
    }


    public int GetArmorClass()
    {
        int armor_class = Mathf.Max(BaseArmorClass, Me.Actions.Attack.EquippedArmor.ArmorClass(GetAdjustedAttributeScore(Proficiencies.Attribute.Dexterity)));
        Armor shield = Me.Actions.Attack.EquippedOffhand?.GetComponent<Armor>();

        return shield != null ? armor_class + shield.ArmorClassEnhancement : armor_class;
    }


    public void UpdateStatBars()
    {
        if (rage_bar != null) {
            rage_bar.value = CurrentRagePercentage();
            if (rage_bar.value >= 1) {
                rage_bar.gameObject.SetActive(false);
            } else {
                rage_bar.gameObject.SetActive(true);
            }
        }

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


    public float CurrentRagePercentage()
    {
        return Me.Actions.Attack.Raging ? 1 - (RageTick / RageDuration) : 0;
    }


    private int DamageAfterResistance(int _damage, Weapons.DamageType _type)
    {
        return (_damage <= 0 || Resistances == null) ? _damage : (_damage -= _damage * (Resistances[_type] / 100));
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

        ClassFeatures = new List<string>();
        ExpertiseInSkills = new List<Proficiencies.Skill>();
        ExpertiseInTools = new List<Proficiencies.Tool>();
        Level = 1;
        RageDuration = 60;
        SavingThrows = new List<Proficiencies.Attribute>();
        Skills = new List<Proficiencies.Skill>();
        Tools = new List<Proficiencies.Tool>();

        AttributeAdjustments = new Dictionary<Proficiencies.Attribute, int>
        {
            [Proficiencies.Attribute.Charisma] = 0,
            [Proficiencies.Attribute.Constitution] = 0,
            [Proficiencies.Attribute.Dexterity] = 0,
            [Proficiencies.Attribute.Intelligence] = 0,
            [Proficiencies.Attribute.Strength] = 0,
            [Proficiencies.Attribute.Wisdom] = 0
        };

        BaseAttributes = new Dictionary<Proficiencies.Attribute, int>
        {
            [Proficiencies.Attribute.Charisma] = 0,
            [Proficiencies.Attribute.Constitution] = 0,
            [Proficiencies.Attribute.Dexterity] = 0,
            [Proficiencies.Attribute.Intelligence] = 0,
            [Proficiencies.Attribute.Strength] = 0,
            [Proficiencies.Attribute.Wisdom] = 0
        };
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
