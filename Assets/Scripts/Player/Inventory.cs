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
    public List<InventorySlot> Slots { get; set; }


    // Unity


    private void Awake()
    {
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


    public void AddItem(Item item)
    {
        List<InventorySlot> available_slots = Slots.Where(slot => slot.Item == null).ToList();

        if (available_slots.Any()) {
            available_slots.First().HoldItem(item);
            item.gameObject.SetActive(false);
        }
    }
}
