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
        if (Contents.Count < 20) {
            Contents.Add(stored_object);
            stored_object.SetActive(false);
        }

        if (Me == Player.Instance.Me) PlayerInventory.Instance.SyncDisplayedInventory();
    }


    public void AddToPockets(GameObject stored_object)
    {
        if (Pockets.Count < 5) {
            Pockets.Add(stored_object);
            stored_object.SetActive(false);
        }

        if (Me == Player.Instance.Me) PlayerInventory.Instance.SyncDisplayedInventory();
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
    }

    public float StoredValue()
    {
        float stored_value = 0f;

        foreach (Item item in Contents.Select(i => i.GetComponent<Item>())) {
            stored_value += item.base_cost_cp;
        }

        return stored_value;
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
