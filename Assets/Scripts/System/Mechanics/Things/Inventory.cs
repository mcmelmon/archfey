using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    // properties

    public List<GameObject> Contents { get; set; }
    public Actor Me { get; set; }
    public List<GameObject> Pockets { get; set; }


    // Unity


    private void Awake()
    {
        Contents = new List<GameObject>();
        Me = GetComponent<Actor>();
        Pockets = new List<GameObject>();
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
}
