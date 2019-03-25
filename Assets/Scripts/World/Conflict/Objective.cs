using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Objective : MonoBehaviour
{
    // Inspector settings

    public Faction initial_claim;
    public List<ClaimNode> claim_nodes;

    // properties

    public Faction ClaimingFaction { get; set; }
    public bool Claimed { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    private void Start()
    {
        StartCoroutine(CheckClaim());
    }


    // public


    public bool IsFriendlyTo(Actor unit)
    {
        return Claimed && unit != null && ClaimingFaction == unit.Faction || !ClaimingFaction.IsHostileTo(unit.Faction);
    }


    // private


    private float AverageControlPercentage()
    {
        float[] claims = new float[claim_nodes.Count];
        float sum = 0;
        for (int i = 0; i < claim_nodes.Count; i++) {
            sum += claim_nodes[i].CurrentClaimPercentage();
        }

        return sum / claim_nodes.Count;
    }


    private IEnumerator CheckClaim()
    {
        while (true) {
            Faction new_faction = null;
            Faction previous_faction = ClaimingFaction;

            foreach (var node in claim_nodes) {
                Faction node_faction = node.ClaimFaction;

                if (new_faction == null)
                    // We have just entered the loop
                    new_faction = node_faction;

                if ((node_faction == null)) {
                    new_faction = null;
                    break;
                } else if (new_faction != node_faction) {
                    new_faction = null;
                    break;
                }
            }

            if (new_faction != previous_faction) {
                TransferClaim(new_faction, previous_faction);
            }
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private void SetComponents()
    {
        ClaimingFaction = initial_claim;
        Claimed = ClaimingFaction != null;
    }


    private void TransferClaim(Faction new_faction, Faction previous_faction)
    {
        if (new_faction != null) {
            Claimed = true;
            ClaimingFaction = new_faction;
        }
    }
}