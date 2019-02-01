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

    // properties

    public List<Actor> Attackers { get; set; }
    public bool Claimed { get; set; }
    public Conflict.Faction ClaimFaction { get; set; }
    public float CurrentClaimPoints { get; set; }
    public List<Actor> Defenders { get; set; }
    public Conflict.Faction OccupyingFaction { get; set; }
    public Objective Objective { get; set; }

    // Unity


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
        OccupyingFaction = ClaimFaction = Conflict.Faction.None;
        Claimed = false;
    }


    public void ClearAttackers()
    {
        Attackers.Clear();
    }


    public void ClearDefenders()
    {
        Defenders.Clear();
    }


    public float CurrentClaimPercentage() {
        return (float)CurrentClaimPoints / (float)maximum_claim_points;
    }


    // private


    private IEnumerator CheckClaim()
    {
        while (true) {
            IdentifyFriendAndFoe();

            // don't use foreach because the units will get destroyed in flight
            for (int i = 0; i < Defenders.Count; i++) {
                BoostClaim(Defenders[i]);
            }

            for (int i = 0; i < Attackers.Count; i++) {
                ReduceClaim(Attackers[i]);
            }

            UpdateClaimBar();
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private void BoostClaim(Actor defender)
    {
        if (defender == null) return;

        if (CurrentClaimPoints >= maximum_claim_points) return;

        CurrentClaimPoints += defender.Stats.ProficiencyBonus;
        if (CurrentClaimPoints >= maximum_claim_points) {
            CurrentClaimPoints = maximum_claim_points;
            ClaimFaction = OccupyingFaction = defender.Faction;
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


    private void IdentifyFriendAndFoe()
    {
        Attackers = FindObjectsOfType<Actor>()
            .Where(actor => actor.Faction != ClaimFaction && Vector3.Distance(actor.transform.position, transform.position) < Movement.ReachedThreshold)
            .ToList();

        Defenders = FindObjectsOfType<Actor>()
            .Where(actor => actor.Faction == ClaimFaction && Vector3.Distance(actor.transform.position, transform.position) < Movement.ReachedThreshold)
            .ToList();
    }


    private void ReduceClaim(Actor attacker)
    {
        if (attacker == null) return;

        if (CurrentClaimPoints <= 0) {
            ClearAllClaim();
            OccupyingFaction = attacker.Faction;
            BoostClaim(attacker);
        } else {
            CurrentClaimPoints -= attacker.Stats.ProficiencyBonus;
            if (CurrentClaimPoints <= 0) ClearAllClaim();
        }
    }


    private void SetComponents()
    {
        Objective = GetComponentInParent<Objective>();  // define before referencing!

        Attackers = new List<Actor>();
        Claimed = Objective.initial_claim != Conflict.Faction.None;
        ClaimFaction = Objective.initial_claim;
        CurrentClaimPoints = (ClaimFaction == Conflict.Faction.None) ? 0 : maximum_claim_points;
        Defenders = new List<Actor>();
        OccupyingFaction = ClaimFaction;
    }


    public void UpdateClaimBar()
    {
        claim_indicator.value = CurrentClaimPercentage();
        if (Mathf.Approximately(claim_indicator.value, 0) || Mathf.Approximately(claim_indicator.value, 1)) {
            claim_indicator.gameObject.SetActive(false);
        } else {
            claim_indicator.gameObject.SetActive(true);
        }
    }
}