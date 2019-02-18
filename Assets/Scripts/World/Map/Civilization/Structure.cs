using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public enum Purpose { Civic = 0, Commercial = 1, Industrial = 2, Military = 3, Residential = 4, Sacred = 5 };

    // Inspector settings
    public Conflict.Alignment alignment;
    public Purpose purpose;

    public int armor_class = 13;
    public int damage_resistance;
    public int maximum_hit_points = 100;

    public List<Transform> entrances = new List<Transform>();

    public float revenue_cp;

    // properties

    public List<Actor> AttachedUnits { get; set; }
    public int CurrentHitPoints { get; set; }
    public Faction Faction { get; set; }
    public float OriginalY { get; set; }
    public float OriginalYScale { get; set; }
    public Dictionary<Weapons.DamageType, int> Resistances { get; set; }
    public Storage Storage { get; set; }
    public Workshop Workshop { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
        StartCoroutine(PruneAttachedUnits());
    }

    // public


    public float CurrentHitPointPercentage()
    {
        return ((float)CurrentHitPoints / (float)maximum_hit_points);
    }


    public void DeliverMaterials(Actor _unit, float _amount)
    {
        BookRevenue(_amount);
        if (Storage != null)
            Storage.StoreMaterials(_unit);
    }


    public void GainStructure(int _amount)
    {
        if (CurrentHitPoints == maximum_hit_points) return;

        CurrentHitPoints += _amount;
        if (CurrentHitPoints > maximum_hit_points) CurrentHitPoints = maximum_hit_points;
        UpdateStructure();
    }


    public void LoseStructure(int amount, Weapons.DamageType type)
    {
        if (CurrentHitPoints == 0) return;

        int reduced_amount = (amount - damage_resistance > 0) ? amount - damage_resistance : 0;
        CurrentHitPoints -= DamageAfterResistance(reduced_amount, type);
        if (CurrentHitPoints <= 0) CurrentHitPoints = 0;
        UpdateStructure();
    }


    public List<string> MaterialsWanted()
    {
        if (Storage != null) {
            var wanted_materials = Storage.materials.Select(s => s.material);
            return wanted_materials.ToList();
        }

        return null;
    }


    public Transform NearestEntranceTo(Transform _location)
    {
        return entrances.OrderBy(s => Vector3.Distance(transform.position, _location.position)).Reverse().ToList().First();
    }


    public Transform RandomEntrance()
    {
        Transform entrance = null;

        if (entrances.Count > 0)
            entrance = entrances[Random.Range(0, entrances.Count)];

        return entrance;
    }


    // private


    private void BookRevenue(float amount)
    {
        // TODO: accumulate revenue
    }


    private int DamageAfterResistance(int damage, Weapons.DamageType type)
    {
        return (damage <= 0 || Resistances == null) ? damage : (damage -= damage * (Resistances[type] / 100));
    }


    private IEnumerator PruneAttachedUnits()
    {
        while (true) {
            yield return new WaitForSeconds(Turn.ActionThreshold);

            for (int i = 0; i < AttachedUnits.Count; i++) {
                // use for loop to avoid modifying collection in foreach
                if (AttachedUnits[i] == null) {
                    AttachedUnits.Remove(AttachedUnits[i]);
                }
            }
        }
    }


    private void SetComponents()
    {
        AttachedUnits = new List<Actor>();
        CurrentHitPoints = maximum_hit_points;
        Faction = GetComponentInParent<Faction>();
        OriginalY = transform.position.y;
        OriginalYScale = transform.localScale.y;
        Resistances = new Dictionary<Weapons.DamageType, int>  // TODO: override some reistances
        {
            [Weapons.DamageType.Acid] = 0,
            [Weapons.DamageType.Bludgeoning] = 25,
            [Weapons.DamageType.Cold] = 25,
            [Weapons.DamageType.Fire] = -50,
            [Weapons.DamageType.Force] = 0,
            [Weapons.DamageType.Lightning] = 0,
            [Weapons.DamageType.Necrotic] = 0,
            [Weapons.DamageType.Piercing] = 50,
            [Weapons.DamageType.Poison] = 100,
            [Weapons.DamageType.Psychic] = 100,
            [Weapons.DamageType.Slashing] = 25,
            [Weapons.DamageType.Thunder] = 0
        };
        Storage = GetComponent<Storage>();
        Workshop = GetComponent<Workshop>();
    }


    private void UpdateStructure()
    {
        if (CurrentHitPoints == maximum_hit_points) return;

        Vector3 scaling = transform.localScale;
        Vector3 position = transform.position;

        switch (CurrentHitPointPercentage()) {
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
