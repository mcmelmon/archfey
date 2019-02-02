using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Objective : MonoBehaviour
{
    // Inspector settings

    public Conflict.Alignment initial_claim;
    public List<ClaimNode> claim_nodes;

    // properties

    public Conflict.Alignment Claim { get; set; }
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
        return Claimed && unit != null && Claim == unit.Alignment || Claim == Conflict.Alignment.Neutral;
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
            Conflict.Alignment new_faction = Conflict.Alignment.Unaligned;
            Conflict.Alignment previous_faction = Claim;

            foreach (var node in claim_nodes) {
                Conflict.Alignment point_faction = node.ClaimFaction;

                if (new_faction == Conflict.Alignment.Unaligned)
                    // We have just entered the loop
                    new_faction = point_faction;

                if ((point_faction == Conflict.Alignment.Unaligned)) {
                    new_faction = Conflict.Alignment.Unaligned;
                    break;
                } else if (new_faction != point_faction) {
                    new_faction = Conflict.Alignment.Unaligned;
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
        Claim = initial_claim;
        Claimed = Claim != Conflict.Alignment.Unaligned;
    }


    private void TransferClaim(Conflict.Alignment new_faction, Conflict.Alignment previous_faction)
    {
        if (new_faction != Conflict.Alignment.Unaligned) {
            Claimed = true;
            Claim = new_faction;
        }
    }
}