using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealth : MonoBehaviour {

    public bool attacking;
    public bool spotted;                    // has another unit overcome the stealth rating?
    public float stealth_rating;            // how hidden is the unit?
    public float stealh_persistence;        // how well does the unit recover stealth after being spotted?

    Material original_material;
    Renderer my_renderer;


    // Unity


    private void Awake()
    {
        my_renderer = GetComponent<Renderer>();
        original_material = my_renderer.material;
        spotted = false;
    }


    private void OnValidate()
    {
        if (stealth_rating > 1f) stealth_rating = 1f;
        if (stealth_rating < 0f) stealth_rating = 0f;
        if (stealh_persistence > 1f) stealh_persistence = 1f;
        if (stealh_persistence < 0f) stealh_persistence = 0f;
    }


    private void Start()
    {
        StartCoroutine(Camouflage());
    }


    // public


    public void RecoverStealth()
    {
        // if we've been spotted, we have a shot every turn to regain our stealth

        if (!spotted || attacking) return;

        if (Random.Range(0f, 100f) < stealh_persistence * 100) {  // TODO: simplify these "percentile dice" ratings
            spotted = false;
            Debug.Log("Recovered stealth");
        }
    }


    public bool Spotted(float perception_rating)
    {
        if (attacking) {
            spotted = true;
        } else {
            // There is a chance to slip back into stealth even when spotted
            RecoverStealth();

            // Units with no perception rating fail to spot us; others contest their perception against our stealth, i.e. it's 50/50 if both match
            spotted = !(Mathf.Approximately(perception_rating, 0f)) && (Random.Range(0, 100) < (0.5f + (perception_rating - stealth_rating)) * 100);
        }
        return spotted;
    }


    private IEnumerator Camouflage()
    {
        while (true) {
            if (!spotted && !attacking) {
                my_renderer.material = GetComponentInParent<World>().GetComponentInChildren<Flora>().GetCanopy()[0].GetComponent<Renderer>().material;
            } else {
                my_renderer.material = original_material;
            }

            yield return null;
        }
    }
}
