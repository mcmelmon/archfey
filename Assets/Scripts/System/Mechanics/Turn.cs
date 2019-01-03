using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour {

    public static float action_threshold = 6f;
    public float haste_delta = 1f;
    float current_haste;

    Actor actor;
    Health health;

    // Unity


    private void Awake () {
        actor = GetComponent<Actor>();
        health = GetComponent<Health>();
    }


    private void Start () {
        StartCoroutine(ResolveTurns());
    }


    // private


    private bool Healthy()
    {
        return health.Persist();
    }


    private IEnumerator ResolveTurns()
    {
        while (!Conflict.Victory) {
            if (current_haste < action_threshold) {
                current_haste += haste_delta * Time.deltaTime;
            } else {
                if (Healthy()) {
                    TakeAction();
                    current_haste = 0;
                }
            }

            yield return null;
        }
    }


    private void TakeAction()
    {
        actor.ActOnTurn();
    }
}
