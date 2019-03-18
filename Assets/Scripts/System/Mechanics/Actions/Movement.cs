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
    public Dictionary<CommonDestination, Vector3> Destinations { get; set; }
    public bool IsJumping { get; set; }
    public float JumpVelocity { get; set; }
    public bool NonAgentMovement { get; set; }
    public float ReachedThreshold { get; set; }
    public float SpeedAdjustment { get; set; }


    // Unity


    private void Start()
    {
        SetComponents();
    }


    // public


    public void AddDestination(CommonDestination key, Vector3 destination)
    {
        if (!Destinations.ContainsKey(key)) {
            Destinations[key] = destination;
        }
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
    }


    public float GetAdjustedSpeed()
    {
        return Mathf.Clamp(BaseSpeed + (SpeedAdjustment * BaseSpeed * 2), 0, 20);
    }


    public void Home()
    {
        SetDestination(Destinations[CommonDestination.Home]);
    }


    public bool InProgress()
    {
        return (Agent != null) && Agent.hasPath && Agent.velocity != Vector3.zero && !NonAgentMovement;
    }


    public void Jump()
    {
        StartCoroutine(Jumping());
    }


    public void ResetPath()
    {
        if (!IsJumping) Agent.ResetPath();
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
        Agent.SetDestination(destination);  // may have height issues on terrain
    }


    public void SetDestination(Vector3 destination)
    {
        ResetPath();
        Vector3 new_facing = Vector3.RotateTowards(transform.forward, transform.position - destination, 30f * Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(new_facing);
        Agent.SetDestination(destination);  // may have height issues on terrain
    }


    public IEnumerator TrackUnit(Actor unit)
    {
        int count = 0;

        while (unit != null && count < Turn.ActionThreshold && !Me.Actions.Attack.IsWithinAttackRange(unit.transform)) {
            SetDestination(unit.GetInteractionPoint(Me));
            count++;
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


    private IEnumerator Jumping()
    {
        while (true) {
            Agent.enabled = false;
            Me.GetComponent<Rigidbody>().AddForce(Vector3.up * JumpVelocity * 150f, ForceMode.Impulse);
            if (IsJumping) break;
            IsJumping = true;
            yield return new WaitForSeconds(2f);
        }
        IsJumping = false;
        Agent.enabled = true;
    }


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>(); // need Me

        Agent = GetComponentInParent<NavMeshAgent>();
        Agent.ResetPath();
        Destinations = new Dictionary<CommonDestination, Vector3>();
        IsJumping = false;
        JumpVelocity = Me.Stats.Skills.Contains(Proficiencies.Skill.Acrobatics)
                         ? 3 + Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Strength) + Me.Stats.ProficiencyBonus / 2
                         : 3 + Me.Stats.GetAdjustedAttributeScore(Proficiencies.Attribute.Strength);
        SpeedAdjustment = 0;
    }
}