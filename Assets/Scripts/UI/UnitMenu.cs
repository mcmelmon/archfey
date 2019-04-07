using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UnitMenu : MonoBehaviour
{

    // Inspector settings
    public List<FactionBlock> factions;

    [Serializable]
    public struct FactionBlock
    {
        public Faction faction;
        public Slider slider;
        public Image fill;
    }

    // Unity

    private void Awake()
    {
        StartCoroutine(UpdateFactionCounts());
    }


    // private


    private float FactionPercentage(Faction faction)
    {
        float faction_count = FindObjectsOfType<Actor>().Count(actor => actor.CurrentFaction == faction);
        return faction_count / TotalUnits();
    }


    private void FillColor(Image fill, Faction faction)
    {
        fill.color = new Color(faction.colors.r, faction.colors.g, faction.colors.b, 1f);
    }


    private float TotalUnits()
    {
        float total_units = FindObjectsOfType<Spawner>().Select(spawner => spawner.TotalUnitsAvailable()).Sum();
        return total_units;
    }


    private IEnumerator UpdateFactionCounts()
    {
        while (true) {
            foreach (var faction_block in factions) {
                faction_block.slider.value = FactionPercentage(faction_block.faction);
                FillColor(faction_block.fill, faction_block.faction);
            }

            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }
}
