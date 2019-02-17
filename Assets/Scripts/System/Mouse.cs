using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    // Inspector settings

    public float double_click_delay;
    public Color highlight_color;

    // properties

    public static Actor HoveredObject { get; set; }
    public static Mouse Instance { get; set; }
    public static float LastClickTime { get; set; }
    public static List<Actor> SelectedObjects { get; set; }


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one mouse instance!");
            Destroy(this);
            return;
        }
        Instance = this;

        LastClickTime = 0f;
        SelectedObjects = new List<Actor>();
    }


    void Start()
    {
        StartCoroutine(Hover());
        StartCoroutine(Select());
    }


    private void Update()
    {
        if (Input.GetKeyDown("escape")) {
            for (int i = 0; i < SelectedObjects.Count; i++) {
                SelectedObjects[i].GetComponent<Renderer>().material.color = OriginalColor(SelectedObjects[i]);
            }
            SelectedObjects.Clear();
        }
    }


    // public


    public Color OriginalColor(Actor _actor)
    {
        // TODO: don't change the color of the material, add an actual effect that can just be removed
        return Characters.Instance.player_prefab.GetComponent<Renderer>().sharedMaterial.color;
    }


    // private


    private IEnumerator Hover()
    {
        while (true) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int interactable_layer_mask = LayerMask.GetMask("Interactable");
            int ui_layer_mask = LayerMask.GetMask("UI");

            if (Physics.Raycast(ray, out RaycastHit hit, 150f, interactable_layer_mask, QueryTriggerInteraction.Ignore)) {
                GameObject hover = hit.transform.gameObject;

                if (HoveredObject != null)
                    // If we hovered over something earlier, reset its color
                    HoveredObject.GetComponent<Renderer>().material.color = OriginalColor(HoveredObject);

                HoveredObject = hover.GetComponent<Actor>();
                if (HoveredObject == null) yield return null;
                HoveredObject.GetComponent<Renderer>().material.color = highlight_color;
            } else {
                if (HoveredObject != null) {
                    HoveredObject.GetComponent<Renderer>().material.color = OriginalColor(HoveredObject);
                    HoveredObject = null;
                }
            }

            yield return null;
        }
    }


    private IEnumerator Select()
    {
        while (true) {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int interactable_layer_mask = LayerMask.GetMask("Interactable");
                int ground_layer_mask = LayerMask.GetMask("Ground");
                int ui_layer_mask = LayerMask.GetMask("UI");

                if (Physics.Raycast(ray, out RaycastHit interactable_hit, 150f, interactable_layer_mask, QueryTriggerInteraction.Ignore)) {
                    GameObject selected = interactable_hit.transform.gameObject;

                    Actor selected_actor = selected.GetComponent<Actor>();
                    if (selected_actor != null) {
                        if (Input.GetKeyDown("left shift") || Input.GetKeyDown("right shift"))
                        { // TODO: the shift keys are not being detected
                            SelectedObjects.Add(selected_actor);
                        } else {
                            foreach (var selection in SelectedObjects) {
                                selection.GetComponent<Renderer>().material.color = OriginalColor(selection);
                            }
                            SelectedObjects.Clear();
                            SelectedObjects.Add(selected_actor);
                        }

                        if (Time.time - LastClickTime < double_click_delay) {
                            StartCoroutine(Player.Instance.Me.Actions.Movement.TrackUnit(selected_actor));
                        }
                        LastClickTime = Time.time;
                    }
                } else if (Physics.Raycast(ray, out RaycastHit ground_hit, 150f, ground_layer_mask, QueryTriggerInteraction.Ignore)) {
                    ClearSelection();
                    Player.Instance.Me.Actions.Movement.SetDestination(ground_hit.point);
                }
                else {
                    ClearSelection();
                }
            }

            foreach (var selection in SelectedObjects) {
                selection.GetComponent<Renderer>().material.color = highlight_color;
            }

            yield return null;
        }
    }


    private void ClearSelection()
    {
        foreach (var selection in SelectedObjects) {
            selection.GetComponent<Renderer>().material.color = OriginalColor(selection);
        }
        SelectedObjects.Clear();
    }
}
