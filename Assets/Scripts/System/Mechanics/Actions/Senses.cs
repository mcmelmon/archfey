﻿ using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Senses : MonoBehaviour
{

    // properties

    public Actor Me { get; set; }
    public bool Darkvision { get; set; }
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


    public bool InsightCheck(bool active_check, int challenge_rating, bool obscurity = false, bool advantage = false, bool disadvantage = false)
    {
        return Me.Actions.SkillCheck(active_check, Proficiencies.Skill.Insight) >= challenge_rating;
    }


    public bool InvestigationCheck(bool active_check, int challenge_rating, bool obscurity = false, bool advantage = false, bool disadvantage = false)
    {
        return Me.Actions.SkillCheck(active_check, Proficiencies.Skill.Investigation) >= challenge_rating;
    }


    public bool PerceptionCheck(bool active_check, int challenge_rating, bool obscurity = false, bool advantage = false, bool disadvantage = false)
    {
        return Me.Actions.SkillCheck(active_check, Proficiencies.Skill.Perception) >= challenge_rating;
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
        TriggerInsights();

        Me.Actions.Attack.SetEnemyRanges();
    }


    // private


    private void RemoveHidden()
    {
        List<Actor> the_sneaking = Actors.Where(actor => actor.Actions.Stealth.IsHiding && actor != Me).ToList();
        foreach (var sneaker in the_sneaking) {
            bool spotted = PerceptionCheck(false, sneaker.Actions.Stealth.StealthChallengeRating) || InvestigationCheck(false, sneaker.Actions.Stealth.StealthChallengeRating);
            if (spotted && (Me == Player.Instance.Me || sneaker == Player.Instance.Me)) {
                sneaker.Actions.Stealth.StopHiding();
            } else {
                Actors.Remove(sneaker);
            }
        }

        List<Item> the_hidden = Items.Where(item => item.is_hidden).ToList();
        foreach (var hidden in the_hidden) {
            bool spotted = PerceptionCheck(false, hidden.spot_challenge_rating) || InvestigationCheck(false, hidden.spot_challenge_rating);
            if (spotted) {
                hidden.IsSpotted = true;
                hidden.GetComponent<Renderer>().material = hidden.GetComponent<Interactable>().highlight_material;
            } else { 
                Items.Remove(hidden); 
            }
        }
    }


    private void TriggerInsights()
    {
        List<Actor> suspects = Actors.Where(actor => actor.Interactions.is_suspicious && actor.Interactions.relevant_skill == Proficiencies.Skill.Insight).ToList();
        foreach (var suspect in suspects){
            bool suspicious = InsightCheck(false, suspect.Interactions.suspicion_challenge_rating) || InvestigationCheck(false, suspect.Interactions.suspicion_challenge_rating);
            if (suspicious) suspect.Interactions.DrawAttention();
        }
    }


    private void SetComponents()
    {
        Actors = new List<Actor>();
        Me = GetComponent<Actor>();
        PerceptionRange = 20f;
        Structures = new List<Structure>();
    }
}