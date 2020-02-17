using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
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


    public void AddToInventory(GameObject stored_object)
    {
        Contents.Add(stored_object);
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

    public void RemoveFromInventory(GameObject stored_object)
    {
        Contents.Remove(stored_object);
        CheckIfEncumbered();
    }

    public void RemoveFromInventoryAt(int index)
    {
        Contents.RemoveAt(index);
    }

    public int StorageCount()
    {
        return Contents.Count;
    }

    public float StorageValue()
    {
        float stored_value = 0f;

        foreach (Item item in Contents.Select(i => i.GetComponent<Item>())) {
            stored_value += item.GetAdjustedValueInCopper();
        }

        return stored_value;
    }

    public float StorageWeight()
    {
        float stored_weight = 0;

        foreach (Item item in Contents.Select(i => i.GetComponent<Item>())) {
            stored_weight += item.GetAdjustedWeight();
        }

        return stored_weight;

    }

    // private

    private void SetComponents()
    {
        Contents = new List<GameObject>();
        Me = GetComponent<Actor>();
        Pockets = new List<GameObject>();
        Structure = GetComponent<Structure>();
    }
}
