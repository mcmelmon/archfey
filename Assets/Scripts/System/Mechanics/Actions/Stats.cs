using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    // Inspector settings

    public Slider health_bar;
    public Slider mana_bar;
    public Transform stat_bars;

    // properties

    public Actor Me { get; set; }
    public string Family { get; set; }
    public string Size { get; set; }
    public int Level { get; set; }

    public int ArmorClass { get; set; }
    public Dictionary<Proficiencies.Attribute, int> AttributeProficiency { get; set; }
    public int DefenseRating { get; set; }
    public Dictionary<Weapons.DamageType, int> Resistances { get; set; }
    public int ProficiencyBonus { get; set; }
    public List<Proficiencies.Skill> Expertise { get; set; }
    public List<Proficiencies.Skill> Skills { get; set; }
    public List<string> Tools { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
        StartCoroutine(StatBarsFaceCamera());
        StartCoroutine(ManageStatBars());
    }

    private void OnValidate()
    {
        if (ArmorClass > 30) ArmorClass = 30;
        if (ArmorClass < 1) ArmorClass = 1;
    }


    // public


    public int DamageAfterDefenses(int _damage, Weapons.DamageType _type)
    {
        return DamageAfterResistance(_damage, _type);
    }


    public void UpdateStatBars()
    {
        if (mana_bar != null)
        {
            mana_bar.value = CurrentManaPercentage();
            if (mana_bar.value >= 1)
            {
                mana_bar.gameObject.SetActive(false);
            }
            else
            {
                mana_bar.gameObject.SetActive(true);
            }
        }

        if (health_bar != null)
        {
            health_bar.value = Me.Health.CurrentHealthPercentage();
            if (health_bar.value >= 1)
            {
                health_bar.gameObject.SetActive(false);
            }
            else
            {
                health_bar.gameObject.SetActive(true);
            }
        }
    }


    // private


    public float CurrentManaPercentage()
    {
        return 1;
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
        while (!Conflict.Victory && Me.Health.MaximumHitPoints > 0)
        {
            UpdateStatBars();
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();

        Expertise = new List<Proficiencies.Skill>();
        Level = 1;
        Skills = new List<Proficiencies.Skill>();
        Tools = new List<string>();

        AttributeProficiency = new Dictionary<Proficiencies.Attribute, int>
        {
            [Proficiencies.Attribute.Charisma] = 0,
            [Proficiencies.Attribute.Constitution] = 0,
            [Proficiencies.Attribute.Dexterity] = 0,
            [Proficiencies.Attribute.Intelligence] = 0,
            [Proficiencies.Attribute.Strength] = 0,
            [Proficiencies.Attribute.Wisdom] = 0
        };

        DefenseRating = ArmorClass + Me.Stats.AttributeProficiency[Proficiencies.Attribute.Dexterity];
    }


    private IEnumerator StatBarsFaceCamera()
    {
        while (true)
        {
            Vector3 stats_position = transform.position;
            Vector3 player_position = Player.Instance.viewport.transform.position;

            Quaternion rotation = Quaternion.LookRotation(player_position - stats_position, Vector3.up);
            stat_bars.rotation = rotation;

            yield return null;
        }
    }
}
