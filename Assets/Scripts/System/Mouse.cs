using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mouse : MonoBehaviour
{
    // Inspector settings

    public float double_click_delay;

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
        //StartCoroutine(Look()); // for first person...
        StartCoroutine(Select());
    }


    private void Update()
    {
        if (Input.GetKeyDown("escape")) {
            if (HoveredObject != null) 
                HoveredObject.GetComponent<Renderer>().material = HoveredObject.GetComponent<Interactable>().OriginalMaterial;

            for (int i = 0; i < SelectedObjects.Count; i++) {
                SelectedObjects[i].GetComponent<Renderer>().material = SelectedObjects[i].GetComponent<Interactable>().OriginalMaterial;
            }
            SelectedObjects.Clear();
        }
    }


    // pubic


    public void SelectObject(GameObject selected_object)
    {
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)) {
            SelectedObjects.Add(selected_object);
        } else {
            AddSelection(selected_object);
        }
    }


    // private

    
    private void AddSelection(GameObject selected_object, bool clear = true)
    {
        if (clear) ClearSelection();

        SelectedObjects.Add(selected_object);
        foreach (var selection in SelectedObjects.Where(so => so != null)) {
            selection.GetComponent<Renderer>().material = selection.GetComponent<Interactable>().highlight_material;
        }
    }


    private IEnumerator Hover()
    {
        while (true) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int interactable_layer_mask = LayerMask.GetMask("Interactable");
            int ui_layer_mask = LayerMask.GetMask("UI");

            if (Physics.Raycast(ray, out RaycastHit hit, 150f, interactable_layer_mask, QueryTriggerInteraction.Ignore)) {
                GameObject hover = hit.transform.gameObject;

                if (HoveredObject != null && !SelectedObjects.Contains(HoveredObject))
                    HoveredObject.GetComponent<Renderer>().material = HoveredObject.GetComponent<Interactable>().OriginalMaterial;

                HoveredObject = hover;
                if (HoveredObject != null && !SelectedObjects.Contains(HoveredObject)) {
                    Item hover_item = HoveredObject.GetComponent<Item>();
                    HoveredObject.GetComponent<Renderer>().material = hover_item != null && !hover_item.IsSpotted
                        ? HoveredObject.GetComponent<Interactable>().OriginalMaterial
                        : HoveredObject.GetComponent<Interactable>().highlight_material;
                }
            } else {
                if (HoveredObject != null && !SelectedObjects.Contains(HoveredObject)) {
                    HoveredObject.GetComponent<Renderer>().material = HoveredObject.GetComponent<Interactable>().OriginalMaterial;
                    HoveredObject = null;
                }
            }

            yield return null;
        }
    }


    private IEnumerator Look()
    {
        // For first person...

        Transform character = Player.Instance.Me.transform;
        Vector2 mouse_look = Vector2.zero;
        Vector2 smooth_v = Vector2.zero;
        float sensitivity = 1f;
        float smoothing = 2f;

        while (true) {
            var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
            smooth_v.x = Mathf.Lerp(smooth_v.x, md.x, 1f / smoothing);
            smooth_v.y = Mathf.Lerp(smooth_v.y, md.y, 1f / smoothing);
            mouse_look += smooth_v;

            Camera.main.transform.localRotation = Quaternion.AngleAxis(-mouse_look.y, Vector3.right);
            character.localRotation = Quaternion.AngleAxis(mouse_look.x, character.up);
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
                            SelectObject(selected_object);
                            if (Time.time - LastClickTime < double_click_delay) HandleDoubleClick(selected_object);
                        }
                    } else if (Physics.Raycast(ray, out RaycastHit ground_hit, 150f, ground_layer_mask, QueryTriggerInteraction.Ignore)) {
                        ClearSelection();
                        //Player.Instance.Me.Actions.Movement.SetDestination(ground_hit.point);
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
            selection.GetComponent<Renderer>().material = selection.GetComponent<Interactable>().OriginalMaterial;
        }
        SelectedObjects.Clear();
    }


    private void HandleDoubleClick(GameObject selected_object)
    {
        Player.Instance.Me.Actions.Movement.ResetPath();

        Actor selected_actor = selected_object.GetComponent<Actor>();
        Structure selected_structure = selected_object.GetComponent<Structure>();
        Item selected_item = selected_object.GetComponent<Item>();

        if (selected_actor != null) {
            selected_actor.Interactions.InteractWith(Player.Instance.Me);
            StartCoroutine(Player.Instance.Me.Actions.Movement.TrackUnit(selected_actor)); // TODO: if within interaction range, initiate dialog
        } else if (selected_structure != null) {
            Player.Instance.Me.Actions.Movement.SetDestination(selected_structure.NearestEntranceTo(Player.Instance.Me.transform));
        } else if (selected_item != null) {
            selected_item.HandleDoubleClick(Player.Instance.Me);
        }
    }
}
