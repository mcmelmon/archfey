using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour {

    NavMeshAgent agent;
    GameObject wisp;


    // Unity


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

	
	void Update () {
        if (wisp != null) {
            if (Vector3.Distance(transform.position, wisp.transform.position) > 5f)
            {
                agent.SetDestination(wisp.transform.position);
            }
            else
            {
                agent.ResetPath();
            }
        }
	}


    // public


    public void MoveToward(Vector3 objective)
    {
        if (wisp == null)
        {
            wisp = Wisp.CallWisp(transform);
            wisp.GetComponent<Wisp>().FindPath(objective);
        }
    }
}


class Wisp : MonoBehaviour {
    List<Vector3> milestones = new List<Vector3>();
    List<Breadcrumb> path = new List<Breadcrumb>();
    Breadcrumb current_objective;
    readonly float approach_angle = 30f; // TODO: allow specification of the approach angle.
    bool has_objective = false;

    public struct Breadcrumb
    {
        public Vector3 position;
        public float remaining_distance;
    }


    // Unity


    private void Awake()
    {

    }


    private void Update()
    {
        if (has_objective) Move();
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 100));

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, (current_objective.position - transform.position));
    }


    // public


    public static GameObject CallWisp(Transform actor)
    {
        GameObject _wisp = new GameObject();  // we need the transform, but inheriting from Monobehavior prevents "new Wisp()"
        _wisp.AddComponent<Wisp>();
        _wisp.name = "Wisp";
        _wisp.transform.position = actor.transform.position;
        _wisp.transform.parent = actor.transform.GetComponentInParent<Conflict>().transform;

        return _wisp;
    }


    public void FindPath(Vector3 objective)
    {
        SetMilestones(objective);
        DropBreadcrumbs(objective);
        has_objective = SetCurrentObjective();
    }


    public void Move()
    {
        Vector3 forward_motion = transform.TransformDirection(Vector3.forward);
        forward_motion.y = 0;

        Vector3 central_motion = (current_objective.position - transform.position) - forward_motion;
        central_motion.y = 0;

        if (Vector3.Angle(forward_motion, central_motion) > approach_angle)
        {
            Vector3 new_facing = Vector3.RotateTowards(transform.forward, central_motion, 1f * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(new_facing);
        }

        Vector3 new_position = (forward_motion) * 3f * Time.deltaTime;
        transform.position += new_position;
        current_objective.remaining_distance = Vector3.Distance(transform.position, current_objective.position);

        if (current_objective.remaining_distance < 10f && path.Count > 0)
        {
            path.RemoveAt(0);
            has_objective = SetCurrentObjective();
        }
    }


    // private


    void DropBreadcrumbs(Vector3 objective)
    {
        foreach (var milestone in milestones)
        {
            Circle opportunity = Circle.CreateCircle(milestone, 15f);
            Vector3 spot = opportunity.RandomContainedPoint();

            Breadcrumb breadcrumb = new Breadcrumb();
            breadcrumb.position = spot;
            breadcrumb.remaining_distance = Vector3.Distance(transform.position, spot);
            path.Add(breadcrumb);

            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.name = "Crumb";
            //cube.transform.position = breadcrumb.position;
        }

        Circle main_target = Circle.CreateCircle(objective, 15f);
        Vector3 final_position = main_target.RandomContainedPoint();

        Breadcrumb last_crumb = new Breadcrumb();
        last_crumb.position = final_position;
        last_crumb.remaining_distance = Vector3.Distance(transform.position, final_position);
        path.Add(last_crumb);

        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.name = "Crumb";
        //cube.transform.position = last_crumb.position;
    }


    void SetMilestones(Vector3 objective)
    {
        float steps = 4;

        for (int s = 1; s < steps - 1; s++)
        {
            Vector3 milestone = Vector3.Lerp(transform.position, objective, s / (steps - 1));
            milestones.Add(milestone);

            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.name = "Milestone";
            //cube.transform.position = milestone;
        }
    }


    bool SetCurrentObjective()
    {
        if (path.Count <= 0) return false;
        current_objective = path[0];
        return true;
    }
}