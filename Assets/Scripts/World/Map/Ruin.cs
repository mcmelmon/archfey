using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin : MonoBehaviour {

    public static float minimum_ruin_proximity = 15f;

    public float control_radius = 10f;  // how close must a unit be to gain control?
    public float control_resistance_rating;  // what is the difficulty of controlling this ruin?
    public float current_control_points;
    public bool is_controlled;
    public float starting_control_points = 10f;  // TODO: configure
    public Material ghaddim_skin;
    public Material mhoddim_skin;

    List<GameObject> contenders = new List<GameObject>();
    GameObject controller;
    SphereCollider control_zone;

    // Unity


    private void Awake()
    {
        control_zone = gameObject.AddComponent<SphereCollider>();  // use this collider for supporting controlling units
        control_zone.radius = control_radius / transform.localScale.y;
        control_zone.isTrigger = true;
        control_resistance_rating = 0.5f;
        current_control_points = starting_control_points;
        is_controlled = false;
    }


    // public


    public void ExertControl(GameObject _unit, float ruin_control_rating)
    {
        current_control_points -= starting_control_points * ruin_control_rating * control_resistance_rating;
        if (current_control_points <= 0) TransferControl(_unit);
    }


    public bool IsFriendlyTo(GameObject _unit)
    {
        if (!is_controlled || _unit == null) return false;

        Ghaddim controller_ghaddim = controller.GetComponent<Ghaddim>();
        Ghaddim unit_ghaddim = _unit.GetComponent<Ghaddim>();
        Mhoddim controller_mhoddim = controller.GetComponent<Mhoddim>();
        Mhoddim unit_mhoddim = _unit.GetComponent<Mhoddim>();

        return (controller_ghaddim != null && unit_ghaddim != null) || (controller_mhoddim != null && unit_mhoddim != null);
    }


    public GameObject GetController()
    {
        return controller;
    }


    // private

    private void Reinforce()
    {
        // support units in control
    }

    private void CheckControl()
    { 
        // determine which faction controls the ruin
    }


    private void TransferControl(GameObject _unit)
    {
        is_controlled = true;
        controller = _unit;
        current_control_points = starting_control_points;
        contenders.Clear();

        if (controller.GetComponent<Ghaddim>() != null) {
            GetComponent<Renderer>().material = ghaddim_skin;
        } else if (controller.GetComponent<Mhoddim>() != null) {
            GetComponent<Renderer>().material = mhoddim_skin;
        }
    }
}