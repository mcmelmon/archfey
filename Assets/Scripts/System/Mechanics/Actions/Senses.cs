using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Senses : MonoBehaviour
{

    // properties

    public Actor Me { get; set; }
    public float Darkvision { get; set; }
    public float PerceptionRange { get; set; }
    public List<Actor> Actors { get; set; }
    public List<Item> Items { get; set; }
    public List<Structure> Structures { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public bool PerceptionCheck(bool active_check, int challenge_rating, bool obscurity = false, bool advantage = false, bool disadvantage = false)
    {
        int proficiency_bonus = Me.Stats.Skills.Contains(Proficiencies.Skill.Perception) ? Me.Stats.ProficiencyBonus : 0;
        if (Me.Stats.Expertise.Contains(Proficiencies.Skill.Perception)) proficiency_bonus += proficiency_bonus;
        int attribute_bonus = Mathf.Max(Me.Stats.AttributeProficiency[Proficiencies.Attribute.Wisdom], Me.Stats.AttributeProficiency[Proficiencies.Attribute.Intelligence]);
        int bonus = proficiency_bonus + attribute_bonus;
        if (obscurity) bonus -= 5;

        int die_roll = Me.Actions.RollDie(20, 1, advantage, disadvantage);

        return die_roll + bonus > challenge_rating;
    }


    public void SetRange(float _range)
    {
        PerceptionRange = _range;
    }


    public void Sense()
    {
        Actors = FindObjectsOfType<Actor>()
            .Where(actor => Vector3.Distance(transform.position, actor.transform.position) < PerceptionRange)
            .Select(collider => collider.gameObject.GetComponent<Actor>()).OfType<Actor>().Distinct().ToList();
        Items = FindObjectsOfType<Item>()
            .Where(item => Vector3.Distance(transform.position, item.transform.position) < PerceptionRange)
            .Select(collider => collider.gameObject.GetComponent<Item>()).OfType<Item>().Distinct().ToList();
        Structures = FindObjectsOfType<Structure>()
            .Where(structure => Vector3.Distance(transform.position, structure.transform.position) < PerceptionRange)
            .Select(collider => collider.gameObject.GetComponent<Structure>()).OfType<Structure>().Distinct().ToList();

        RemoveHidden();

        Me.Actions.Attack.SetEnemyRanges();
    }


    // private


    private void RemoveHidden()
    {
        List<Actor> the_sneaking = Actors.Where(actor => actor.Actions.Stealth.Hiding).ToList();
        foreach (var sneaker in the_sneaking) {
            bool spotted = Me.Senses.PerceptionCheck(false, sneaker.Actions.Stealth.ChallengeRatting);  // TODO: include environmental detail for obscurity
            if (!spotted) Actors.Remove(sneaker);
        }

        List<Item> the_hidden = Items.Where(item => item.is_hidden).ToList();
        foreach (var hidden in the_hidden) {
            bool spotted = Me.Senses.PerceptionCheck(false, hidden.spot_challenge_rating);  // TODO: include environmental detail for obscurity
            if (spotted) {
                hidden.IsSpotted = true;
                hidden.GetComponent<Renderer>().material = hidden.GetComponent<Interactable>().highlight_material;
            } else { 
                Items.Remove(hidden); 
            }
        }
    }


    private void SetComponents()
    {
        Actors = new List<Actor>();
        Me = GetComponent<Actor>();
        Structures = new List<Structure>();
    }
}