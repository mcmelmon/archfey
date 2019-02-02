using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveControlUI : MonoBehaviour
{
    // Inspector settings
    public Transform objective_captured;
    public Transform objective_lost;
    public Transform objective_captured_faction;
    public Transform objective_lost_faction;


    // properties

    public static List<GameObject> ActiveUIElements { get; set; }
    public static ObjectiveControlUI Instance { get; set; }
    public Objective MostRecentFlip { get; set; }


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


    public void ChangeClaim(Conflict.Alignment new_faction, Conflict.Alignment previous_faction)
    {
        if (new_faction != Conflict.Alignment.Unaligned) {
            TextMeshProUGUI faction_text = objective_captured_faction.GetComponent<TextMeshProUGUI>();

            switch (new_faction)
            {
                case Conflict.Alignment.Evil:
                    int ghaddim_count = FindObjectsOfType<Objective>().Where(objective => objective.Claim == Conflict.Alignment.Evil).ToList().Count;
                    faction_text.text = "Gnolls";
                    objective_captured.gameObject.SetActive(true);
                    ActiveUIElements.Add(objective_captured.gameObject);
                    break;
                case Conflict.Alignment.Good:
                    int mhoddim_count = FindObjectsOfType<Objective>().Where(objective => objective.Claim == Conflict.Alignment.Good).ToList().Count;
                    faction_text.text = "Peasants";
                    objective_captured.gameObject.SetActive(true);
                    ActiveUIElements.Add(objective_captured.gameObject);
                    break;
            }
        } else {
            TextMeshProUGUI faction_text = objective_lost_faction.GetComponent<TextMeshProUGUI>();

            switch (previous_faction) {
                case Conflict.Alignment.Evil:
                    int ghaddim_count = FindObjectsOfType<Objective>().Where(objective => objective.Claim == Conflict.Alignment.Evil).ToList().Count;
                    faction_text.text = "Gnolls";
                    objective_lost.gameObject.SetActive(true);
                    ActiveUIElements.Add(objective_lost.gameObject);
                    break;
                case Conflict.Alignment.Good:
                    int mhoddim_count = FindObjectsOfType<Objective>().Where(objective => objective.Claim == Conflict.Alignment.Good).ToList().Count;
                    faction_text.text = "Peasants";
                    objective_lost.gameObject.SetActive(true);
                    ActiveUIElements.Add(objective_lost.gameObject);
                    break;
                case Conflict.Alignment.Unaligned:
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
            yield return new WaitForSeconds(Turn.ActionThreshold / 1.5f);

            for (int i = 0; i < ActiveUIElements.Count; i++)
            {
                ActiveUIElements[i].SetActive(false);
                ActiveUIElements.RemoveAt(i);
            }
        }
    }
}
