using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Objective : MonoBehaviour
{
    // Inspector settings

    public Conflict.Faction initial_claim;
    public List<ClaimNode> claim_nodes;

    // properties

    public Conflict.Faction Claim { get; set; }
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


    public bool IsFriendlyTo(GameObject _unit)
    {
        if (!Claimed || _unit == null) return false;

        switch (Claim) {
            case Conflict.Faction.Ghaddim:
                return _unit.GetComponent<Ghaddim>() != null;
            case Conflict.Faction.Mhoddim:
                return _unit.GetComponent<Mhoddim>() != null;
            default:
                return false;
        }
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
            Conflict.Faction new_faction = Conflict.Faction.None;
            Conflict.Faction previous_faction = Claim;

            foreach (var control_point in claim_nodes) {
                Conflict.Faction point_faction = control_point.ClaimFaction;

                if (new_faction == Conflict.Faction.None)
                    // We have just entered the loop
                    new_faction = point_faction;

                if ((point_faction == Conflict.Faction.None)) {
                    new_faction = Conflict.Faction.None;
                    break;
                } else if (new_faction != point_faction) {
                    new_faction = Conflict.Faction.None;
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
        Claimed = Claim != Conflict.Faction.None;
    }


    private void TransferClaim(Conflict.Faction new_faction, Conflict.Faction previous_faction)
    {
        Claimed = true;
        Claim = new_faction;

        Objectives.Instance.AccountForClaim(new_faction, previous_faction, this);
        ObjectiveControlUI.Instance.ChangeClaim(Claim, previous_faction);
        ObjectiveControlUI.Instance.MostRecentFlip = this;
    }
}