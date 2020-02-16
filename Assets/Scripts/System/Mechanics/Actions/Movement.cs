using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    public enum CommonDestination { Home, Harvest, Craft, Military, Repair, Warehouse };

    // properties

    public float BaseSpeed { get; set; }
    public bool IsDashing { get; set; }
    public Actor Me { get; set; }
    public NavMeshAgent Agent { get; set; }
    public Vector3 CurrentDestination { get; set; }
    public Dictionary<CommonDestination, Vector3> Destinations { get; set; }
    public bool Encumbered { get; set; }
    public bool IsJumping { get; set; }
    public float ReachedThreshold { get; set; }
    public float SpeedAdjustment { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
        StartCoroutine(IsEncumbered());
    }


    // public


    public void AddDestination(CommonDestination key, Vector3 destination)
    {
        if (!Destinations.ContainsKey(key)) {
            Destinations[key] = destination;
        }
    }

    public bool AtCurrentDestination()
    {
        return CurrentDestination != Vector3.zero && Vector3.Distance(Me.transform.position, CurrentDestination) < ReachedThreshold + 1.5f; // account for navmesh obstacle buffer
    }

    public void AdjustSpeed(float boost)
    {
        // boost is a percentage that will get doubled to calculate effect
        // boost of 0.5f will result in 2x movement
        SpeedAdjustment += boost;
        if (SpeedAdjustment > 1f) SpeedAdjustment = 1f;
        if (SpeedAdjustment < -0.5f) SpeedAdjustment = -0.5f;
        Agent.speed = GetAdjustedSpeed();
    }

    public void ClearCurrentDestination()
    {
        CurrentDestination = Vector3.zero;
    }

    public void Dash()
    {
        if (!IsDashing) StartCoroutine(Dashing());
    }

    public void Disengage()
    {
        Vector3 backward = transform.forward * -1;
        Agent.enabled = false;
        Me.transform.position += backward * 10f;
        Agent.enabled = true;

        Me.Actions.SheathWeapon();
    }

    public float GetAdjustedSpeed()
    {
        float raw_speed = BaseSpeed + (SpeedAdjustment * BaseSpeed * 2);
        return Mathf.Clamp(raw_speed, 0, 20);
    }


    public IEnumerator HarassUnit(Actor unit)
    {
        while (unit != null && Me != null && (Me.Actions.Combat.IsWithinMeleeRange(unit.transform) || !Me.Actions.Combat.IsWithinAttackRange(unit.transform))) {
            yield return new WaitForSeconds(Turn.ActionThreshold * 2);
            Vector3 point = unit.GetHarassPoint(Me);
            if (point != Vector3.zero) SetDestination(point);
        }
    }

    public void Home()
    {
        SetDestination(Destinations[CommonDestination.Home]);
    }

    public bool InProgress()
    {
        return Me.HasTask && !AtCurrentDestination();
    }

    public void Jump()
    {
        StartCoroutine(Jumping());
    }

    public float JumpVelocity()
    {
        return Me.Stats.Skills.Contains(Proficiencies.Skill.Acrobatics)
                         ? 3 + Me.Stats.GetAdjustedAttributeModifier(Proficiencies.Attribute.Strength) + Me.Stats.ProficiencyBonus / 2
                         : 3 + Me.Stats.GetAdjustedAttributeModifier(Proficiencies.Attribute.Strength);
    }

    public void ResetPath()
    {
        if (!IsJumping) {
            Agent.ResetPath();
            CurrentDestination = Vector3.zero;
        }
    }

    public void ResetSpeed()
    {
        SpeedAdjustment = 0;
        Agent.speed = GetAdjustedSpeed();
    }

    public void SetDestination(Transform target_object)
    {
        ResetPath();
        Collider target_collider = target_object.GetComponent<Collider>();
        Vector3 destination = (target_collider != null) ? target_collider.ClosestPointOnBounds(transform.position) : target_object.position;
        Vector3 new_facing = Vector3.RotateTowards(transform.forward, transform.position - destination, 30f * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(new_facing);
        Agent.SetDestination(destination);
        CurrentDestination = destination;
    }

    public void SetDestination(Vector3 destination)
    {
        ResetPath();
        Vector3 new_facing = Vector3.RotateTowards(transform.forward, transform.position - destination, 30f * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(new_facing);
        NavMeshPath path = new NavMeshPath();
        Agent.SetDestination(destination);
        CurrentDestination = destination;
    }

    public IEnumerator TrackUnit(Actor unit)
    {
        while (unit != null && Me != null && !Me.Actions.Combat.IsWithinMeleeRange(unit.transform)) {
            Vector3 point = unit.GetInteractionPoint(Me);
            if (point != Vector3.zero) SetDestination(point);
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }

    public void Warehouse()
    {
        SetDestination(Destinations[CommonDestination.Warehouse]);
    }

    public void Work()
    {
        if (Destinations.ContainsKey(CommonDestination.Craft)) {
            SetDestination(Destinations[CommonDestination.Craft]);
        } else if (Destinations.ContainsKey(CommonDestination.Harvest)) {
            SetDestination(Destinations[CommonDestination.Harvest]);
        }
    }


    // private

    private IEnumerator Dashing()
    {
        int tick = 0;
        float previous_speed_adjustment = SpeedAdjustment;
        AdjustSpeed(0.5f);
        IsDashing = true;

        while (tick < 10) {
            tick++;
            yield return new WaitForSeconds(1);
        }

        IsDashing = false;
        ResetSpeed();
        AdjustSpeed(previous_speed_adjustment);
    }

    private IEnumerator IsEncumbered()
    {
        while (true) {
            if (Encumbered) {
                Agent.speed = GetAdjustedSpeed() / 2f;
                Me.HasFullLoad = true;
            } else {
                Agent.speed = GetAdjustedSpeed();
                Me.HasFullLoad = false;
            }

            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }

    private IEnumerator Jumping()
    {
        while (true) {
            Agent.enabled = false;
            Me.GetComponent<Rigidbody>().AddForce(Vector3.up * JumpVelocity() * 150f, ForceMode.Impulse);
            if (IsJumping) break;
            IsJumping = true;
            yield return new WaitForSeconds(1);
        }
        IsJumping = false;
        Agent.enabled = true;
    }

    private bool NotMoving()
    {
        return Agent.velocity.x < 0.01f && Agent.velocity.y < 0.01f && Agent.velocity.z < 0.01f;
    }

    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>(); // need Me

        Agent = GetComponentInParent<NavMeshAgent>();
        Agent.ResetPath();
        CurrentDestination = Vector3.zero;
        Destinations = new Dictionary<CommonDestination, Vector3>();
        Encumbered = false;
        IsJumping = false;
        SpeedAdjustment = 0;

        AddDestination(CommonDestination.Home, transform.position);
    }
}