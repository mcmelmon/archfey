using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Inspector settings

    public int hit_points;
    public List<GameObject> triggers;

    // properties

    public Item Item { get; set; }
    public Vector3 OriginalScale { get; set; }

    // Unity


    private void Awake()
    {
        Item = GetComponent<Item>();
        Item.OnDoubleClick = OpenDoor;
        OriginalScale = transform.localScale; 

        StartCoroutine(CloseDoor());
    }


    // public


    public void OpenDoor()
    {
        if (Item.IsUnlocked) gameObject.transform.localScale = new Vector3(0,0,0);
    }


    // private


    private IEnumerator CloseDoor()
    {
        while (true) {
            yield return new WaitForSeconds(10);
            gameObject.transform.localScale = OriginalScale;
        }
    }
}
