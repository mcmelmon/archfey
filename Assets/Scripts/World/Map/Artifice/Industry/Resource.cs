using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    // Inspector fields
    [SerializeField] Resources.Category category;
    [SerializeField] Proficiencies.Tool required_tool;
    [SerializeField] Proficiencies.Attribute required_attribute;
    [SerializeField] Proficiencies.Skill required_skill;
    [SerializeField] int challenge_rating;


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
        if (Name == null || Name == "") Debug.Log("Blank name in category: " + category + "; CR: " + challenge_rating);
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
                Debug.Log("Harvested: " + Name);
                return true;
            } else {
                Debug.Log("Encumbered");
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
