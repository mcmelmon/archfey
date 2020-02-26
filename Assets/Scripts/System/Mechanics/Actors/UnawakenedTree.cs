using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnawakenedTree : MonoBehaviour
{

    // Inspector settings

    [SerializeField] HarvestingNode herb = null;
    [SerializeField] HarvestingNode timber = null;
    
    // properties

    public HarvestingNode HerbNode { get; set; }
    public HarvestingNode TimberNode { get; set; }
    public Actor Me { get; set; }
    // Unity

    private void Awake()
    {
        SetComponents();
        SetActions();
    }

    private void Start()
    {
        StartCoroutine(SetStatsWhenReady());
    }

    // public

    public void OnHealthChange()
    {
        // We do this in order to account for the tree being healed; or, damaged in combat
        TimberNode.SetInitialQuantity(Me.Health.CurrentHitPoints);
    }

    public void OnQuantityChange()
    {
        int change = TimberNode.CurrentlyAvailable - Me.Health.CurrentHitPoints;

        if (change == 0) return;

        if (change > 0) {
            Me.Health.RecoverHealth(Mathf.Abs(change));
        } else {
            Me.Health.LoseHealth(Mathf.Abs(change));
        }
    }


    // private

    private IEnumerator SetStatsWhenReady()
    {
        while (Me.Health.CurrentHitPoints == 0) {
            yield return null;
        }

        SetStats();
    }

    private void SetActions()
    {

    }

    private void SetComponents()
    {
        Me = GetComponent<Actor>();

        HerbNode = herb;
        TimberNode = timber;
    }

    private void SetStats()
    {
        SetAdditionalStats();
        TimberNode.SetInitialQuantity(Me.Health.CurrentHitPoints);
    }


    private void SetAdditionalStats()
    {
        Me.Stats.Resistances = new Dictionary<Weapons.DamageType, Stats.ResistanceLevels> {
            [Weapons.DamageType.Bludgeoning] = Stats.ResistanceLevels.Resistant,
            [Weapons.DamageType.Fire] = Stats.ResistanceLevels.Vulnerable,
            [Weapons.DamageType.Piercing] = Stats.ResistanceLevels.Resistant,
        };

        Me.Health.AddHitDice(Me.Health.HitDice.First().Key, (int)transform.localScale.y - 1);
        Me.Health.OnHealthChange = OnHealthChange;
        TimberNode.OnQuantityChange = OnQuantityChange;
    }
}
