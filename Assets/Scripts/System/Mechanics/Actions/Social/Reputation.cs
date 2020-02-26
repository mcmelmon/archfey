using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reputation : MonoBehaviour
{
    // Inspector

    [SerializeField] Plot plot = null;
    [SerializeField] Faction faction = null;

    // properties

    public Actor Me { get; set; }
    public int Value { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    // private

    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        Value = 0;
    }
}
