using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    Geography geography;
    int sky_height = 20;


    public struct HeavenAndEarth
    {
        public static Plane earth;
        public static Plane heaven;
    }


    // Unity


    void Awake ()
    {
        geography = transform.GetComponentInChildren<Geography>();
        SetHeavenAndEarth();
    }


    // public


    public Vector3 GetCenter()
    {
        return geography.GetCenter();
    }


    public Geography GetGeography()
    {
        return geography;
    }


    // private

    private void OnValidate()
    {
        if (sky_height < 30) sky_height = 30;
    }


    void AddDirectionBoundary(string direction, Transform boundaries)
    {
        // TODO: keep player from flying off map
    }


    void SetBounds()
    {

    }


    void SetHeavenAndEarth()
    {
        HeavenAndEarth.earth = new Plane(Vector3.up, 0f);
        HeavenAndEarth.heaven = new Plane(Vector3.down, sky_height * 5);
    }
}