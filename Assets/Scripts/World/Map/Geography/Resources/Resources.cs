using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public enum Type
    {
        Copper = 0,
        Farm = 1,
        Fish = 2,
        Game = 3,
        Gold = 4,
        Iron = 5,
        Lumber = 6,
        Skins = 7
    };

    // Inspector settings

    public List<Resource> available_resources;

    // properties

    public static Resources Instance { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one resources instance!");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // private
}
