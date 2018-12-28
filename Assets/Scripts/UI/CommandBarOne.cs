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
    public Transform player_transform;


    //public TextMeshProUGUI ghaddim_deaths;
    //public TextMeshProUGUI ghaddim_captures;
    //public TextMeshProUGUI mhoddim_deaths;
    //public TextMeshProUGUI mhoddim_captures;

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


    public void DireOak()
    {
        Vector3 starting_position = new Vector3(player_transform.position.x, 0f, player_transform.position.z) + new Vector3(0, ent_prefab.transform.position.y, 0);
        Vector3 summon_position = starting_position + player_transform.TransformDirection(Vector3.forward) * 20f;
        Ent _ent = ent_prefab.SummonEnt(summon_position, fey_transform);
    }


    public void FountainOfHealing()
    {
        Player.Instance.GetComponentInChildren<FountainOfHealing>().Cast(Mouse.SelectedObject, false);
        mana_bar.value = Player.Instance.Abilities.CurrentManaPercentage();
    }


    public IEnumerator Metrics()
    {
        while (true) {
            mana_bar.value = Player.Instance.Abilities.CurrentManaPercentage();
            yield return new WaitForSeconds(Turn.action_threshold);

            //ghaddim_deaths.text = "Deaths: " + Conflict.Casualties[Conflict.Faction.Ghaddim].ToString();
            //ghaddim_captures.text = "Ruins: " + Ruins.ForFaction[Conflict.Faction.Ghaddim].Count;
            //mhoddim_deaths.text = "Deaths: " + Conflict.Casualties[Conflict.Faction.Mhoddim].ToString();
            //mhoddim_captures.text = "Ruins: " + Ruins.ForFaction[Conflict.Faction.Mhoddim].Count;
        }

    }


    public void Raven()
    {

    }
}
