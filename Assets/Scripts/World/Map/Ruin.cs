using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ruin : MonoBehaviour
{
    // Inspector settings
    public RuinControlPoint control_point_prefab;
    public Material ghaddim_skin;
    public Material mhoddim_skin;
    public Material unclaimed_skin;

    // properties

    public Conflict.Faction Control { get; set; }
    public bool Controlled { get; set; }
    public List<RuinControlPoint> ControlPoints { get; set; }
    public static float MinimumRuinSpacing { get; set; }
    public GameObject NearestActor { get; set; }


    // static


    public static Ruin InstantiateRuin(Ruin prefab, Vector3 point, Ruins _ruins)
    {
        Ruin _ruin = Instantiate(prefab, point, _ruins.transform.rotation, _ruins.transform);
        _ruin.transform.localScale += new Vector3(4, 16, 4);
        _ruin.transform.position += new Vector3(0, _ruin.transform.localScale.y / 2, 0);
        _ruin.SetComponents();

        return _ruin;
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

            foreach (var control_point in ControlPoints) {
                RuinControlPoint _point = control_point.GetComponent<RuinControlPoint>();
                Conflict.Faction point_faction = _point.Faction;

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
        ControlPoints = new List<RuinControlPoint>();
        MinimumRuinSpacing = 20f;
        SetControlPoints();
        StartCoroutine(CheckControl());
    }


    private void SetControlPoints()
    {
        Circle _center = Circle.CreateCircle(new Vector3 (transform.position.x, 0, transform.position.z), 10f, 3);

        foreach (var vertex in _center.vertices) {
            RuinControlPoint control_point = Instantiate(control_point_prefab, vertex + new Vector3(0,1,0), transform.rotation);
            control_point.Ruin = this;
            control_point.transform.parent = transform; // using the Ruin transform scales the point out
            ControlPoints.Add(control_point);
            Ruins.RuinControlPoints.Add(control_point);
        }
    }


    private void TransferControl(Conflict.Faction new_faction, Conflict.Faction previous_faction)
    {
        Controlled = true;
        Control = new_faction;

        switch (Control) {
            case Conflict.Faction.Ghaddim:
                GetComponent<Renderer>().material = ghaddim_skin;
                break;
            case Conflict.Faction.Mhoddim:
                GetComponent<Renderer>().material = mhoddim_skin;
                break;
            case Conflict.Faction.None:
                GetComponent<Renderer>().material = unclaimed_skin;
                break;
        }

        Ruins.Instance.AccountForControl(new_faction, previous_faction, this);
        RuinControlUI.Instance.ChangeInControl(Control, previous_faction);
        RuinControlUI.Instance.MostRecentFlip = this;
    }
}