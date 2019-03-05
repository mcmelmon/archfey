using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    // properties

    public Item Item { get; set; }


    // Unity


    private void Awake()
    {
        Item = null;
    }


    // public


    public void HoldItem(Item item)
    {
        Item = item;
        GetComponent<Button>().GetComponent<Image>().color = Color.black;
    }
}
