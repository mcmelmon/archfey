using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    // Inspector settings

    public Material highlight_material;

    // properties

    public List<Actor> Interactors { get; set; }
    public Actor Me { get; set; }
    public Structure Structure {get; set; }
    public Item Item { get; set; }
    public Material OriginalMaterial { get; set; }


    // Unity


    private void Awake() {
        Item = GetComponent<Item>();
        Interactors = new List<Actor>();
        Me = GetComponent<Actor>();
        OriginalMaterial = GetComponent<Renderer>().material;
        Structure = GetComponent<Structure>();
    }


    public void InteractWith(Actor other_actor)
    {
        if (Interactors.Contains(other_actor)) return;
        Interactors.Clear(); // for now; in future, interact with player first, then an npc
        Interactors.Add(other_actor);
        other_actor.Interactions.InteractWith(Me);
    }


    public void InteractWith(Structure _structure)
    {

    }
}
