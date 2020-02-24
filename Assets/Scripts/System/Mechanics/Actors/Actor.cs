using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Actor : MonoBehaviour
{
    public const int rested_at = 5;

    // Inspector settings

    public LayerMask ground_layer;
    public Transform offhand_transform;
    public Transform weapon_transform;

    // properties

    public Actions Actions { get; set; }
    public Conflict.Alignment Alignment { get; set; }
    public Dialog Dialog { get; set; }
    public int ExhaustionLevel { get; set; } // TODO: create exhaustion class
    public Faction CurrentFaction { get; set; }
    public List<Faction> Factions { get; set; }
    public bool HasFullLoad { get; set; }
    public bool HasTask { get; set; }
    public Health Health { get; set; }
    public Interactable Interactions { get; set; }
    public Inventory Inventory { get; set; }
    public Actor Me { get; set; }
    public int RestCounter { get; set; }
    public Route Route { get; set; }
    public Senses Senses { get; set; }
    public Stats Stats { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void ChangeFaction(Faction new_faction)
    {
        if (!Factions.Contains(CurrentFaction)) Factions.Add(CurrentFaction);
        Me.CurrentFaction = new_faction;
        Renderer rend = GetComponent<Renderer>();
        rend.sharedMaterial.SetColor("_BaseColor", new_faction.colors);
    }


    public Vector3 GetHarassPoint(Actor other_unit)
    {
        if (other_unit == null || Me == null) return Vector3.zero;

        if (other_unit.Actions.Combat.EquippedRangedWeapon != null) {
            float melee_range = other_unit.Actions.Combat.MeleeRange();
            float long_range = other_unit.Actions.Combat.EquippedRangedWeapon.Range;
            float target_range = (long_range - melee_range) + (melee_range * 0.66f); // the edge of long range is too far, but within melee is too close
            Vector3 harass_point = (other_unit.transform.position - transform.position).normalized * target_range;
            Vector3 their_bottom = other_unit.GetComponent<Collider>().bounds.min;
            return new Vector3((transform.position + harass_point).x, their_bottom.y, (transform.position + harass_point).z);
        }
        return GetInteractionPoint(other_unit);
    }


    public Vector3 GetInteractionPoint(Actor other_unit)
    {
        if (other_unit == null || Me == null) return Vector3.zero;
        // The point on Me that other_unit will move to so that I am in their range
        Vector3 toward_approach = (other_unit.transform.position - transform.position).normalized * other_unit.Actions.Movement.StoppingDistance();
        Vector3 interaction_point = GetComponent<Collider>().ClosestPointOnBounds(other_unit.transform.position) + toward_approach;
        Vector3 their_bottom = other_unit.GetComponent<Collider>().bounds.min;

        return new Vector3(interaction_point.x, their_bottom.y, interaction_point.z);
    }

    public bool IsGrounded()
    {
        CapsuleCollider my_collider = GetComponentInChildren<CapsuleCollider>(); // TODO: in future, may not always be a capsule
        Vector3 my_base = new Vector3(my_collider.bounds.center.x, my_collider.bounds.min.y, my_collider.bounds.center.z);
        return Physics.CheckCapsule(my_collider.bounds.center, my_base, my_collider.radius * .9f, ground_layer);
    }


    public bool IsPlayer()
    {
        return Me.GetComponent<Player>() != null;
    }


    public float SeparationFrom(Transform target)
    {
        Actor actor = target.GetComponent<Actor>();
        Structure structure = target.GetComponent<Structure>();
        Vector3 their_closest_point_to_me = Vector3.zero;
        Vector3 their_bottom = Vector3.zero;
        Vector3 my_closest_point_to_them = Vector3.zero;
        Vector3 my_bottom = Vector3.zero;
        Vector3 structure_interaction_point = Vector3.zero;
        float separation = float.MaxValue;

        if (actor != null) {
            their_closest_point_to_me = actor.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            their_bottom = actor.GetComponent<Collider>().bounds.min;
            my_closest_point_to_them = GetComponent<Collider>().ClosestPointOnBounds(actor.transform.position);
            my_bottom = GetComponent<Collider>().bounds.min;
            separation = Vector3.Distance(new Vector3(my_closest_point_to_them.x, my_bottom.y, my_closest_point_to_them.z), new Vector3(their_closest_point_to_me.x, their_bottom.y, their_closest_point_to_me.z));
        } else if (structure != null) {
            structure_interaction_point = structure.GetInteractionPoint(Me);
            separation = Vector3.Distance(structure_interaction_point, transform.position);
        }

        return separation;
    }


    // private


    private void SetComponents()
    {
        Me = this;
        Actions = GetComponentInChildren<Actions>();
        Alignment = Conflict.Alignment.Unaligned;
        CurrentFaction = null;
        Dialog = GetComponent<Dialog>();
        ExhaustionLevel = 0;
        Factions = new List<Faction>();
        HasFullLoad = false;
        HasTask = false;
        Health = GetComponent<Health>();
        Interactions = GetComponent<Interactable>();
        Inventory = GetComponent<Inventory>();
        RestCounter = 0;
        Route = GetComponent<Route>();
        Senses = GetComponent<Senses>();
        Stats = GetComponent<Stats>();

        if (GetComponent<Faction>() != null) CurrentFaction = GetComponent<Faction>();
    }
}