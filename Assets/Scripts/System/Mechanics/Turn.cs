using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour {

    // properties

    public static float ActionThreshold { get; set; }
    public Actor Me { get; set; }
    public float CurrentHaste { get; set; }
    public float HasteDelta { get; set; }

    // Unity


    private void Awake () {
        ActionThreshold = 6f;
        Me = GetComponent<Actor>();
        HasteDelta = 1f;
    }


    private void Start () {
        StartCoroutine(ResolveTurns());
    }


    // private


    private bool Healthy()
    {
        return Me.Health.Persist();
    }


    private IEnumerator ResolveTurns()
    {
        while (true) {
            if (CurrentHaste < ActionThreshold) {
                CurrentHaste += HasteDelta * Time.deltaTime;
            } else {
                if (Healthy()) {
                    TakeAction();
                    CurrentHaste = 0;
                }
            }

            yield return null;
        }
    }


    private void TakeAction()
    {
        Me.Senses.Sense();
        Me.Actions.ActOnTurn();
    }
}
