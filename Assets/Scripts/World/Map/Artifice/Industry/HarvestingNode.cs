using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HarvestingNode : MonoBehaviour
{
    public enum ReplenishStrategy { Once, Random, Perpetual, Proximity, Timer };

    // Inspector settings
    [SerializeField] int perception_challenge_rating = 10;
    [SerializeField] ReplenishStrategy replenish_strategy = ReplenishStrategy.Once;
    [SerializeField] int replenish_delay = 10;
    [SerializeField] int initial_quantity = 10;  // the quantity of total chances to get a resource in the node; not the quantity of one particular resource (unless there is only one)
    [SerializeField] List<Resource> available_for_harvest = new List<Resource>();  // to simulate rareness, have multiple copies of 'common" and fewer copies of "rare"


    // properties

    public int ChallengeRating { get; set; }
    public int CurrentlyAvailable { get; set; }
    public System.Action OnQuantityChange {get; set; }
    public ReplenishStrategy Replenishes { get; set; }
    public int ReplenishTurn { get; set; }
    public Structure Structure { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
        StartCoroutine(Replenish());
    }


    // public

    public bool HarvestResource(Actor _harvester)
    {
        if (!Proficiencies.Instance.IsHarvester(_harvester) || CurrentlyAvailable <= 0) return false;

        if (SelectHarvestFor(_harvester).HarvestBy(_harvester)) {
            CurrentlyAvailable -= 1;
            OnQuantityChange?.Invoke();
            return true;
        }
        return false;
    }

    public bool IsAccessibleTo(Actor _harvester)
    {
        foreach (var resource in available_for_harvest) {
            if (resource.IsAccessibleTo(_harvester)) return true;
        }

        return false;
    }

    public void SetInitialQuantity(int _quantity)
    {
        initial_quantity = CurrentlyAvailable = _quantity;
    }


    // private

    private IEnumerator Replenish()
    {
        while (true) {
            if (CurrentlyAvailable <= 0 && replenish_strategy == ReplenishStrategy.Timer) {
                if (ReplenishTurn >= replenish_delay ) {
                    CurrentlyAvailable = initial_quantity;
                    ReplenishTurn = 0;
                } else {
                    ReplenishTurn++;
                }
            }

            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }

    private Resource SelectHarvestFor(Actor _harvester)
    {
        List<Resource> available = available_for_harvest.Where(r => r.IsAccessibleTo(_harvester)).ToList();
        Random.InitState(System.DateTime.Now.Second);
        return available[Random.Range(0, available.Count)];
    }

    private void SetComponents()
    {
        ChallengeRating = perception_challenge_rating;
        CurrentlyAvailable = initial_quantity;
        Replenishes = replenish_strategy;
        ReplenishTurn = 0;
        Structure = GetComponent<Structure>();
    }
}
