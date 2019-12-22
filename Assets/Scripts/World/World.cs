using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    // Inspector settings

    public Light the_sun;
    public Vector3 sun_angle = new Vector3(45, 70, 0);
    public bool generate_map = false;



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
        the_sun.transform.rotation = Quaternion.Euler(sun_angle);

    }

    private void Start() {
        CreateTheWorld();
    }


    // public


    public Light Sun()
    {
        return the_sun;
    }

    // private

    private void CreateTheWorld()
    {
        if (generate_map) GetComponentInChildren<Map>().DrawMap();
        // GetComponentInChildren<Conflict>().Hajime();
    }
}