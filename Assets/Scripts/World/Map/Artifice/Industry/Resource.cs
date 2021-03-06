﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    // Inspector fields
    [SerializeField] Resources.Category category = Resources.Category.None;
    [SerializeField] Proficiencies.Tool required_tool = Proficiencies.Tool.Alchemist;
    [SerializeField] Proficiencies.Attribute required_attribute = Proficiencies.Attribute.None;
    [SerializeField] Proficiencies.Skill required_skill = Proficiencies.Skill.None;
    [SerializeField] int challenge_rating = 5;


    // properties

    public Item Item { get; set; }
    public string Name { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    // public

    public bool HarvestBy(Actor _harvester)
    {
        if (!IsAccessibleTo(_harvester)) return false;

        if (required_attribute != Proficiencies.Attribute.None) {
            if (_harvester.Actions.AttributeCheck(true, required_attribute) <= challenge_rating) {
                return false;
            }
        } else if (required_skill != Proficiencies.Skill.None) {
            if (_harvester.Actions.SkillCheck(true, required_skill) <= challenge_rating) {
                return false;
            }
        }

        if (_harvester.Actions.ToolCheck(required_tool) >= challenge_rating) {
            if (!_harvester.Actions.Movement.Encumbered) {
                _harvester.Me.Inventory.AddToInventory(this.gameObject);
                _harvester.HasFullLoad = false;
                return true;
            } else {
                _harvester.HasFullLoad = true;
                return false;
            }
        }

        return false;
    }

    public bool IsAccessibleTo(Actor _harvester)
    {
        return _harvester.Stats.Tools.Contains(required_tool);

    }

    // private

    private void SetComponents()
    {
        Item = GetComponent<Item>();
        Name = this.name;
    }
}
