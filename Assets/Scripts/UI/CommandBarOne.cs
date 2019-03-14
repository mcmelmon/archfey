using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CommandBarOne : MonoBehaviour {

    // Inspector settings

    public GameObject player;
    public GameObject attack_action;
    public GameObject rage_action;
    public GameObject stealth_action;
    public GameObject talk_action;
    public DialogPanel dialog_panel;

    // properties

    public Button AttackButton { get; set; }
    public static CommandBarOne Instance { get; set; }
    public Actor Me { get; set; }
    public Button RageButton { get; set; }
    public Button StealthButton { get; set; }
    public Button TalkButton { get; set; }
    

    // Unity


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one command bar one instance");
            Destroy(this);
            return;
        }
        Instance = this;
        SetComponents();
        StartCoroutine(HandleActionKeys());
        StartCoroutine(ManageButtons());
    }


    // public


    public void Attack()
    {
        // TODO: bonus action attack

        if (Me.Actions.CanTakeAction && AttackButton.interactable) {
            var targets = Mouse.SelectedObjects.Where(so => so != null && Me.Actions.Attack.IsAttackable(so) && Me.Actions.Attack.IsWithinAttackRange(so.transform));
            if (targets.Any()) {
                Me.Actions.Attack.AttackEnemiesInRange(targets.First());
                Me.Actions.CanTakeAction = false;
            }
        }
    }


    public void Dash()
    {
        if (Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction) {
            Me.Actions.CanTakeAction = false;
        }
    }


    public void Disengage()
    {
        if (Me.Actions.CanTakeAction) {

        }
    }


    public void Rage()
    {
        if (Me.Actions.CanTakeAction && RageButton.interactable) {
            if (!Player.Instance.GodOfRage) {
                Player.Instance.Enrage();
            }
        }
    }


    public void Sneak()
    {
        if (Me.Actions.CanTakeAction && StealthButton.interactable) {
            if (Me.Actions.Stealth.Hiding) {
                Me.Actions.Stealth.Appear();
            } else {
                Me.Actions.Stealth.Hide();
                Me.Actions.CanTakeAction = false;
            }
        }
    }


    public void Talk()
    {
        if (TalkButton.interactable) {
            Actor interactor = Mouse.SelectedObjects.First().GetComponent<Actor>();
            if (interactor != null) {
                interactor.Dialog.InitiateDialog(dialog_panel);
            }
        }
    }


    // private


    private IEnumerator HandleActionKeys()
    {
        while (true) {

            if (Input.GetKeyDown(KeyCode.Tab)) {
                List<Actor> potential_targets = FindObjectsOfType<Actor>()
                    .Where(actor => actor != Me && Me.Actions.Attack.IsWithinAttackRange(actor.transform) && !Mouse.SelectedObjects.Contains(actor.gameObject))
                    .OrderBy(actor => Vector3.Distance(transform.position, actor.transform.position))
                    .ToList();

                if (potential_targets.Any()) {
                    Mouse.Instance.SelectObject(potential_targets.First().gameObject);
                }

            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                Attack();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                Talk();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                Sneak();
            }

            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                Rage();
            }

            yield return null;
        }
    }


    private IEnumerator ManageButtons()
    {
        while (true) {
            if (Me.Actions != null) {
                if (Me.Health.CurrentHitPoints == 0) {
                    Me.Actions.CanTakeAction = false;
                    AttackButton.interactable = false;
                    StealthButton.interactable = false;
                    TalkButton.interactable = false;
                    yield return null;
                }

                if (Player.Instance.GodOfRage) {
                    RageButton.gameObject.SetActive(false);
                    StealthButton.gameObject.SetActive(false);
                    TalkButton.gameObject.SetActive(false);
                } else {
                    if (Me.ExhaustionLevel == 0) {
                        RageButton.gameObject.SetActive(true);
                        RageButton.interactable = true;
                    } else {
                        RageButton.gameObject.SetActive(false);
                    }
                    StealthButton.gameObject.SetActive(true);
                    StealthButton.interactable = true;
                    TalkButton.gameObject.SetActive(true);
                }

                if (AttackButton != null) {
                    var interactors = Mouse.SelectedObjects
                                           .Where(so => so != null && Me.Actions.Attack.IsAttackable(so) && Me.Actions.Attack.IsWithinAttackRange(so.transform));
                    bool not_moving = !Me.Actions.Movement.InProgress();
                    bool have_target = interactors.Any();
                    AttackButton.interactable = Me.Actions.CanTakeAction && not_moving && have_target;
                }

                if (StealthButton != null) {
                    StealthButton.GetComponent<Image>().color = Me.Actions.Stealth.Hiding ? Color.black : Color.white;
                }

                if (RageButton != null) {
                    RageButton.GetComponent<Image>().color = Player.Instance.GodOfRage ? Color.red : Color.white;
                }

                if (TalkButton != null && Mouse.HoveredObject != null) {
                    TalkButton.interactable = false;
                    Actor hovered_actor = Mouse.HoveredObject?.GetComponent<Actor>();
                    Actor selected_actor = null;

                    if (Mouse.SelectedObjects.Count == 1 && Mouse.SelectedObjects.First() != null) {
                        selected_actor = Mouse.SelectedObjects.First().GetComponent<Actor>();
                    }

                    if (hovered_actor != null || selected_actor != null) {
                        TalkButton.interactable = (hovered_actor != null) ? hovered_actor.Dialog.WithinRange(Me) : selected_actor.Dialog.WithinRange(Me);
                    }
                }
            }
            yield return null;
        }
    }


    private void SetComponents()
    {
        Me = player.GetComponent<Actor>();

        AttackButton = attack_action.GetComponent<Button>();
        RageButton = rage_action.GetComponent<Button>();
        StealthButton = stealth_action.GetComponent<Button>();
        TalkButton = talk_action.GetComponent<Button>();
    }
}
