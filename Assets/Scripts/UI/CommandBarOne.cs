using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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


    public void EntangleUnits()
    {

    }


    public void HealUnits()
    {

    }


    public void SummonEnt()
    {
        Vector3 starting_position = new Vector3(player_transform.position.x, 0f, player_transform.position.z) + new Vector3(0, ent_prefab.transform.position.y, 0);
        Vector3 summon_position = starting_position + player_transform.TransformDirection(Vector3.forward) * 20f;
        Ent _ent = ent_prefab.SummonEnt(summon_position, fey_transform);
    }


    public void SummonRaven()
    {

    }
}
