using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemFamily { Component, Consumable, Container, Jewelry, Ore }

    public bool is_equipable;
    public bool is_hidden;
    public int spot_challenge_rating;
    public bool is_locked;
    public int unlock_challenge_rating;
    public float base_weight;
    public float weight_override;
    public float base_cost;
    public float cost_override;
    
    // properties

    public Interactable Interactable { get; set; }
    public Action OnDoubleClick { get; set; }
    public bool IsSpotted { get; set; }
    public bool IsUnlocked { get; set; }


    // Unity

    private void Awake()
    {
        SetComponents();
    }


    // public


    public bool HandleDoubleClick(Actor player)
    {
        if (OnDoubleClick != null) {
            OnDoubleClick.Invoke();
        } else {
            Player.Instance.Inventory.AddItem(this);
        }
        return true;
    }


    // private


    private void SetComponents()
    {
        Interactable = GetComponent<Interactable>();
        IsSpotted = !is_hidden;
        IsUnlocked = !is_locked;
    }
}
