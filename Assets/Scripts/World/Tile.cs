﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

    // properties

    public Vector3 Location { get; set; }
    public Ruin Ruin { get; set; }
    public Actor Occupier { get; set; }
    public List<Obstacle> Obstacles { get; set; }
    public List<Tree> Trees { get; set; }


    // static


    public static Tile New(Vector3 _location)
    {
        Tile _tile = new Tile
        {
            Location = _location,
            Ruin = null,
            Occupier = null,
            Obstacles = new List<Obstacle>(),
            Trees = new List<Tree>()
        };

        return _tile;
    }


    // public


    public bool Unoccupied()
    {
        return Ruin == null && Occupier == null && Obstacles.Count == 0 && Trees.Count == 0;
    }
}