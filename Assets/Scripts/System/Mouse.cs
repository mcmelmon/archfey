using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mouse : MonoBehaviour
{
    // Inspector settings

    public float double_click_delay;
    public Color highlight_color;

    // properties

    public static GameObject HoveredObject { get; set; }
    public static Mouse Instance { get; set; }
    public static float LastClickTime { get; set; }
    public static List<GameObject> SelectedObjects { get; set; }


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one mouse instance!");
            Destroy(this);
            return;
        }
        Instance = this;

        LastClickTime = 0f;
        SelectedObjects = new List<GameObject>();
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
                SelectedObjects[i].GetComponent<Renderer>().material.color -= highlight_color;
            }
            SelectedObjects.Clear();
        }
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
                    HoveredObject.GetComponent<Renderer>().material.color -= highlight_color;

                HoveredObject = hover;
                if (HoveredObject != null) HoveredObject.GetComponent<Renderer>().material.color += highlight_color;
            } else {
                if (HoveredObject != null) {
                    HoveredObject.GetComponent<Renderer>().material.color -= highlight_color;
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
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    int interactable_layer_mask = LayerMask.GetMask("Interactable");
                    int ground_layer_mask = LayerMask.GetMask("Ground");

                    if (Physics.Raycast(ray, out RaycastHit interactable_hit, 150f, interactable_layer_mask, QueryTriggerInteraction.Ignore)) {
                        GameObject selected_object = interactable_hit.transform.gameObject;

                        if (selected_object != null) {
                            if (Input.GetKeyDown("left shift") || Input.GetKeyDown("right shift")) { // TODO: the shift keys are not being detected
                                SelectedObjects.Add(selected_object);
                            } else {
                                foreach (var selection in SelectedObjects.Where(so => so != null)) {
                                    selection.GetComponent<Renderer>().material.color -= highlight_color;
                                }
                                SelectedObjects.Clear();
                                SelectedObjects.Add(selected_object);
                            }

                            if (Time.time - LastClickTime < double_click_delay) HandleDoubleClick(selected_object);
                            selected_object.GetComponent<Renderer>().material.color += highlight_color;
                        }
                    } else if (Physics.Raycast(ray, out RaycastHit ground_hit, 150f, ground_layer_mask, QueryTriggerInteraction.Ignore)) {
                        ClearSelection();
                        Player.Instance.Me.Actions.Movement.SetDestination(ground_hit.point);
                    } else {
                        ClearSelection();
                    }
                }
                LastClickTime = Time.time;
            }

            yield return null;
        }
    }


    private void ClearSelection()
    {
        foreach (var selection in SelectedObjects.Where(so => so != null)) {
            selection.GetComponent<Renderer>().material.color -= highlight_color;
        }
        SelectedObjects.Clear();
    }


    private void HandleDoubleClick(GameObject selected_object)
    {
        Player.Instance.Me.Actions.Movement.ResetPath();

        Actor selected_actor = selected_object.GetComponent<Actor>();
        Structure selected_structure = selected_object.GetComponent<Structure>();

        if (selected_actor != null) {
            StartCoroutine(Player.Instance.Me.Actions.Movement.TrackUnit(selected_actor));
        } else if (selected_structure != null) {
            Player.Instance.Me.Actions.Movement.SetDestination(selected_structure.NearestEntranceTo(Player.Instance.Me.transform));
        }
    }
}
