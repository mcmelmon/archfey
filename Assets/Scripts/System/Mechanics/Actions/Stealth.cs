using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealth : MonoBehaviour {

    // properties

    public Actor Actor { get; set; }
    public bool Attacking { get; set; }
    public Material OriginalSkin { get; set; }
    public bool Seen { get; set; }
    public int StealthProficiency { get; set; }
    public int StealthRating { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    private void OnValidate()
    {
        if (StealthProficiency > 10) StealthProficiency = 10;
    }


    private void Start()
    {
        if (Flora.Instance != null) StartCoroutine(Camouflage());
    }


    // public


    public void RecoverStealth()
    {
        // if we've been spotted, we have a shot every turn to regain our stealth

        if (!Seen || Attacking) return;

        if (Random.Range(1,21) < 10 + StealthProficiency + Actor.Stats.AttributeProficiency[Proficiencies.Attribute.Dexterity]) {
            Seen = false;
        }
    }


    public bool SpottedBy(Actor _spotter)
    {
        if (Attacking) {
            Seen = true;
        } else {
            // If we were previously spotted, there is a chance to slip back into stealth if not attacking
            RecoverStealth();

            if (!Actor.Actions.Decider.IsFriendOrNeutral(_spotter)) {
                // Units with no perception rating fail to spot us
                // others contest their perception against our stealth, i.e. it's 50/50 if both match
                Seen = Random.Range(1,21) < (10 + StealthRating);
            }
        }
        return Seen;
    }


    private IEnumerator Camouflage()
    {
        while (true) {
            RecoverStealth();

            if (!Seen && !Attacking) {
                GameObject[] canopies = Flora.Instance.ForestLayers;
                if (canopies.Length > 0)
                    GetComponent<Renderer>().material = canopies[0].GetComponent<Renderer>().material;
            } else {
                GetComponent<Renderer>().material = OriginalSkin;
            }

            yield return null;
        }
    }


    private void SetComponents()
    {
        Actor = GetComponentInParent<Actor>();
        Attacking = false;
        OriginalSkin = GetComponent<Renderer>().material;
        Seen = false;
        StealthRating = StealthProficiency + Actor.Stats.AttributeProficiency[Proficiencies.Attribute.Dexterity];
    }
}
