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
    public Faction OccupyingFaction { get; set; }
    public Objective Objective { get; set; }

    // Unity

    private void OnTriggerExit(Collider other)
    {
        Actor actor = other.gameObject.GetComponent<Actor>();
        if (actor != null) {
            if (ClaimFaction != null) {
                if (actor.Faction.IsHostileTo(ClaimFaction)) {
                    if (Attackers.Contains(actor)) Attackers.Remove(actor);
                } else {
                    if (Defenders.Contains(actor)) Defenders.Remove(actor);
                }
            } else if (OccupyingFaction != null) {
                if (actor.Faction.IsHostileTo(OccupyingFaction)) {
                    if (Attackers.Contains(actor)) Attackers.Remove(actor);
                } else {
                    if (Defenders.Contains(actor)) Defenders.Remove(actor);
                }
            }
        } 
    }


    private void OnTriggerStay(Collider other)
    {
        Actor actor = other.gameObject.GetComponent<Actor>();
        if (actor != null) {
            if (ClaimFaction != null) {
                if (actor.Faction.IsHostileTo(ClaimFaction)) {
                    if (!Attackers.Contains(actor)) Attackers.Add(actor);
                } else {
                    if (!Defenders.Contains(actor)) Defenders.Add(actor);
                }
            }
            else if (OccupyingFaction != null)
            {
                if (actor.Faction.IsHostileTo(OccupyingFaction)) {
                    if (!Attackers.Contains(actor)) Attackers.Add(actor);
                } else {
                    if (!Defenders.Contains(actor)) Defenders.Add(actor);
                }
            }
        }
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
        OccupyingFaction = ClaimFaction = null;
        Claimed = false;
    }


    public float CurrentClaimPercentage() {
        return (float)CurrentClaimPoints / (float)maximum_claim_points;
    }


    // private


    private IEnumerator CheckClaim()
    {
        // Attackers and Defenders lists are managed by trigger stay/exit

        // TODO: we may want to alter the amount units boost or reduce the claim

        while (true) {
            int net_offense = Attackers.Count - Defenders.Count;

            if (net_offense > 0) {
                if (Claimed) {
                    ReduceClaim(net_offense);
                } else {
                    OccupyingFaction = Attackers.First().Faction;
                    SwapRoles();
                    BoostClaim(net_offense);
                }
            } else if (net_offense < 0) {
                if (Claimed) {
                    BoostClaim(Mathf.Abs(net_offense));
                } else { // if claim was lost, then the defenders "probably" won't end up here, but would instead become attackers...
                    OccupyingFaction = Defenders.First().Faction;
                    BoostClaim(Mathf.Abs(net_offense));
                }
            }

            UpdateClaimBar();
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private void BoostClaim(int boost)
    {
        if (CurrentClaimPoints >= maximum_claim_points) return;

        CurrentClaimPoints += boost;
        if (CurrentClaimPoints >= maximum_claim_points) {
            CurrentClaimPoints = maximum_claim_points;
            ClaimFaction = OccupyingFaction;
            Claimed = true;
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


    private void ReduceClaim(int reduction)
    {
        CurrentClaimPoints -= reduction;
        if (CurrentClaimPoints <= 0) ClearAllClaim();
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
        OccupyingFaction = ClaimFaction;
    }


    private void SwapRoles()
    {
        Attackers.Clear();
        Defenders.Clear();
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