using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NodeCounter : MonoBehaviour
{

    // Inspector settings
    public List<UnitCounter.FactionBlock> factions;


    // Unity

    private void Awake()
    {
        StartCoroutine(UpdateFactionCounts());
    }


    // private


    private float FactionPercentage(Faction faction)
    {
        float faction_count = FindObjectsOfType<ClaimNode>().Count(spawner => spawner.NodeFaction == faction);
        return faction_count / TotalNodes();
    }


    private void FillColor(Image fill, Faction faction)
    {
        fill.color = new Color(faction.colors.r, faction.colors.g, faction.colors.b, 1f);
    }


    private float TotalNodes()
    {
        float total_nodes = FindObjectsOfType<ClaimNode>().Count();
        return total_nodes;
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
