using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ClaimNode : MonoBehaviour
{
    // Inspector settings

    public int maximum_claim_points = 50;
    public Slider claim_indicator;
    public float influence_zone_radius = 10f;

    // properties

    public List<Actor> Attackers { get; set; }
    public bool Claimed { get; set; }
    public Faction ClaimFaction { get; set; }
    public float CurrentClaimPoints { get; set; }
    public List<Actor> Defenders { get; set; }
    public SphereCollider InfluenceZone { get; set; }
    public Objective Objective { get; set; }

    // Unity

    private void OnTriggerExit(Collider other)
    {
        Actor actor = other.gameObject.GetComponent<Actor>();
        if (actor != null) {
            if (ClaimFaction != null) {
                if (actor.Faction.IsHostileTo(ClaimFaction)) {
                    if (Attackers.Contains(actor) || actor == null) Attackers.Remove(actor);
                } else {
                    if (Defenders.Contains(actor) || actor == null) Defenders.Remove(actor);
                }
            } 
        }
        PruneAttackAndDefense();
    }


    private void OnTriggerStay(Collider other)
    { 
        Actor actor = other.gameObject.GetComponent<Actor>();
        if (actor != null) {
            if (ClaimFaction != null) {
                if (actor.Faction.IsHostileTo(ClaimFaction)) {
                    if (!Attackers.Contains(actor) && actor != null) Attackers.Add(actor);
                } else {
                    if (!Defenders.Contains(actor) && actor != null) Defenders.Add(actor);
                }
            }
        }

        PruneAttackAndDefense();
    }


    private void Start()
    {
        SetComponents();
        UpdateClaimBar();
        StartCoroutine(CheckClaim());
        StartCoroutine(ClaimBarFaceCamera());
    }


    // public


    public void ClearAllClaim()
    {
        CurrentClaimPoints = 0;
        ClaimFaction = null;
        Claimed = false;
    }


    public float CurrentClaimPercentage() {
        return (float)CurrentClaimPoints / (float)maximum_claim_points;
    }


    // private


    private IEnumerator CheckClaim()
    {

        // TODO: we may want to alter the amount units boost or reduce the claim

        while (true) {
            int net_offense = Attackers.Count - Defenders.Count;   // Attackers and Defenders lists are managed by trigger stay/exit

            if (net_offense != 0)
            {
                Faction attacking_faction = Attackers.Any() ? Attackers.First().Faction : null;
                Faction defending_faction = Defenders.Any() ? Defenders.First().Faction : null;

                if (net_offense > 0)
                {
                    if (Claimed && ClaimFaction != attacking_faction)
                    {
                        ReduceClaim(net_offense);
                    }
                    else if (!Claimed)
                    {
                        BoostClaim(net_offense, attacking_faction);
                    }
                }
                else if (net_offense < 0)
                {
                    if (Claimed && ClaimFaction == defending_faction)
                    {
                        BoostClaim(Mathf.Abs(net_offense), defending_faction);
                    }
                }

                UpdateClaimBar();
            }
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private void BoostClaim(int boost, Faction boosting_faction)
    {
        if (CurrentClaimPoints >= maximum_claim_points) return;

        CurrentClaimPoints += boost;
        if (CurrentClaimPoints >= maximum_claim_points) {
            CurrentClaimPoints = maximum_claim_points;
            Claimed = true;
            ClaimFaction = boosting_faction;
        }
    }


    private IEnumerator ClaimBarFaceCamera()
    {
        while (true) {
            yield return null;
            Vector3 control_position = transform.position;
            Vector3 player_position = Player.Instance.viewport.transform.position;

            Quaternion rotation = Quaternion.LookRotation(player_position - control_position, Vector3.up);
            claim_indicator.transform.rotation = rotation;
        }
    }


    private void PruneAttackAndDefense()
    {
        if (Attackers.Any())
        {
            for (var i = Attackers.Count -1; i >= 0; i--)
            {
                if (Attackers[i] == null) Attackers.RemoveAt(i);
            }
        }

        if (Defenders.Any())
        {
            for (var i = Defenders.Count - 1; i >= 0; i--)
            {
                if (Defenders[i] == null) Defenders.RemoveAt(i);
            }
        }
    }


    private void ReduceClaim(int reduction)
    {
        CurrentClaimPoints -= reduction;
        if (CurrentClaimPoints <= 0) ClearAllClaim();
        if (ClaimFaction != null) {
            ClaimFaction.LoseTotalObjectiveControl();
        }
    }


    private void SetComponents()
    {
        Objective = GetComponentInParent<Objective>();  // define before referencing!

        Attackers = new List<Actor>();
        Claimed = Objective.initial_claim != null;
        ClaimFaction = Objective.initial_claim;
        CurrentClaimPoints = (ClaimFaction == null) ? 0 : maximum_claim_points;
        Defenders = new List<Actor>();
        InfluenceZone = GetComponent<SphereCollider>();
        InfluenceZone.radius = influence_zone_radius;
    }


    private void UpdateClaimBar()
    {
        claim_indicator.value = CurrentClaimPercentage();
        if (Mathf.Approximately(claim_indicator.value, 0) || Mathf.Approximately(claim_indicator.value, 1)) {
            claim_indicator.gameObject.SetActive(false);
        } else {
            claim_indicator.gameObject.SetActive(true);
        }
    }
}