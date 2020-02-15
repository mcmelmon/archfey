using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    // Inspector fields
    [SerializeField] int acquisition_turns;
    [SerializeField] List<Proficiencies.Tool> required_tools;

    // properties

    public int AcquisitionTurns { get; set; }
    public string Name { get; set; }
    public List<Proficiencies.Tool> RequiredTools { get; set; }

    // Unity

    private void Awake() {
        AcquisitionTurns = acquisition_turns;
        Name = this.name;
        RequiredTools = new List<Proficiencies.Tool>(required_tools);
    }


    // public

    public bool HarvestedBy(Actor _harvester)
    {
        if (!_harvester.IsEncumbered(GetComponent<Item>().GetAdjustedWeight())) {
            _harvester.Me.Inventory.AddToInventory(this.gameObject);
            _harvester.HasFullLoad = false;
            return true;
        }
        _harvester.HasFullLoad = true;
        return false;
    }

}
