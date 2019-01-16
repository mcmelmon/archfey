using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile {

    // properties

    public Vector3 Location { get; set; }
    public Objective Objective { get; set; }
    public Actor Occupier { get; set; }
    public List<Tree> Trees { get; set; }


    // static


    public static MapTile New(Vector3 _location)
    {
        MapTile _tile = new MapTile
        {
            Location = _location,
            Objective = null,
            Occupier = null,
            Trees = new List<Tree>()
        };

        return _tile;
    }


    // public


    public bool Unoccupied()
    {
        return Objective == null && Occupier == null && Trees.Count == 0;
    }
}