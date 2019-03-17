using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventorySlot : MonoBehaviour
{
    // properties

    public GameObject Contents { get; set; }


    // Unity


    private void Awake()
    {
        Contents = null;
    }


    // public


    public void HoldItem(GameObject thing)
    {
        if (thing != null) {
            Contents = thing;
            GetComponent<Button>().GetComponent<Image>().color = Color.black;
        }
    }


    public void RemoveItem()
    {
        Contents = null;
        GetComponent<Button>().GetComponent<Image>().color = Color.white;
    }
}
