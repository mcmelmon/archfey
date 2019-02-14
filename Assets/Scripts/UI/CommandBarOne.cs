using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandBarOne : MonoBehaviour {

    // Inspector settings

    public GameObject player;
    public GameObject attack_action;

    // properties

    public Button AttackButton { get; set; }
    public static CommandBarOne Instance { get; set; }
    public Actor Me { get; set; }
    

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


    public IEnumerator CooldownTimer()
    {
        while (true) {
            if (AttackButton != null && Me.Actions != null) 
                AttackButton.interactable = Me.Actions.CanTakeTurn;
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    // private

    private void SetComponents()
    {
        AttackButton = attack_action.GetComponent<Button>();
        Me = player.GetComponent<Actor>();
    }
}
