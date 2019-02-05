using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandBarOne : MonoBehaviour {

    // Inspector settings

    public Transform player_transform;

    // properties

    public static CommandBarOne Instance { get; set; }
    

    // Unity


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one command bar one instance");
            Destroy(this);
            return;
        }
        Instance = this;
        StartCoroutine(Metrics());
    }


    private void Start()
    {
    }


    // public


    public IEnumerator Metrics()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }
}
