using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuinControlUI : MonoBehaviour
{
    // Inspector settings
    public Transform ruin_captured;
    public Transform ruin_lost;
    public Transform ruin_captured_faction;
    public Transform ruin_lost_faction;


    // properties

    public static List<GameObject> ActiveUIElements { get; set; }
    public static RuinControlUI Instance { get; set; }
    public Ruin MostRecentFlip { get; set; }


    // Unity


    private void Awake() {
        if (Instance != null)
        {
            Debug.LogError("More than one ruin control instance");
            Destroy(this);
            return;
        }
        Instance = this;
        ActiveUIElements = new List<GameObject>();
        StartCoroutine(HideUIPanels());
    }


    // public


    public void ChangeInControl(Conflict.Faction new_faction, Conflict.Faction previous_faction)
    {
        if (new_faction != Conflict.Faction.None) {
            TextMeshProUGUI faction_text = ruin_captured_faction.GetComponent<TextMeshProUGUI>();

            switch (new_faction)
            {
                case Conflict.Faction.Ghaddim:
                    int ghaddim_count = Ruins.ForFaction[Conflict.Faction.Ghaddim].Count;
                    faction_text.text = "Unaussprech";
                    ruin_captured.gameObject.SetActive(true);
                    ActiveUIElements.Add(ruin_captured.gameObject);
                    break;
                case Conflict.Faction.Mhoddim:
                    int mhoddim_count = Ruins.ForFaction[Conflict.Faction.Mhoddim].Count;
                    faction_text.text = "Nibelung";
                    ruin_captured.gameObject.SetActive(true);
                    ActiveUIElements.Add(ruin_captured.gameObject);
                    break;
            }
        } else {
            TextMeshProUGUI faction_text = ruin_lost_faction.GetComponent<TextMeshProUGUI>();

            switch (previous_faction) {
                case Conflict.Faction.Ghaddim:
                    int ghaddim_count = Ruins.ForFaction[Conflict.Faction.Ghaddim].Count;
                    faction_text.text = "Unaussprech";
                    ruin_lost.gameObject.SetActive(true);
                    ActiveUIElements.Add(ruin_lost.gameObject);
                    break;
                case Conflict.Faction.Mhoddim:
                    int mhoddim_count = Ruins.ForFaction[Conflict.Faction.Mhoddim].Count;
                    faction_text.text = "Nibelung";
                    ruin_lost.gameObject.SetActive(true);
                    ActiveUIElements.Add(ruin_lost.gameObject);
                    break;
                case Conflict.Faction.None:
                    break;
            }
        }
    }


    public void Teleport()
    {
        GameObject.FindWithTag("Player").transform.position = MostRecentFlip.transform.position + new Vector3(0, 14, 0);
        if (ActiveUIElements.Count > 0)
            ActiveUIElements[ActiveUIElements.Count - 1].SetActive(false);
    }


    // private


    private IEnumerator HideUIPanels()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.action_threshold);

            for (int i = 0; i < ActiveUIElements.Count; i++)
            {
                ActiveUIElements[i].SetActive(false);
                ActiveUIElements.RemoveAt(i);
            }
        }
    }
}
