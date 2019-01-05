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
    public Conflict.Faction initial_control;


    // properties

    public Conflict.Faction Control { get; set; }
    public bool Controlled { get; set; }
    public GameObject NearestActor { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
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
                Conflict.Faction point_faction = control_point.ControllingFaction;

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
        Control = initial_control;
        Controlled = (Control != Conflict.Faction.None);
        StartCoroutine(CheckControl());

        foreach (var rend in renderers) {
            rend.material = (initial_control == Conflict.Faction.None) ? unclaimed_skin : (initial_control == Conflict.Faction.Ghaddim) ? ghaddim_skin : mhoddim_skin;
        }
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
        ObjectiveControlUI.Instance.ChangeInControl(Control, previous_faction);
        ObjectiveControlUI.Instance.MostRecentFlip = this;
    }
}