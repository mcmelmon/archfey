using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour
{
    // position units in 

    public enum Profile { Round = 0, Square = 1, Triangle = 2 };

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
                Circle circular_formation = Circle.CreateCircle(center, width / 2f, units.Count);
                PositionCircle(circular_formation);
                break;
            case Profile.Square:
                Rectangle rectangular_formation = Rectangle.CreateRectangle(center, Mathf.RoundToInt(Mathf.Sqrt(units.Count)) + 1, Mathf.RoundToInt(Mathf.Sqrt(units.Count)) + 1, 5f);
                PositionRectangle(rectangular_formation);
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


    private void PositionRectangle(Rectangle formation)
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].transform.position = formation.points[i];
        }
    }
}