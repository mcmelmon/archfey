using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    // Inspector settings

    public GameObject inventory_panel;
    public GameObject inventory_slots;

    // properties

    public bool Displayed { get; set; }
    public static PlayerInventory Instance { get; set; }
    public PlayerInventorySlot[] Inventory { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one inventory!");
            Destroy(this);
            return;
        }
        Instance = this;
        Displayed = false;
        Inventory = inventory_slots.GetComponentsInChildren<PlayerInventorySlot>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) Displayed = !Displayed;

        if (Displayed) {
            inventory_panel.SetActive(true);
        } else {
            inventory_panel.SetActive(false);
        }
    }


    // public 


    public void SyncDisplayedInventory() 
    {
        foreach (var slot in Inventory) {
            slot.RemoveItem();
        }

        foreach (var thing in Player.Instance.Me.Inventory.Contents) {
            Inventory[Player.Instance.Me.Inventory.Contents.IndexOf(thing)].HoldItem(thing);
        }

        foreach (var thing in Player.Instance.Me.Inventory.Pockets) {
            Inventory[Player.Instance.Me.Inventory.Contents.IndexOf(thing) + 20].HoldItem(thing);
        }
    }
}
