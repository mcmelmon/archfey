using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommandBarOne : MonoBehaviour {

    public Transform player_transform;  // TODO: name other transforms using _transform convention
    public Transform fey_transform;
    public Ent ent_prefab;

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

            Transform _ghaddim = transform.Find("Ghaddim");
            Transform ghaddim_casualties = _ghaddim.Find("Casualties");
            Transform ghaddim_ruins = _ghaddim.Find("Ruins");
            TextMeshProUGUI ghaddim_deaths = ghaddim_casualties.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI ghaddim_captures = ghaddim_ruins.GetComponent<TextMeshProUGUI>();
            ghaddim_deaths.text = "Deaths: " + Conflict.Casualties[Conflict.Faction.Ghaddim].ToString();
            ghaddim_captures.text = "Ruins: " + Ruins.ForFaction[Conflict.Faction.Ghaddim].Count;

            Transform _mhoddim = transform.Find("Mhoddim");
            Transform mhoddim_casualties = _mhoddim.Find("Casualties");
            Transform mhoddim_ruins = _mhoddim.Find("Ruins");
            TextMeshProUGUI mhoddim_deaths = mhoddim_casualties.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI mhoddim_captures = mhoddim_ruins.GetComponent<TextMeshProUGUI>();
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
