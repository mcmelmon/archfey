using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuinControlUI : MonoBehaviour
{
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
            Transform _captured = transform.Find("RuinCaptured");
            Transform _text = _captured.Find("Text");
            Transform _faction = _text.Find("Faction");
            TextMeshProUGUI faction_text = _faction.GetComponent<TextMeshProUGUI>();

            switch (new_faction)
            {
                case Conflict.Faction.Ghaddim:
                    int ghaddim_count = Ruins.ForFaction[Conflict.Faction.Ghaddim].Count;
                    faction_text.text = "Unaussprechlichen";
                    _captured.gameObject.SetActive(true);
                    ActiveUIElements.Add(_captured.gameObject);
                    break;
                case Conflict.Faction.Mhoddim:
                    int mhoddim_count = Ruins.ForFaction[Conflict.Faction.Mhoddim].Count;
                    faction_text.text = "Nibelung";
                    _captured.gameObject.SetActive(true);
                    ActiveUIElements.Add(_captured.gameObject);
                    break;
            }
        } else {
            Transform _lost = transform.Find("RuinLost");
            Transform _text = _lost.Find("Text");
            Transform _faction = _text.Find("Faction");
            TextMeshProUGUI faction_text = _faction.GetComponent<TextMeshProUGUI>();

            switch (previous_faction) {
                case Conflict.Faction.Ghaddim:
                    int ghaddim_count = Ruins.ForFaction[Conflict.Faction.Ghaddim].Count;
                    faction_text.text = "Unaussprechlichen";
                    _lost.gameObject.SetActive(true);
                    ActiveUIElements.Add(_lost.gameObject);
                    break;
                case Conflict.Faction.Mhoddim:
                    int mhoddim_count = Ruins.ForFaction[Conflict.Faction.Mhoddim].Count;
                    faction_text.text = "Nibelung";
                    _lost.gameObject.SetActive(true);
                    ActiveUIElements.Add(_lost.gameObject);
                    break;
                case Conflict.Faction.None:
                    break;
            }
        }
    }


    public void Teleport()
    {
        ActiveUIElements[ActiveUIElements.Count - 1].SetActive(false);
        ActiveUIElements.Remove(ActiveUIElements[ActiveUIElements.Count - 1]);
        GameObject.FindWithTag("Player").transform.position = MostRecentFlip.transform.position + new Vector3(0, 14, 0);
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
