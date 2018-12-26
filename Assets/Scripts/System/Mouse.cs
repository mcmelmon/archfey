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
            int faction_layer_mask = LayerMask.GetMask("Faction");
            int ui_layer_mask = LayerMask.GetMask("UI");

            if (SelectedObject != null) {
                SelectedObject.GetComponent<Renderer>().material.color = highlight_color;
            }

            if (Physics.Raycast(ray, out RaycastHit hit, 150f, faction_layer_mask, QueryTriggerInteraction.Ignore)) {
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

            } else {
                // We have not hit a faction element, so drop previous highlighting
                // TODO: decide whether to keep or drop selection

                if (HoveredObject != null) {

                    HoveredObject.GetComponent<Renderer>().material.color = OriginalColor(HoveredObject);
                    HoveredObject = null;
                }
            }

            yield return null;
        }
    }


    private Color OriginalColor(Actor _actor)
    {
        return (_actor.Faction == Conflict.Faction.Ghaddim) ? Conflict.Instance.ghaddim_prefab.GetComponent<Renderer>().sharedMaterial.color : Conflict.Instance.mhoddim_prefab.GetComponent<Renderer>().sharedMaterial.color;

    }
}
