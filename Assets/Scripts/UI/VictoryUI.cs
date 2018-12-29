using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    // Inspector settings

    public Slider mhoddim_victory_count;
    public Slider ghaddim_victory_count;
    public Transform victorious_faction;
    public Transform victor;


    // properties

    public static VictoryUI Instance { get; set; }

    
    // Unity


    private void Awake() {
        if (Instance != null) 
        {
            Debug.LogError("More than one victory ui instance");
            Destroy(this);
            return;
        }
        Instance = this;
        StartCoroutine(ManageVictory());
    }


    // private


    private IEnumerator ManageVictory()
    {
        while (!Conflict.Victory) {
            yield return new WaitForSeconds(Turn.action_threshold);
            ghaddim_victory_count.value = Ruins.Instance.FactionControl(Conflict.Faction.Ghaddim);
            mhoddim_victory_count.value = Ruins.Instance.FactionControl(Conflict.Faction.Mhoddim);
        }

        TextMeshProUGUI faction_text = victorious_faction.GetComponent<TextMeshProUGUI>();
        faction_text.text = (Conflict.Victor == Conflict.Faction.Ghaddim) ? "Ashen" : "Nibelung";
        victor.gameObject.SetActive(true);
    }
}
