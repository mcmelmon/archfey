using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealth : MonoBehaviour {

    public bool spotted;                    // has another unit overcome the stealth rating?
    public float stealth_rating;            // how hidden is the unit?
    public float stealh_persistence;        // how well does the unit recover stealth after being spotted?


    // Unity


    private void Awake()
    {
        spotted = false;
    }


    private void OnValidate()
    {
        if (stealth_rating > 1f) stealth_rating = 1f;
        if (stealth_rating < 0f) stealth_rating = 0f;
        if (stealh_persistence > 1f) stealh_persistence = 1f;
        if (stealh_persistence < 0f) stealh_persistence = 0f;
    }


    private void Update()
    {
        // This "works," but only if you inspect the material at runtime
        //if (!spotted) {
        //    Renderer my_renderer = GetComponent<Renderer>();
        //    my_renderer.material.SetFloat("_Mode", 3);
        //    Color _color = my_renderer.material.color;
        //    _color.a = 0.4f;
        //    my_renderer.material.color = _color;
        //} else {
        //    Renderer my_renderer = GetComponent<Renderer>();
        //    my_renderer.material.SetFloat("_Mode", 0);
        //    Color _color = my_renderer.material.color;
        //    _color.a = 1f;
        //    my_renderer.material.color = _color;
        //}
    }


    // public


    public void RecoverStealth()
    {
        // if we've been spotted, we have a shot every turn to regain our stealth

        if (!spotted) return;

        if (Random.Range(0f, 1f) < stealh_persistence) {
            spotted = false;
            Debug.Log("Recovered stealth");
        }
    }


    public bool Spotted(float perception_rating)
    {
        // There is a chance to slip back into stealth even when spotted
        RecoverStealth();

        // If that failed, we're seen
        if (spotted) return true;

        // If still unspotted, and stealth_rating == perception_rating, then 50% chance of being spotted
        spotted = Random.Range(0f, 1f) < 0.5f + (perception_rating - stealth_rating);
        return spotted;
    }
}
