using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    // Inspector settings

    public Color highlight_color;

    // properties

    public static Actor HoveredObject { get; set; }
    public static Actor SelectedObject { get; set; }

    void Start()
    {
        StartCoroutine(FeyTouch());
    }


    // private


    private IEnumerator FeyTouch()
    {
        while (true) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layer_mask = LayerMask.GetMask("Faction");

            if (SelectedObject != null) {
                SelectedObject.GetComponent<Renderer>().material.color = highlight_color;
            }

            if (Physics.Raycast(ray, out RaycastHit hit, 150f, layer_mask, QueryTriggerInteraction.Ignore)) {
                // We've touched something on the Faction layer

                GameObject hover = hit.transform.gameObject;

                if (HoveredObject != null)
                    // If we touched something earlier, reset its color
                    HoveredObject.GetComponent<Renderer>().material.color = OriginalColor(HoveredObject);

                HoveredObject = hover.GetComponent<Actor>();
                HoveredObject.GetComponent<Renderer>().material.color = highlight_color;

                if (Input.GetMouseButtonDown(0)) {
                    if (SelectedObject != null)
                        SelectedObject.GetComponent<Renderer>().material.color = OriginalColor(SelectedObject);

                    SelectedObject = HoveredObject;
                    SelectedObject.GetComponent<Renderer>().material.color = highlight_color;
                }

            } else if (HoveredObject != null) {
                // We have not hit something, so drop previous highlighting

                HoveredObject.GetComponent<Renderer>().material.color = OriginalColor(HoveredObject);
                HoveredObject = null;
            } else if (Input.GetMouseButtonDown(0) && SelectedObject != null) {
                SelectedObject.GetComponent<Renderer>().material.color = OriginalColor(SelectedObject);
                SelectedObject = null;
            }

            yield return null;
        }
    }


    private Color OriginalColor(Actor _actor)
    {
        return (_actor.Faction == Conflict.Faction.Ghaddim) ? Conflict.Instance.ghaddim_prefab.GetComponent<Renderer>().sharedMaterial.color : Conflict.Instance.mhoddim_prefab.GetComponent<Renderer>().sharedMaterial.color;

    }
}
