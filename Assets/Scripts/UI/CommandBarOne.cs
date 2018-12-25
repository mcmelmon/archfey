using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandBarOne : MonoBehaviour {

    // Inspector settings
    public Ent ent_prefab;
    public Transform fey_transform;
    public TextMeshProUGUI ghaddim_deaths;
    public TextMeshProUGUI ghaddim_captures;
    public TextMeshProUGUI mhoddim_deaths;
    public TextMeshProUGUI mhoddim_captures;
    public Transform player_transform;

    // properties

    public static CommandBarOne Instance { get; set; }


    private IEnumerator healing_touch_coroutine;
    private IEnumerator poison_touch_coroutine;

    // Unity


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


    // public


    public void DireOak()
    {
        Vector3 starting_position = new Vector3(player_transform.position.x, 0f, player_transform.position.z) + new Vector3(0, ent_prefab.transform.position.y, 0);
        Vector3 summon_position = starting_position + player_transform.TransformDirection(Vector3.forward) * 20f;
        Ent _ent = ent_prefab.SummonEnt(summon_position, fey_transform);
    }


    public void HealingTouch(Toggle _toggle)
    {
        if (_toggle.isOn)
        {
            if (healing_touch_coroutine != null) StopCoroutine(healing_touch_coroutine);
            healing_touch_coroutine = HealTheTouched(_toggle);
            StartCoroutine(healing_touch_coroutine);
        } else if (healing_touch_coroutine != null) {
            StopCoroutine(healing_touch_coroutine);
        }
    }


    public IEnumerator Metrics()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.action_threshold);
            ghaddim_deaths.text = "Deaths: " + Conflict.Casualties[Conflict.Faction.Ghaddim].ToString();
            ghaddim_captures.text = "Ruins: " + Ruins.ForFaction[Conflict.Faction.Ghaddim].Count;
            mhoddim_deaths.text = "Deaths: " + Conflict.Casualties[Conflict.Faction.Mhoddim].ToString();
            mhoddim_captures.text = "Ruins: " + Ruins.ForFaction[Conflict.Faction.Mhoddim].Count;
        }

    }


    public void PoisonTouch(Toggle _toggle)
    {
        if (_toggle.isOn)
        {
            if (poison_touch_coroutine != null) StopCoroutine(poison_touch_coroutine);
            poison_touch_coroutine = PoisonTheTouched(_toggle);
            StartCoroutine(poison_touch_coroutine);
        }
        else if (poison_touch_coroutine != null)
        {
            StopCoroutine(poison_touch_coroutine);
        }

    }


    public void Raven()
    {

    }


    // private


    private IEnumerator HealTheTouched(Toggle _toggle)
    {
        while (_toggle.isOn) {
            if (Mouse.HoveredObject != null)
                Mouse.HoveredObject.Health.RecoverHealth(15);
            yield return new WaitForSeconds(Turn.action_threshold / 4f);
        }
    }


    private IEnumerator PoisonTheTouched(Toggle _toggle)
    {
        while (_toggle.isOn) {
            if (Mouse.HoveredObject != null)
                Mouse.HoveredObject.Health.LoseHealth(15);
            yield return new WaitForSeconds(Turn.action_threshold / 4f);
        }
    }
}
