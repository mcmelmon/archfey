using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour
{
    // Inspector
    [SerializeField] GameObject speaker_name = null;
    [SerializeField] GameObject spoken_text = null;

    // properties

    public static DialogPanel Instance { get; set; }

    // Unity

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one dialog panel instance!");
            Destroy(this);
            return;
        }
        Instance = this;
        gameObject.SetActive(false);
    }

    // public


    public void ResponseChosen(UnityEngine.UI.Text response_text)
    {
        Actor interactor = Player.Instance.Me.Interactions.Interactors.First();

    }

    public void SetSpeaker(string _speaker) =>
        speaker_name.GetComponent<UnityEngine.UI.Text>().text = _speaker;
    public void SetText(string _text) =>
        spoken_text.GetComponent<UnityEngine.UI.Text>().text = _text;
}
