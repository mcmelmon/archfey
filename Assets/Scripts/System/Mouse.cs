using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    // Inspector settings

    public Color highlight_color;

    // properties

    public static Actor HoveredObject { get; set; }
    public static List<Actor> SelectedObjects { get; set; }


    private void Awake()
    {
        SelectedObjects = new List<Actor>();
    }
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

            foreach (var selection in SelectedObjects) {
                selection.GetComponent<Renderer>().material.color = highlight_color;
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
                    if (Input.GetKeyDown("left shift") || Input.GetKeyDown("right shift")) { // TODO: the shift keys are not being detected
                        SelectedObjects.Add(HoveredObject);
                    } else {
                        foreach (var selection in SelectedObjects) {
                            selection.GetComponent<Renderer>().material.color = OriginalColor(selection);
                        }
                        SelectedObjects.Clear();
                        SelectedObjects.Add(HoveredObject);
                    }
                }
            } else {
                // We have not hit a faction element, so drop previous highlighting
                // TODO: decide whether to keep or drop selection

                if (HoveredObject != null) {
                    HoveredObject.GetComponent<Renderer>().material.color = OriginalColor(HoveredObject);
                    HoveredObject = null;
                }
            }

            if (Input.GetKeyDown("escape")) {
                for (int i = 0; i < SelectedObjects.Count; i++) {
                    SelectedObjects[i].GetComponent<Renderer>().material.color = OriginalColor(SelectedObjects[i]);
                }
                SelectedObjects.Clear();
            }

            yield return null;
        }
    }


    private Color OriginalColor(Actor _actor)
    {
        // TODO: don't change the color of the material, add an actual effect that can just be removed
        return Characters.Instance.player_prefab.GetComponent<Renderer>().sharedMaterial.color;
    }
}
