using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour
{
    // Inspector settings
    public GameObject speaker_name;
    public GameObject spoken_text;
    public GameObject first_response;
    public GameObject second_response;
    public GameObject third_response;


    // public


    public void ResponseChosen(UnityEngine.UI.Text response_text)
    {
        Actor interactor = Player.Instance.Me.Interactions.Interactors.First();
        if (interactor != null) {
            interactor.Dialog.Answer(response_text.text);
        }
    }
}
