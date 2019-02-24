using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    // Inspector settings
    public List<string> chit_chat;
    public List<DialogNode> statements;
    public List<DialogResponse> responses;

    // properties

    public DialogNode Current { get; set; }
    public DialogPanel Panel { get; set; }
    public Actor Me { get; set; }


    // Unity


    private void Awake()
    {
        Current = (statements.Any()) ? statements[0] : null;
        Me = GetComponent<Actor>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (Me == null) return;

        // TODO: check for stealth
        Actor actor = other.GetComponent<Actor>();
        if (actor == null) return;

        if (actor.IsPlayer() || (Me.IsPlayer() && Me.Interactors.Contains(actor))) {
            Vector3 new_facing = Vector3.RotateTowards(transform.forward, actor.transform.position - transform.position, 10f, 0f);
            transform.rotation = Quaternion.LookRotation(new_facing);
        } 

        string chat = GetChitChat();
        Debug.Log(chat);
    }


    private void OnTriggerStay(Collider other)
    {
        if (Me == null) return;

        // TODO: check for stealth
        Actor actor = other.GetComponent<Actor>();
        if (actor == null) return;

        if (actor.IsPlayer() || (Me.IsPlayer() && Me.Interactors.Contains(actor))) {
            Vector3 new_facing = Vector3.RotateTowards(transform.forward, actor.transform.position - transform.position, 10f, 0f);
            transform.rotation = Quaternion.LookRotation(new_facing);
        }

        // TODO: also face a combatant
    }


    // public


    public void Answer(string response_text)
    {
        DialogResponse response = responses.Where(r => r.text == response_text).ToList().First();
        if (response != null) {
            response.chosen = true;
            Current = statements.First(statement => statement.id == response.destination_dialog_id);
            DisplayCurrent();
        }
    }


    public string GetChitChat() =>
        // Chit Chat consists of strings the actor emotes when the player passes by, but does not specifically Talk
        (chit_chat.Count > 0) ? chit_chat[Random.Range(0, chit_chat.Count)] : "";


    public void InitiateDialog(DialogPanel dialog_panel)
    {
        Panel = dialog_panel;
        Panel.speaker_name.GetComponent<UnityEngine.UI.Text>().text = Me.Stats.name;
        DisplayCurrent();
        Panel.gameObject.SetActive(true);
    }


    public bool WithinRange(Actor other_actor)
    {
        return Vector3.Distance(transform.position, other_actor.transform.position) < 10f;
    }


    // private


    private void DisplayCurrent()
    {
        Current.seen = true;
        Panel.spoken_text.GetComponent<UnityEngine.UI.Text>().text = Current.text;
        List<DialogResponse> current_responses = responses.Where(r => r.initiating_dialog_id == Current.id).ToList();

        // yes, this is silly...
        switch (current_responses.Count) {
            case 1:
                Panel.first_response.GetComponent<UnityEngine.UI.Text>().text = current_responses[0].text;
                Panel.first_response.SetActive(true);
                Panel.second_response.SetActive(false);
                Panel.third_response.SetActive(false);
                break;
            case 2:
                Panel.first_response.GetComponent<UnityEngine.UI.Text>().text = current_responses[0].text;
                Panel.second_response.GetComponent<UnityEngine.UI.Text>().text = current_responses[1].text;
                Panel.first_response.SetActive(true);
                Panel.second_response.SetActive(true);
                Panel.third_response.SetActive(false);

                break;
            case 3:
                Panel.first_response.GetComponent<UnityEngine.UI.Text>().text = current_responses[0].text;
                Panel.second_response.GetComponent<UnityEngine.UI.Text>().text = current_responses[1].text;
                Panel.third_response.GetComponent<UnityEngine.UI.Text>().text = current_responses[2].text;
                Panel.first_response.SetActive(true);
                Panel.second_response.SetActive(true);
                Panel.third_response.SetActive(true);
                break;
            default:
                Panel.first_response.SetActive(false);
                Panel.second_response.SetActive(false);
                Panel.third_response.SetActive(false);
                break;
        }
    }
}


[System.Serializable]
public class DialogNode
{
    // inspector settings
    public int id;
    public string text;
    public List<int> response_ids;
    public bool seen;
}


[System.Serializable]
public class DialogResponse
{
    // inspector settings
    public int initiating_dialog_id;
    public int destination_dialog_id;
    public string text;
    public bool chosen;
}
