using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    // Inspector fields
    [SerializeField] int acquisition_turns;
    [SerializeField] List<Proficiencies.Tool> required_tools;
    [SerializeField] int value_cp;
    [SerializeField] float weight;

    // properties

    public int AcquisitionTurns { get; set; }
    public string Name { get; set; }
    public List<Proficiencies.Tool> RequiredTools { get; set; }

    public int ValueInCopper { get; set; }
    public float Weight { get; set; }

    // Unity

    private void Awake() {
        AcquisitionTurns = acquisition_turns;
        Name = this.name;
        RequiredTools = new List<Proficiencies.Tool>(required_tools);
        ValueInCopper = value_cp;
        Weight = weight;
    }


    // public

    public bool HarvestedBy(Actor _harvester)
    {
        if (!_harvester.IsEncumbered(Weight)) {
            if (_harvester.Load.ContainsKey(this)) {
                _harvester.Load[this] += 1;
            } else {
                _harvester.Load[this] = 1;
            }

            return true;
        }

        return false;
    }

}
