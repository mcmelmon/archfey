using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    // Inspector
    [SerializeField] List<Actor> participants;
    [SerializeField] List<Objective> places;
    [SerializeField] List<Item> objects;

    // properties

    public bool Active { get; set; }
    public bool DiscoveredByPlayer { get; set; }
    public List<Actor> MembersInteractedWith { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    // private

    private void SetComponents()
    {
        Active = true;
        DiscoveredByPlayer = false;
        MembersInteractedWith = new List<Actor>();
    }
}
