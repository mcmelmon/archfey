using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    // Inspector settings

    public GameObject inventory_panel;
    public GameObject inventory_slots;

    // properties

    public bool Displayed { get; set; }
    public Inventory Instance { get; set; }
    public List<InventorySlot> Slots { get; set; }


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
        Slots = FindObjectsOfType<InventorySlot>().ToList();
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


    public void AddThing(GameObject thing)
    {
        List<InventorySlot> available_slots = Slots.Where(slot => slot.Thing == null).ToList();

        if (available_slots.Any()) {
            available_slots.First().HoldItem(thing);
            thing.SetActive(false);
        }
    }
}
