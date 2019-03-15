using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CommandBarOne : MonoBehaviour {

    // Inspector settings

    public GameObject player;
    public GameObject attack_action;
    public GameObject dash_action;
    public GameObject rage_action;
    public GameObject smite_action;
    public GameObject stealth_action;
    public GameObject talk_action;
    public DialogPanel dialog_panel;

    // properties

    public List<Button> ActiveButtonSet { get; set; }
    public List<GameObject> AllActions { get; set; }
    public Button AttackButton { get; set; }
    public Dictionary<string, List<Button>> ButtonSets { get; set; }
    public Button DashButton { get; set; }
    public static CommandBarOne Instance { get; set; }
    public Actor Me { get; set; }
    public List<Button> OnCooldown { get; set; }
    public Button RageButton { get; set; }
    public Button SmiteButton { get; set; }
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
        StartCoroutine(ButtonInteractability());
    }


    // public


    public void ActivateButtonSet(string set)
    {
        foreach (var action in AllActions) {
            action.SetActive(false);
        }

        ActiveButtonSet = ButtonSets[set];

        foreach (var button in ActiveButtonSet) {
            button.gameObject.SetActive(true);
        }
    }


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
            Me.Actions.Movement.Dash();
            StartCoroutine(Cooldown(DashButton, 30));
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


    public void Smite()
    {
        if (Me.Actions.CanTakeAction && SmiteButton.interactable) {
            var targets = Mouse.SelectedObjects.Where(so => so != null && Me.Actions.Attack.IsAttackable(so) && Me.Actions.Attack.IsWithinAttackRange(so.transform));
            if (targets.Any()) {
                Me.Actions.Attack.AttackEnemiesInRange(targets.First());
                Actor actor = targets.First().GetComponent<Actor>();
                if (actor != null) {
                    Player.Instance.EldritchSmite(actor);
                    StartCoroutine(Cooldown(SmiteButton, 30));
                }
                Me.Actions.CanTakeAction = false;
            }
        }
    }


    public void Sneak()
    {
        if (Me.Actions.CanTakeAction && StealthButton.interactable) {
            if (Me.Actions.Stealth.IsHiding) {
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


    private IEnumerator ButtonInteractability()
    {
        while (true) {
            if (Me.Actions != null) {

                // deal with the buttons as if no cooldowns

                if (AttackButton != null) {
                    var interactors = Mouse.SelectedObjects
                                           .Where(so => so != null && Me.Actions.Attack.IsAttackable(so) && Me.Actions.Attack.IsWithinAttackRange(so.transform));
                    bool not_moving = !Me.Actions.Movement.InProgress();
                    bool have_target = interactors.Any();
                    AttackButton.interactable = Me.Actions.CanTakeAction && not_moving && have_target;
                    SmiteButton.interactable = AttackButton.interactable;
                }

                if (DashButton != null) {
                    DashButton.interactable = !Me.Actions.Movement.IsDashing && (Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction);
                }

                if (RageButton != null) {
                    RageButton.interactable = Me.ExhaustionLevel == 0;
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

                // then deal with cooldowns

                foreach (var button in OnCooldown) {
                    button.interactable = false;
                }
            }
            yield return null;
        }
    }


    private IEnumerator Cooldown(Button button, int seconds)
    {
        int tick = 0;
        if (!OnCooldown.Contains(button)) OnCooldown.Add(button);

        while (tick < seconds) {
            tick++;
            yield return new WaitForSeconds(1);
        }

        OnCooldown.Remove(button);
    }


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
                ActiveButtonSet[0]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                ActiveButtonSet[1]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                ActiveButtonSet[2]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                ActiveButtonSet[3]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha5)) {
                ActiveButtonSet[4]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha6)) {
                ActiveButtonSet[5]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha7)) {
                ActiveButtonSet[6]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha8)) {
                ActiveButtonSet[7]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha9)) {
                ActiveButtonSet[8]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha0)) {
                ActiveButtonSet[9]?.onClick.Invoke();
            }

            yield return null;
        }
    }


    private void SetComponents()
    {
        Me = player.GetComponent<Actor>();

        AllActions = new List<GameObject> { attack_action, dash_action, rage_action, smite_action, stealth_action, talk_action };
        AttackButton = attack_action.GetComponent<Button>();
        DashButton = dash_action.GetComponent<Button>();
        OnCooldown = new List<Button>();
        RageButton = rage_action.GetComponent<Button>();
        SmiteButton = smite_action.GetComponent<Button>();
        StealthButton = stealth_action.GetComponent<Button>();
        TalkButton = talk_action.GetComponent<Button>();

        ButtonSets = new Dictionary<string, List<Button>>
        {
            ["Thief"] = new List<Button> { AttackButton, DashButton, StealthButton, RageButton, TalkButton },
            ["Warlock"] = new List<Button> { AttackButton, SmiteButton }
        };
    }
}
