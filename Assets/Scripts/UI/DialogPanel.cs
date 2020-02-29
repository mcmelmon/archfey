using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour
{
    // Inspector
    [SerializeField] GameObject speaker_name = null;
    [SerializeField] GameObject spoken_text = null;
    [SerializeField] Button first_response_button = null;
    [SerializeField] Button second_response_button = null;
    [SerializeField] Button third_response_button = null;
    [SerializeField] GameObject first_response_text = null;
    [SerializeField] GameObject second_response_text = null;
    [SerializeField] GameObject third_response_text = null;


    // properties

    public static DialogPanel Instance { get; set; }

    public Dialog Dialog { get; set; }

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

    public void AddResponse(int _position, Response _response)
    {
        switch (_position) {
            case 0:
                first_response_text.GetComponent<UnityEngine.UI.Text>().text = _response.TextForSuccess;
                first_response_button.onClick.AddListener(delegate{Dialog.HandleResponse(_response);});
                first_response_text.gameObject.SetActive(true);
                break;
            case 1:
                second_response_text.GetComponent<UnityEngine.UI.Text>().text = _response.TextForSuccess;
                second_response_button.onClick.AddListener(delegate{Dialog.HandleResponse(_response);});
                second_response_text.gameObject.SetActive(true);
                break;
            case 2:
                third_response_text.GetComponent<UnityEngine.UI.Text>().text = _response.TextForSuccess;
                third_response_button.onClick.AddListener(delegate{Dialog.HandleResponse(_response);});
                third_response_text.gameObject.SetActive(true);
                break;
        }
    }

    public void ClearResponses()
    {
        first_response_text.GetComponent<UnityEngine.UI.Text>().text = null;
        second_response_text.GetComponent<UnityEngine.UI.Text>().text = null;
        third_response_text.GetComponent<UnityEngine.UI.Text>().text = null;
    }

    public void SetSpeaker(string _speaker) =>
        speaker_name.GetComponent<UnityEngine.UI.Text>().text = _speaker;
    public void SetText(string _text) =>
        spoken_text.GetComponent<UnityEngine.UI.Text>().text = _text;
}
