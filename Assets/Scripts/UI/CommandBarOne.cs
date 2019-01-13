using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandBarOne : MonoBehaviour {

    // Inspector settings

    public Ent ent_prefab;
    public Transform fey_transform;
    public Slider mana_bar;
    public Slider amber_bar;
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
        amber_bar.value = 0;
        mana_bar.value = 1;
    }


    // public


    public void DireOak()
    {
        Vector3 starting_position = new Vector3(player_transform.position.x, 0f, player_transform.position.z) + new Vector3(0, ent_prefab.transform.position.y, 0);
        Vector3 summon_position = starting_position + player_transform.TransformDirection(Vector3.forward) * 20f;
        Ent _ent = ent_prefab.SummonEnt(summon_position, fey_transform);
    }


    public void FountainOfHealing()
    {
        foreach (var target in Mouse.SelectedObjects) {
            Player.Instance.GetComponentInChildren<FountainOfHealing>().Cast(target);
        }
    }


    public IEnumerator Metrics()
    {
        while (true) {
            amber_bar.value = Player.Instance.CurrentAmberPercentage();
            mana_bar.value = Player.Instance.CurrentManaPercentage();
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    public void Raven()
    {

    }
}
