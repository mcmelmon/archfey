using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    [SerializeField] List<GameObject> contents = new List<GameObject>();
    // properties

    public List<GameObject> Contents { get; set; }
    public Actor Me { get; set; }
    public List<GameObject> Pockets { get; set; }
    public Structure Structure { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public 


    public void AddToInventory(GameObject stored_object, int number_to_add = 1)
    {
        for (int i = 0; i < number_to_add; i++) {
            Contents.Add(stored_object);
        }

        CheckIfEncumbered();

        // TODO: some objects should be deactivated (like weapons picked up), some shouldn't (like resource primitives)

        if (Me != null && Me.IsPlayer()) {
            PlayerInventory.Instance.SyncDisplayedInventory();
        }
    }


    public void AddToPockets(GameObject stored_object)
    {
        Pockets.Add(stored_object);
    }

    public void CheckIfEncumbered()
    {
        if (Me != null) Me.Actions.Movement.Encumbered = StorageWeight() > Me.Stats.CarryingCapacity();
    }

    public void Empty()
    {
        Contents.Clear();
    }

    public bool HasContents()
    {
        return Contents.Count > 0;
    }

    public void RemoveFromInventory(GameObject stored_object, int number_to_remove = 1)
    {
        int number_in_inventory = StorageCount(stored_object);
        if (number_to_remove > number_in_inventory) number_to_remove = number_in_inventory;
        List<GameObject> removeable_items = Contents.Where(i => i == stored_object).ToList();

        for (int i = 0; i < removeable_items.Count(); i++) {
            GameObject item = removeable_items[i];
            Contents.Remove(item);
        }

        CheckIfEncumbered();
    }

    public void RemoveFromInventoryAt(int index)
    {
        Contents.RemoveAt(index);
    }

    public int StorageCount(GameObject item = null)
    {
        if (item == null) {
            return Contents.Count;
        } else {
            return Contents.Where(i => i.gameObject == item).ToList().Count();
        }
    }

    public float StorageValue(GameObject item = null)
    {
        float stored_value = 0f;

        if (item == null) {
            foreach (Item match in Contents.Select(i => i.GetComponent<Item>())) {
                stored_value += match.GetAdjustedValueInCopper();
            }
        } else {
            foreach (Item match in Contents.Where(i => i.gameObject == item).Select(i => i.GetComponent<Item>())) {
                stored_value += match.GetAdjustedValueInCopper();
            }
        }

        return stored_value;
    }

    public float StorageWeight(GameObject item = null)
    {
        float stored_weight = 0;

        if (item == null) {
            foreach (Item match in Contents.Select(i => i.GetComponent<Item>())) {
                stored_weight += match.GetAdjustedWeight();
            }
        } else {
            foreach (Item match in Contents.Where(i => i.gameObject == item).Select(i => i.GetComponent<Item>())) {
                stored_weight += match.GetAdjustedWeight();
            }
        }

        return stored_weight;
    }

    // private

    private void SetComponents()
    {
        Contents = contents;
        Me = GetComponent<Actor>();
        Pockets = new List<GameObject>();
        Structure = GetComponent<Structure>();
    }
}
