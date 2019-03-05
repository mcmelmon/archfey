using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CommandBarOne : MonoBehaviour {

    // Inspector settings

    public GameObject player;
    public GameObject attack_action;
    public GameObject stealth_action;
    public GameObject talk_action;
    public DialogPanel dialog_panel;

    // properties

    public Button AttackButton { get; set; }
    public static CommandBarOne Instance { get; set; }
    public Actor Me { get; set; }
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
        StartCoroutine(ManageButtons());
    }


    // public


    public void Attack()
    {
        if (Me.Actions.CanTakeTurn) {
            AttackButton.interactable = false;
            Me.Actions.CanTakeTurn = false;
            Me.Actions.Decider.IdentifyEnemies();
            Me.Actions.Attack.AttackEnemiesInRange();
        }
    }


    public void Sneak()
    {
        if (Me.Actions.CanTakeTurn) {
            if (Me.Actions.Stealth.Hiding) {
                Me.Actions.Stealth.Appear();
            } else {
                Me.Actions.Stealth.Hide();
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


    public IEnumerator ManageButtons()
    {
        while (true) {
            if (Me.Actions != null) {
                if (AttackButton != null) {
                    var interactors = Mouse.SelectedObjects.Select(so => so.GetComponent<Actor>());
                    AttackButton.interactable = interactors.Any() && Me.Actions.CanTakeTurn;
                }

                if (TalkButton != null) {
                    TalkButton.interactable = false;
                    Actor hovered_actor = Mouse.HoveredObject?.GetComponent<Actor>();
                    Actor selected_actor = (Mouse.SelectedObjects.Count == 1) ? Mouse.SelectedObjects.First().GetComponent<Actor>() : null;

                    if (hovered_actor != null || selected_actor != null) {
                        TalkButton.interactable = (hovered_actor != null) ? hovered_actor.Dialog.WithinRange(Me) : selected_actor.Dialog.WithinRange(Me);
                    }
                }

                if (StealthButton != null) {
                    StealthButton.GetComponent<Image>().color = Me.Actions.Stealth.Hiding ? Color.black : Color.white;
                }
            }
            yield return null;
        }
    }


    // private

    private void SetComponents()
    {
        Me = player.GetComponent<Actor>();

        AttackButton = attack_action.GetComponent<Button>();
        StealthButton = stealth_action.GetComponent<Button>();
        TalkButton = talk_action.GetComponent<Button>();
    }
}
