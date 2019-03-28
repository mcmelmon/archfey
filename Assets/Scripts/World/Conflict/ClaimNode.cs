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
    public Spawner node_spawner;


    // properties

    public List<Actor> Attackers { get; set; }
    public bool Claimed { get; set; }
    public float CurrentClaimPoints { get; set; }
    public List<Actor> Defenders { get; set; }
    public SphereCollider InfluenceZone { get; set; }
    public Faction NodeFaction { get; set; }
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
        NodeFaction = null;
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
            SetAttackAndDefense();

            int net_offense = Attackers.Count - Defenders.Count;   // Attackers and Defenders lists are managed by trigger stay/exit

            if (net_offense != 0) {
                Faction attacking_faction = Attackers.Any() ? Attackers.First().Faction : null;
                Faction defending_faction = Defenders.Any() ? Defenders.First().Faction : null;

                if (net_offense > 0) {
                    if (Claimed && NodeFaction != attacking_faction) {
                        ReduceClaim(net_offense);
                    } else if (!Claimed) {
                        BoostClaim(net_offense, attacking_faction);
                    }
                } else if (net_offense < 0) {
                    if (Claimed && NodeFaction == defending_faction) {
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
            NodeFaction = boosting_faction;
            if (node_spawner != null) {
                node_spawner.faction = boosting_faction;
            }
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
        if (NodeFaction != null) {
            NodeFaction.LoseTotalObjectiveControl();
        }
    }


    private void SetAttackAndDefense()
    {
        Attackers.Clear();
        Defenders.Clear();

        List<Actor> units_within_range = FindObjectsOfType<Actor>().
            Where(actor => Vector3.Distance(actor.transform.position, transform.position) < influence_zone_radius).
            ToList();

        if (NodeFaction != null) {
            // attack and defense is determined by who currently claims the node

            for (int i = 0; i < units_within_range.Count; i++) {
                Actor actor = units_within_range[i];

                if (actor != null) {
                    if (actor.Faction.IsHostileTo(NodeFaction)) {
                        if (!Attackers.Contains(actor) && actor != null) Attackers.Add(actor);
                    } else {
                        if (!Defenders.Contains(actor) && actor != null) Defenders.Add(actor);
                    }
                }
            }
        } else {
            // the attackers are the faction with the most units; everybody else can try to kill them
            // NOTE: the faction that has the most units for the last boost gains the node (of course, then they have to keep it)

            var faction_units = units_within_range
                .GroupBy(unit => unit.Faction,
                        (faction, factions) => new {
                            Key = faction,
                            Count = factions.Count()
                        })
                .OrderByDescending(faction => faction.Count);

            Attackers.AddRange(units_within_range.Where(unit => unit.Faction == faction_units.First().Key));
        }
    }


    private void SetComponents()
    {
        Objective = GetComponentInParent<Objective>();  // define before referencing!

        Attackers = new List<Actor>();
        Claimed = Objective.initial_claim != null;
        NodeFaction = Objective.initial_claim;
        CurrentClaimPoints = (NodeFaction == null) ? 0 : maximum_claim_points;
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