using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestingNode : MonoBehaviour
{
    // Inspector settings
    public string material;
    public Conflict.Alignment owner;
    public bool perpetual;
    public int initial_quantity;
    public int harvest_increment;
    public int full_harvest;
    public List<string> required_tools;

    // properties

    public int CurrentlyAvailable { get; set; }
    public float OriginalY { get; set; }
    public float OriginalYScale { get; set; }
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
            if (required_tools.Contains(tool)) return true;
        }

        return false;
    }


    public void HarvestResource(Actor _harvestor)
    {
        if (!Proficiencies.Instance.Harvester(_harvestor)) return;

        int optimal_harvest = harvest_increment * _harvestor.Stats.ProficiencyBonus;
        int harvested = optimal_harvest;

        if (Structure != null) {
            // TODO: reduce harvested amount if structure damaged
        }

        if (_harvestor.Load.ContainsKey(this) && _harvestor.Load[this] + harvested > full_harvest) 
            harvested = full_harvest - _harvestor.Load[this];

        if (!perpetual && harvested > CurrentlyAvailable) harvested = CurrentlyAvailable;
        CurrentlyAvailable -= perpetual ? 0 : harvested;

        if (_harvestor.Load.ContainsKey(this))
        {
            _harvestor.Load[this] += harvested;
        } else {
            _harvestor.Load[this] = harvested;
        }
    }


    // private


    private float CurrentlyAvailablePercentage()
    {
        return ((float)CurrentlyAvailable / (float)initial_quantity);
    }


    private void SetComponents()
    {
        CurrentlyAvailable = initial_quantity;
        OriginalY = transform.position.y;
        OriginalYScale = transform.localScale.y;
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
