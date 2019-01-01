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

    public float GhaddimControl { get; set; }
    public static VictoryUI Instance { get; set; }
    public float MhoddimControl { get; set; }


    // Unity


    private void Awake() {
        if (Instance != null) 
        {
            Debug.LogError("More than one victory ui instance");
            Destroy(this);
            return;
        }
        GhaddimControl = 0f;
        Instance = this;
        MhoddimControl = 0f;
        StartCoroutine(ManageVictory());
    }


    // private


    private float GhaddimControlPercentage()
    {
        return GhaddimControl / Objectives.AllObjectives.Count;
    }


    private IEnumerator ManageVictory()
    {
        while (!Conflict.Victory) {
            if (Objectives.HeldByFaction != null) {
                GhaddimControl = Objectives.HeldByFaction[Conflict.Faction.Ghaddim].Count;
                MhoddimControl = Objectives.HeldByFaction[Conflict.Faction.Mhoddim].Count;

                ghaddim_victory_count.value = GhaddimControlPercentage();
                mhoddim_victory_count.value = MhoddimControlPercentage();
            }

            yield return new WaitForSeconds(Turn.action_threshold);
        }

        TextMeshProUGUI faction_text = victorious_faction.GetComponent<TextMeshProUGUI>();
        faction_text.text = (Conflict.Victor == Conflict.Faction.Ghaddim) ? "Gnolls" : "Peasants";
        victor.gameObject.SetActive(true);
    }


    private float MhoddimControlPercentage()
    {
        return MhoddimControl / Objectives.AllObjectives.Count;
    }
}
