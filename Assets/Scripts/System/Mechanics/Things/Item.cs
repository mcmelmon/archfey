using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemFamily { Material, Product }

    public bool is_equipable;
    public bool is_pocketable;
    public bool is_hidden;
    public int spot_challenge_rating;
    public bool is_locked;
    public int unlock_challenge_rating;
    public float base_weight;
    public float base_cost_cp;
    
    // properties

    public Interactable Interactable { get; set; }
    public bool IsSpotted { get; set; }
    public bool IsUnlocked { get; set; }
    public Action OnDoubleClick { get; set; }
    public float ValueAdjustmentRatio { get; set; }
    public float WeightAdjustmentRatio { get; set; }

    // Unity

    private void Awake()
    {
        SetComponents();
    }


    // public

    public float GetAdjustedValueInCopper()
    {
        return ValueAdjustmentRatio * base_cost_cp;
    }
    
    public float GetAdjustedWeight()
    {
        return WeightAdjustmentRatio * base_weight;
    }

    public bool HandleDoubleClick(Actor player)
    {
        if (!player == Player.Instance.Me) return false;

        if (OnDoubleClick != null) {
            OnDoubleClick.Invoke();
        } else {
            if (is_pocketable) {
                Player.Instance.Me.Inventory.AddToPockets(this.gameObject);
            } else {
                Player.Instance.Me.Inventory.AddToInventory(this.gameObject);
            }
        }
        return true;
    }


    // private


    private void SetComponents()
    {
        Interactable = GetComponent<Interactable>();
        IsSpotted = !is_hidden;
        IsUnlocked = !is_locked;
        ValueAdjustmentRatio = 1f;
        WeightAdjustmentRatio = 1f;
    }
}
