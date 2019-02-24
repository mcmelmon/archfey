using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CommandBarOne : MonoBehaviour {

    // Inspector settings

    public GameObject player;
    public GameObject attack_action;
    public GameObject talk_action;
    public DialogPanel dialog_panel;

    // properties

    public Button AttackButton { get; set; }
    public static CommandBarOne Instance { get; set; }
    public Actor Me { get; set; }
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
        StartCoroutine(CooldownTimer());
        StartCoroutine(PushToTalk());
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


    public void Talk()
    {
        if (TalkButton.interactable) {
            Actor interactor = Mouse.SelectedObjects.First().GetComponent<Actor>();
            if (interactor != null) {
                interactor.Dialog.InitiateDialog(dialog_panel);
            }
        }
    }


    public IEnumerator CooldownTimer()
    {
        while (true) {
            if (AttackButton != null && Me.Actions != null) 
                AttackButton.interactable = Me.Actions.CanTakeTurn;
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    public IEnumerator PushToTalk()
    {
        while (true) {
            if (TalkButton != null && Me.Actions != null) {
                TalkButton.interactable = false;
                Actor hovered_actor = Mouse.HoveredObject?.GetComponent<Actor>();
                Actor selected_actor = (Mouse.SelectedObjects.Count == 1) ? Mouse.SelectedObjects.First().GetComponent<Actor>() : null;
                
                if (hovered_actor != null || selected_actor != null) {
                    TalkButton.interactable = (hovered_actor != null) ? hovered_actor.Dialog.WithinRange(Me) : selected_actor.Dialog.WithinRange(Me);
                }
            }
            yield return null;
        }
    }


    // private

    private void SetComponents()
    {
        AttackButton = attack_action.GetComponent<Button>();
        Me = player.GetComponent<Actor>();
        TalkButton = talk_action.GetComponent<Button>();
    }
}
