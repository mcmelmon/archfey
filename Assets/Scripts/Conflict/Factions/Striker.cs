using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour {

    Geography geography;
    Actor actor;

    // Unity

    private void Awake()
    {
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        actor = GetComponent<Actor>();
    }


    private void Start()
    {
        ConfigureRoleSpecificProperties();
        Strategize();
    }


    private void Update()
    {

    }


    // public


    private void Restrategize()
    {
        if (actor.attack != null) {

        }
        else {

        }
    }


    private void Strategize()
    {
        // TODO: differentiate between Mhoddim and Ghaddim approaches

        if (actor.attack != null) {
        }
        else {
        }
    }


    // private


    private void ConfigureRoleSpecificProperties()
    {
        actor = GetComponent<Actor>();
        actor.SetComponents();
        actor.SetStats();
    }
}


public class Formation 
{
    // position units in 

    public enum Profile { Round = 0, Rectangle = 1, Triangle = 2 };

    Profile profile;
    Vector3 center;
    float width;
    List<GameObject> units = new List<GameObject>();

    public static Formation CreateFormation(Vector3 _center, float _width, Profile _profile)
    {
        Formation _formation = new Formation
        {
            profile = _profile,
            center = _center,
            width = _width
        };

        return _formation;
    }


    public void JoinFormation(GameObject unit)
    {
        units.Add(unit);

        switch (profile)
        {
            case Profile.Round:
                Circle formation = Circle.CreateCircle(center, width / 2f, units.Count);
                PositionCircle(formation);
                break;
            case Profile.Rectangle:
                break;
            case Profile.Triangle:
                break;
        }
    }


    // private


    private void PositionCircle(Circle formation)
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].transform.position = formation.vertices[i];
            Vector3 facing = units[i].transform.position - formation.center;
            facing.y = 0;
            units[i].transform.rotation = Quaternion.LookRotation(facing);
        }
    }
}