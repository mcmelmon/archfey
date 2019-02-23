using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    // Inspector settings
    public List<string> chit_chat;
    public DialogNode start_conversation;

    // properties

    public DialogNode Current { get; set; }
    public Actor Me { get; set; }


    // Unity


    private void Awake()
    {
        Current = start_conversation;
        Me = GetComponent<Actor>();
    }


    private void Start()
    {
        StartCoroutine(ChatIfPlayerNear());
    }

    // public


    public string GetChitChat()
    {
        // Chit Chat consists of strings the actor emotes when the player passes by, but does not specifically Talk
        return chit_chat[Random.Range(0, chit_chat.Count)];
    }


    public void InitiateDialog(DialogPanel dialog_panel)
    {
        dialog_panel.speaker_name.GetComponent<UnityEngine.UI.Text>().text = Me.Stats.name;
        dialog_panel.spoken_text.GetComponent<UnityEngine.UI.Text>().text = "Hello, " + Player.Instance.Me.Stats.name;
        dialog_panel.gameObject.SetActive(true);
    }


    public bool WithinRange(Actor other_actor)
    {
        return Vector3.Distance(transform.position, other_actor.transform.position) < 10f;
    }


    // private


    private IEnumerator ChatIfPlayerNear() {
        // TODO: account for stealth

        while (!Me.IsPlayer()) {
            List<Actor> audience = FindObjectsOfType<Actor>().Where(actor => actor != Me && WithinRange(actor)).ToList();

            // TODO: make references to specific audience members, but for now...
            if (audience.Any()) {
                Vector3 new_facing = Vector3.RotateTowards(transform.forward, audience.First().transform.position - transform.position, 10f, 0f);
                transform.rotation = Quaternion.LookRotation(new_facing);

                string chat = GetChitChat();
                if (chat != "") {
                    Debug.Log(chat);
                }
            }
            yield return new WaitForSeconds(Turn.ActionThreshold * Random.Range(1,3));
        }
    }
}

[System.Serializable]
public class DialogNode
{
    // inspector settings
    public bool is_exit;
    public string text;
    public List<DialogNode> respones;
}
