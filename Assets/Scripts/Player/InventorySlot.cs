using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    // properties

    public GameObject Thing { get; set; }


    // Unity


    private void Awake()
    {
        Thing = null;
    }


    // public


    public void HoldItem(GameObject thing)
    {
        if (thing != null) {
            Thing = thing;
            GetComponent<Button>().GetComponent<Image>().color = Color.black;
        }
    }
}
