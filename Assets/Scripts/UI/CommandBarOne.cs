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
    public GameObject disengage_action;
    public GameObject hide_action;
    public GameObject offhand_action;
    public GameObject performance_action;
    public GameObject pick_lock_action;
    public GameObject rage_action;
    public GameObject search_action;
    public GameObject sleight_action;
    public GameObject smite_action;
    public GameObject talk_action;
    public DialogPanel dialog_panel;

    // properties

    public List<Button> ActiveButtonSet { get; set; }
    public List<GameObject> AllActions { get; set; }
    public Button AttackButton { get; set; }
    public Dictionary<string, List<Button>> ButtonSets { get; set; }
    public Button DashButton { get; set; }
    public Button DisengageButton { get; set; }
    public Button HideButton { get; set; }
    public static CommandBarOne Instance { get; set; }
    public Actor Me { get; set; }
    public List<Button> OnCooldown { get; set; }
    public Button OffhandButton { get; set; }
    public Button PerformanceButton { get; set; }
    public Button PickLockButton { get; set; }
    public Button RageButton { get; set; }
    public Button SearchButton { get; set; }
    public Button SleightButton { get; set; }
    public Button SmiteButton { get; set; }
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
        if (Me.Actions.CanTakeAction && AttackButton.interactable && Mouse.SelectedObjects != null) {
            var targets = Mouse.SelectedObjects.Where(so => so != null && Me.Actions.Combat.IsAttackable(so) && Me.Actions.Combat.IsWithinAttackRange(so.transform));
            if (targets.Any()) {
                GameObject target = targets.First();
                if (Me.Actions.Combat.IsWithinMeleeRange(target.transform)) {
                    Me.Actions.Decider.TargetMelee(target);
                } else {
                    Me.Actions.Decider.TargetRanged(target);
                }
                Me.Actions.Attack(false, true); // offhand = false, player target = true
                Me.Actions.CanTakeAction = false;
            }
        }
    }


    public void Dash()
    {
        if ((Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction) && DashButton.interactable) {
            Me.Actions.CanTakeAction = false;
            Me.Actions.Movement.Dash();
            StartCoroutine(Cooldown(DashButton, 30));
        }
    }


    public void Disengage()
    {
        if ((Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction) && DisengageButton.interactable) {
            Me.Actions.CanTakeAction = false;
            Me.Actions.Movement.Disengage();
            StartCoroutine(Cooldown(DisengageButton, 15));
        }
    }


    public void Hide()
    {
        if ((Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction) && HideButton.interactable) {
            if (Me.Actions.Stealth.IsHiding){
                Me.Actions.Stealth.StopHiding();
            } else {
                Me.Actions.Stealth.Hide();
                Me.Actions.CanTakeAction = false;
            }
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


    public void Offhand()
    {
        if (Me.Actions.CanTakeBonusAction && OffhandButton.interactable && Mouse.SelectedObjects != null) {
            var targets = Mouse.SelectedObjects.Where(so => so != null && Me.Actions.Combat.IsAttackable(so) && Me.Actions.Combat.IsWithinAttackRange(so.transform));
            if (targets.Any()) {
                GameObject target = targets.First();
                if (Me.Actions.Combat.IsWithinMeleeRange(target.transform)) {
                    Me.Actions.Decider.TargetMelee(target);
                } else {
                    Me.Actions.Decider.TargetRanged(target); // e.g. offhand thrown dagger
                }
                Me.Actions.Attack(true, true); // offhand = true, player target = true
                Me.Actions.CanTakeBonusAction = false;
            }
        }
    }


    public void Performance()
    {
        if (Me.Actions.CanTakeAction && PerformanceButton.interactable) {
            if (Me.Actions.Stealth.IsPerforming) {
                Me.Actions.Stealth.StopPerforming();
            } else {
                Me.Actions.Stealth.Performance();
                Me.Actions.CanTakeAction = false;
            }
        }
    }


    public void PickLock()
    {
        if ((Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction) && PickLockButton.interactable) {
            Me.Actions.CanTakeAction = false;
            Me.Actions.Stealth.PickLock();
        }
    }


    public void Search()
    {
        if ((Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction) && SearchButton.interactable) {
            Me.Actions.CanTakeAction = false;
            Me.Actions.Search();
        }
    }


    public void SleightOfHand()
    {
        if ((Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction) && SleightButton.interactable) {
            Me.Actions.CanTakeAction = false;
            Me.Actions.Stealth.SleightOfHand();
        }
    }


    public void Smite()
    {
        if (Me.Actions.CanTakeAction && SmiteButton.interactable && Mouse.SelectedObjects != null) {
            var targets = Mouse.SelectedObjects.Where(so => so != null && Me.Actions.Combat.IsAttackable(so) && Me.Actions.Combat.IsWithinAttackRange(so.transform));
            if (targets.Any()) {
                Actor actor = targets.First().GetComponent<Actor>();
                if (actor != null && Me.Actions.Combat.IsWithinMeleeRange(actor.transform)) {
                    Me.Actions.Decider.TargetMelee(actor.gameObject);
                    Me.Actions.Attack(false, true); // offhand = false, player target = true
                    Player.Instance.CastEldritchSmite(actor);
                    StartCoroutine(Cooldown(SmiteButton, 20));
                    Me.Actions.CanTakeAction = false;
                }
            }
        }
    }


    public void Talk()
    {
        if (TalkButton.interactable && Mouse.SelectedObjects != null) {
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

                if (AttackButton != null && Mouse.SelectedObjects != null) {
                    var interactors = Mouse.SelectedObjects
                                           .Where(so => so != null && Me.Actions.Combat.IsAttackable(so) && Me.Actions.Combat.IsWithinAttackRange(so.transform));
                    bool have_target = interactors.Any();
                    AttackButton.interactable = Me.Actions.CanTakeAction && have_target;
                    SmiteButton.interactable = AttackButton.interactable;
                }

                if (DashButton != null) {
                    DashButton.interactable = !Me.Actions.Movement.IsDashing && (Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction);
                }

                if (DisengageButton != null) {
                    DisengageButton.interactable = !Me.Actions.Movement.IsDashing && (Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction);
                }

                if (HideButton != null) {
                    HideButton.interactable = !Me.Actions.Combat.Engaged && (Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction);
                }

                if (OffhandButton != null && Mouse.SelectedObjects != null) {
                    var interactors = Mouse.SelectedObjects
                                           .Where(so => so != null && Me.Actions.Combat.IsAttackable(so) && Me.Actions.Combat.IsWithinAttackRange(so.transform));
                    bool have_target = interactors.Any();
                    OffhandButton.interactable = Me.Actions.CanTakeBonusAction && have_target;
                }

                if (PerformanceButton != null) {
                    PerformanceButton.interactable = Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction;
                }

                if (PickLockButton != null && Mouse.SelectedObjects != null) {
                    if (Mouse.SelectedObjects.Count == 1 && Mouse.SelectedObjects.First() != null) {
                        Item target = Mouse.SelectedObjects.First().GetComponent<Item>();
                        PickLockButton.interactable = Me.Actions.CanTakeAction && target != null && !target.IsUnlocked;
                    } else {
                        PickLockButton.interactable = false;
                    }
                }

                if (RageButton != null) {
                    RageButton.interactable = Me.ExhaustionLevel == 0;
                }

                if (SearchButton != null) {
                    SearchButton.interactable = Me.Actions.CanTakeAction || Me.Actions.CanTakeBonusAction;
                }

                if (SleightButton != null && Mouse.SelectedObjects != null) {
                    if (Mouse.SelectedObjects.Count == 1 
                        && Mouse.SelectedObjects.First() != null 
                        && Vector3.Distance(transform.position, Mouse.SelectedObjects.First().transform.position) < Me.Actions.Movement.ReachedThreshold) 
                    {
                        Actor target_actor = Mouse.SelectedObjects.First().GetComponent<Actor>();
                        Item target_item = Mouse.SelectedObjects.First().GetComponent<Item>();
                        SleightButton.interactable = Me.Actions.CanTakeAction && (target_actor != null || target_item != null);
                    } else {
                        SleightButton.interactable = false;
                    }
                }

                if (TalkButton != null) {
                    TalkButton.interactable = false;
                    Actor hovered_actor = null;
                    Actor selected_actor = null;

                    if (Mouse.HoveredObject != null) {
                        hovered_actor = Mouse.HoveredObject?.GetComponent<Actor>();
                    }
                    
                    if (Mouse.SelectedObjects?.Count == 1 && Mouse.SelectedObjects.First() != null) {
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
                    .Where(actor => actor != Me && Me.Actions.Combat.IsWithinAttackRange(actor.transform) && !Mouse.SelectedObjects.Contains(actor.gameObject))
                    .OrderBy(actor => Vector3.Distance(transform.position, actor.transform.position))
                    .ToList();

                if (potential_targets.Any()) {
                    Mouse.Instance.SelectObject(potential_targets.First().gameObject);
                }

            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                if (ActiveButtonSet.Count >= 1 && ActiveButtonSet[0].interactable)
                    ActiveButtonSet[0]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                if (ActiveButtonSet.Count >= 2 && ActiveButtonSet[1].interactable)
                    ActiveButtonSet[1]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                if (ActiveButtonSet.Count >= 3 && ActiveButtonSet[2].interactable)
                    ActiveButtonSet[2]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                if (ActiveButtonSet.Count >= 4 && ActiveButtonSet[3].interactable)
                    ActiveButtonSet[3]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha5)) {
                if (ActiveButtonSet.Count >= 5 && ActiveButtonSet[4].interactable)
                    ActiveButtonSet[4]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha6)) {
                if (ActiveButtonSet.Count >= 6 && ActiveButtonSet[5].interactable)
                    ActiveButtonSet[5]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha7)) {
                if (ActiveButtonSet.Count >= 7 && ActiveButtonSet[6].interactable)
                    ActiveButtonSet[6]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha8)) {
                if (ActiveButtonSet.Count >= 8 && ActiveButtonSet[7].interactable)
                    ActiveButtonSet[7]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha9)) {
                if (ActiveButtonSet.Count >= 9 && ActiveButtonSet[8].interactable)
                    ActiveButtonSet[8]?.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha0)) {
                if (ActiveButtonSet.Count >= 10 && ActiveButtonSet[9].interactable)
                    ActiveButtonSet[9]?.onClick.Invoke();
            }

            yield return null;
        }
    }


    private void SetComponents()
    {
        Me = player.GetComponent<Actor>();

        AllActions = new List<GameObject> { attack_action, dash_action, disengage_action, hide_action, performance_action, pick_lock_action, rage_action, search_action, sleight_action, smite_action, talk_action  };
        AttackButton = attack_action.GetComponent<Button>();
        DashButton = dash_action.GetComponent<Button>();
        DisengageButton = disengage_action.GetComponent<Button>();
        HideButton = hide_action.GetComponent<Button>();
        OnCooldown = new List<Button>();
        OffhandButton = offhand_action.GetComponent<Button>();
        PerformanceButton = performance_action.GetComponent<Button>();
        PickLockButton = pick_lock_action.GetComponent<Button>();
        RageButton = rage_action.GetComponent<Button>();
        SearchButton = search_action.GetComponent<Button>();
        SleightButton = sleight_action.GetComponent<Button>();
        SmiteButton = smite_action.GetComponent<Button>();
        TalkButton = talk_action.GetComponent<Button>();

        ButtonSets = new Dictionary<string, List<Button>>
        {
            // In new story, only have "druid" role at moment, but may add other roles

            ["Druid"] = new List<Button> { AttackButton, HideButton, SearchButton },
            ["Thief"] = new List<Button> { AttackButton, OffhandButton, DashButton, DisengageButton, HideButton, PickLockButton, SleightButton, PerformanceButton, TalkButton, RageButton },
            ["Warlock"] = new List<Button> { AttackButton, SmiteButton, TalkButton }
        };
    }
}
