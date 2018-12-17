using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealth : MonoBehaviour {

    public bool attacking;
    public bool spotted;                  // has hostile unit overcome the stealth rating?
    public int stealth_rating;            // how hidden is the unit?
    public int stealh_persistence;        // how well does the unit recover stealth after being spotted?

    Actor actor;
    Material original_material;
    Renderer my_renderer;


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    private void OnValidate()
    {
        if (stealth_rating > 100) stealth_rating = 100;
        if (stealth_rating < 0) stealth_rating = 0;
        if (stealh_persistence > 100) stealh_persistence = 100;
        if (stealh_persistence < 0) stealh_persistence = 0;
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

        if (Random.Range(0, 100) < stealh_persistence) {
            spotted = false;
        }
    }


    public bool Spotted(GameObject _spotter, int opposing_perception)
    {
        if (attacking) {
            spotted = true;
        } else {
            // If we were previously spotted, there is a chance to slip back into stealth if not attacking
            RecoverStealth();

            if (!actor.IsMyFaction(_spotter)) {
                // Our own faction will not reveal us; neutrals will
                // Units with no perception rating fail to spot us
                // others contest their perception against our stealth, i.e. it's 50/50 if both match
                int roll = Random.Range(0, 100);
                spotted = !(opposing_perception == 0) && roll < (50 + opposing_perception - stealth_rating);
            }
        }
        return spotted;
    }


    private IEnumerator Camouflage()
    {
        while (true) {
            RecoverStealth();

            if (!spotted && !attacking) {
                GameObject[] canopies = GetComponentInParent<World>().GetComponentInChildren<Flora>().GetCanopy();
                if (canopies.Length > 0)
                    my_renderer.material = canopies[0].GetComponent<Renderer>().material;
            } else {
                my_renderer.material = original_material;
            }

            yield return null;
        }
    }


    private void SetComponents()
    {
        actor = GetComponent<Actor>();
        my_renderer = GetComponent<Renderer>();
        original_material = my_renderer.material;
        spotted = false;
    }
}
