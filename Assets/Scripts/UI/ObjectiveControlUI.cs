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
