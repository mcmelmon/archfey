using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    // Inspector settings

    public bool battleground;
    public Light the_sun;


    // properties

    public static World Instance { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one world instance!");
            Destroy(this);
            return;
        }
        Instance = this;
        Vector3 sun_rotation = new Vector3(45, 70, 0);
        the_sun.transform.rotation = Quaternion.Euler(sun_rotation);

    }


    // public


    public Light Sun()
    {
        return the_sun;
    }
}