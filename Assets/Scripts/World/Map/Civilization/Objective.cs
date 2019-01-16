using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Objective : MonoBehaviour
{
    // Inspector settings
    [Header("Claim")]
    // Claim covers which faction benefits from controling the objective
    public Conflict.Faction initial_claim;
    public List<ClaimNode> claim_nodes;

    [Header("Rendering")]
    // Rendering tints the objective based on controling faction
    public Material ghaddim_skin;
    public Material mhoddim_skin;
    public Material unclaimed_skin;
    public List<Renderer> renderers;

    // properties

    public Conflict.Faction Claim { get; set; }
    public bool Claimed { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
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
        for (int i = 0; i < claim_nodes.Count; i++)
        {
            sum += claim_nodes[i].CurrentClaimPercentage();
        }

        return sum / claim_nodes.Count;
    }


    private IEnumerator CheckClaim()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.ActionThreshold);

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
        }
    }


    private void SetComponents()
    {
        Claim = initial_claim;
        Claimed = Claim != Conflict.Faction.None;
        StartCoroutine(CheckClaim());

        foreach (var rend in renderers) {
            rend.material = (initial_claim == Conflict.Faction.None) ? unclaimed_skin : (initial_claim == Conflict.Faction.Ghaddim) ? ghaddim_skin : mhoddim_skin;
        }
    }


    private void TransferClaim(Conflict.Faction new_faction, Conflict.Faction previous_faction)
    {
        Claimed = true;
        Claim = new_faction;

        switch (Claim) {
            case Conflict.Faction.Ghaddim:
                GetComponentInChildren<Renderer>().material = ghaddim_skin;
                break;
            case Conflict.Faction.Mhoddim:
                GetComponentInChildren<Renderer>().material = mhoddim_skin;
                break;
            case Conflict.Faction.None:
                GetComponentInChildren<Renderer>().material = unclaimed_skin;
                break;
        }

        Objectives.Instance.AccountForClaim(new_faction, previous_faction, this);
        ObjectiveControlUI.Instance.ChangeClaim(Claim, previous_faction);
        ObjectiveControlUI.Instance.MostRecentFlip = this;
    }
}