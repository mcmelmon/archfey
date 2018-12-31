using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objective : MonoBehaviour
{
    // Inspector settings
    public List<ObjectiveControlPoint> control_points;
    public Material ghaddim_skin;
    public Material mhoddim_skin;
    public Material unclaimed_skin;
    public List<Renderer> renderers;


    // properties

    public Conflict.Faction Control { get; set; }
    public bool Controlled { get; set; }
    public static float MinimumSpacing { get; set; }
    public GameObject NearestActor { get; set; }


    // static


    public static Objective Create(Objective prefab, Vector3 point, Objectives _objectives)
    {
        Vector3 random_facing = new Vector3(0, Random.Range(0, 7) * 30, 0);
        Objective _objective = Instantiate(prefab, point, _objectives.transform.rotation, _objectives.transform);
        _objective.transform.position += new Vector3(0, Geography.Terrain.SampleHeight(point), 0);
        _objective.transform.rotation = Quaternion.Euler(random_facing);
        _objective.SetComponents();

        return _objective;
    }

    // public


    public bool IsFriendlyTo(GameObject _unit)
    {
        if (!Controlled || _unit == null) return false;

        switch (Control)
        {
            case Conflict.Faction.Ghaddim:
                return _unit.GetComponent<Ghaddim>() != null;
            case Conflict.Faction.Mhoddim:
                return _unit.GetComponent<Mhoddim>() != null;
            default:
                return false;
        }
    }


    // private


    private IEnumerator CheckControl()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.action_threshold);

            Conflict.Faction new_faction = Conflict.Faction.None;
            Conflict.Faction previous_faction = Control;

            foreach (var control_point in control_points) {
                Conflict.Faction point_faction = control_point.Faction;

                if (new_faction == Conflict.Faction.None)
                    // We have just entered the loop
                    new_faction = point_faction;

                if ((point_faction == Conflict.Faction.None)) {
                    new_faction = Conflict.Faction.None;
                    break;
                } else if (new_faction != point_faction) {
                    new_faction = Conflict.Faction.None;
                    break;
                }
            }

            if (new_faction != previous_faction) {
                TransferControl(new_faction, previous_faction);
            }
        }
    }


    private void SetComponents()
    {
        Control = Conflict.Faction.None;
        Controlled = false;
        MinimumSpacing = 20f;
        StartCoroutine(CheckControl());
    }


    private void TransferControl(Conflict.Faction new_faction, Conflict.Faction previous_faction)
    {
        Controlled = true;
        Control = new_faction;

        switch (Control) {
            case Conflict.Faction.Ghaddim:
                GetComponentInChildren<Renderer>().material = ghaddim_skin;
                break;
            case Conflict.Faction.Mhoddim:
                GetComponentInChildren<Renderer>().material = mhoddim_skin;
                break;
            case Conflict.Faction.None:
                GetComponentInChildren<Renderer>().material = unclaimed_skin;
                break;
        }

        Objectives.Instance.AccountForControl(new_faction, previous_faction, this);
        RuinControlUI.Instance.ChangeInControl(Control, previous_faction);
        RuinControlUI.Instance.MostRecentFlip = this;
    }
}