using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    // properties

    public Actions Actions { get; set; }
    public Conflict.Faction Faction { get; set; }
    public Fey Fey { get; set; }
    public Ghaddim Ghaddim { get; set; }
    public Health Health { get; set; }
    public Mhoddim Mhoddim { get; set; }
    public Conflict.Role Role { get; set; }
    public Senses Senses { get; set; }
    public float Size { get; set; }
    public Stats Stats { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // private


    private void SetComponents()
    {
        Actions = GetComponentInChildren<Actions>();
        Fey = GetComponent<Fey>();
        Ghaddim = GetComponent<Ghaddim>();
        Health = GetComponent<Health>();
        Mhoddim = GetComponent<Mhoddim>();
        Role = Conflict.Role.None;  // offense and defense set this role for mortals
        Senses = GetComponent<Senses>();
        Size = GetComponent<Renderer>().bounds.extents.magnitude;
        Stats = GetComponent<Stats>();

        Faction = (Fey != null) ? Conflict.Faction.Fey : (Ghaddim != null) ? Conflict.Faction.Ghaddim : Conflict.Faction.Mhoddim;
    }
}