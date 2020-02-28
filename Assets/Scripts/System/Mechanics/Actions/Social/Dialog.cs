using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    // Inspector settings
    public List<string> chit_chat;
    public List<Statement> statements;
    public List<Statement> responses;

    // properties

    public Actor Me { get; set; }
    public Statement CurrentStatement { get; set; }
    public List<Statement> ResponsesChosen { get; set; }

    // Unity

    private void Awake()
    {
        SetComponents();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Me == null) return;

        Actor actor = other.GetComponent<Actor>();
        if (actor == null || actor.Actions.Stealth.IsHiding) return;

        if (Me.Senses.HasLineOfSightOn(actor.gameObject) && (actor.IsPlayer() || (Me.IsPlayer() && Me.Interactions.Interactors.Contains(actor)))) {
            Vector3 new_facing = Vector3.RotateTowards(transform.forward, actor.transform.position - transform.position, 10f, 0f);
            transform.rotation = Quaternion.LookRotation(new_facing);
        } 

        string chat = GetChitChat();
    }

    private void OnTriggerStay(Collider other)
    {
        if (Me == null) return;

        Actor actor = other.GetComponent<Actor>();
        if (actor == null || actor.Actions.Stealth.IsHiding) return;

        if (Me.Senses.HasLineOfSightOn(actor.gameObject) && (actor.IsPlayer() || (Me.IsPlayer() && Me.Interactions.Interactors.Contains(actor)))) {
            Vector3 new_facing = Vector3.RotateTowards(transform.forward, actor.transform.position - transform.position, 10f, 0f);
            transform.rotation = Quaternion.LookRotation(new_facing);
        }

        // TODO: also face a combatant
    }

    // public

    public string GetChitChat() =>
        // Chit Chat consists of strings the actor emotes when the player passes by, but does not specifically Talk
        (chit_chat.Count > 0) ? chit_chat[Random.Range(0, chit_chat.Count)] : "";

    public void HandleResponse(Statement _response)
    {
        CurrentStatement = _response.Answer();
        DisplayCurrent();
    }

    public void InitiateDialog(DialogPanel dialog_panel)
    {
        DisplayCurrent();
        DialogPanel.Instance.gameObject.SetActive(true);
    }

    public bool WithinRange(Actor other_actor)
    {
        return Vector3.Distance(transform.position, other_actor.transform.position) < 10f;
    }

    // private

    private void DisplayCurrent()
    {
        CurrentStatement.SeenByPlayer = true;
        DialogPanel.Instance.SetSpeaker(Me.Stats.name);
        DialogPanel.Instance.SetText(CurrentStatement.GetStatementToPlayer());

        foreach (var response in CurrentStatement.PresentResponses()) {
            // create a "button" in the dialog
        }
    }

    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        CurrentStatement = (statements.Any()) ? statements[0] : null;
        ResponsesChosen = new List<Statement>();
    }
}

