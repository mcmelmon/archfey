using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    // properties
    public HarvestingNode HarvestingNode { get; set; }
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
        HarvestingNode.SetInitialQuantity(Me.Health.CurrentHitPoints);
    }

    public void OnQuantityChange()
    {
        int change = HarvestingNode.CurrentlyAvailable - Me.Health.CurrentHitPoints;

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
        HarvestingNode = Me.GetComponentInChildren<HarvestingNode>();
    }

    private void SetStats()
    {
        SetAdditionalStats();
        HarvestingNode.SetInitialQuantity(Me.Health.CurrentHitPoints);
    }


    private void SetAdditionalStats()
    {
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
        Me.Stats.Resistances[Weapons.DamageType.Bludgeoning] = 50;
        Me.Stats.Resistances[Weapons.DamageType.Fire] = 200;
        Me.Stats.Resistances[Weapons.DamageType.Piercing] = 50;

        Me.Health.OnHealthChange = OnHealthChange;
        HarvestingNode.OnQuantityChange = OnQuantityChange;
    }
}
