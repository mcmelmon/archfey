using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestingNode : MonoBehaviour
{
    // Inspector settings
    [SerializeField] Resource harvested_material;
    [SerializeField] Faction faction;
    [SerializeField] bool perpetual;
    [SerializeField] int initial_quantity;

    // properties

    public int CurrentlyAvailable { get; set; }
    public int FullHarvest { get; set; }
    public Resource HarvestedMaterial { get; set; }
    public float OriginalY { get; set; } // TODO: Come up with a better way to indicate destruction/depletion
    public float OriginalYScale { get; set; }
    public Faction Owner { get; set; }
    public bool Perpetual { get; set; }
    public Structure Structure { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public

    public bool AccessibleTo(Actor _unit)
    {
        foreach (var tool in _unit.Stats.Tools) {
            if (HarvestedMaterial.RequiredTools.Contains(tool)) return true;
        }

        return false;
    }

    public bool HarvestResource(Actor _harvestor)
    {
        if (!Proficiencies.Instance.IsHarvester(_harvestor)) return false;

        if (HarvestedMaterial.HarvestBy(_harvestor)) {
            CurrentlyAvailable -= 1;
            return true;
        }
        _harvestor.HasTask = false;
        return false;
    }


    // private


    private float CurrentlyAvailablePercentage()
    {
        return ((float)CurrentlyAvailable / (float)initial_quantity);
    }


    private void SetComponents()
    {
        CurrentlyAvailable = initial_quantity;
        HarvestedMaterial = harvested_material;
        OriginalY = transform.position.y;
        OriginalYScale = transform.localScale.y;
        Owner = faction;
        Perpetual = perpetual;
        Structure = GetComponent<Structure>();
    }


    private void UpdateResource()
    {
        if (CurrentlyAvailable == initial_quantity) return;

        Vector3 scaling = transform.localScale;
        Vector3 position = transform.position;

        switch (CurrentlyAvailablePercentage())
        {
            case float n when (n >= 0.33f && n <= 0.66f):
                scaling.y = OriginalYScale * 0.66f;
                transform.position = new Vector3(transform.position.x, OriginalY * 0.66f, transform.position.z);
                break;
            case float n when (n >= 0.1f && n < 0.33f):
                scaling.y = OriginalYScale * 0.33f;
                transform.position = new Vector3(transform.position.x, OriginalY * 0.33f, transform.position.z);
                break;
            case float n when (n < 0.1f):
                scaling.y = OriginalYScale * 0.01f;
                transform.position = new Vector3(transform.position.x, OriginalY * 0.01f, transform.position.z);
                break;
        }

        transform.localScale = scaling;
    }
}
