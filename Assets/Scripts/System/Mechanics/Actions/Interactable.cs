using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    // Inspector settings

    public Material highlight_material;
    public bool is_suspicious;
    public int suspicion_challenge_rating;
    public Proficiencies.Skill relevant_skill;

    // properties

    public bool Flashing { get; set; }
    public List<Actor> Interactors { get; set; }
    public Actor Actor { get; set; }
    public Structure Structure {get; set; }
    public Item Item { get; set; }
    public Material OriginalMaterial { get; set; }


    // Unity


    private void Awake() {
        SetComponents();
    }


    // public


    public void DrawAttention()
    {
        StartCoroutine(Flash());
    }


    public void InteractWith(Actor other_actor)
    {
        if (Interactors.Contains(other_actor)) return;  // this should prevent infinite recursion
        Interactors.Clear(); // in future, interact with player first, then an npc
        Interactors.Add(other_actor);
        other_actor.Interactions.InteractWith(Actor);
    }


    public void LoseAttention()
    {
        StopCoroutine(Flash());
    }


    // private


    private IEnumerator Flash()
    {
        while (!Flashing) {
            GetComponent<Renderer>().material = highlight_material;
            yield return new WaitForSeconds(Turn.ActionThreshold);
            Flashing = true;
        }
        GetComponent<Renderer>().material = OriginalMaterial;
        Flashing = false;
    }


    private void SetComponents()
    {
        Flashing = false;
        Item = GetComponent<Item>();
        Interactors = new List<Actor>();
        Actor = GetComponent<Actor>();
        OriginalMaterial = GetComponent<Renderer>().material;
        Structure = GetComponent<Structure>();
    }
}
