using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VectorTest : MonoBehaviour {

    public List<Vector3> milestones = new List<Vector3>();
    public List<Breadcrumb> path = new List<Breadcrumb>();

    public float angle;
    public Transform target;

    NavMeshAgent agent;
    GameObject wisp;
    Breadcrumb current_objective;

    public struct Breadcrumb
    {
        public Vector3 position;
        public float remaining_distance;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        wisp = new GameObject();
        wisp.name = "Wisp";
        wisp.transform.position = transform.position;
        wisp.transform.rotation = transform.rotation;
    }


    void Start () {
        Grid grid = Grid.New(new Vector3(10, 0, 10), 5, 5, 5f, true);
        //SetMilestones();
        //LayPath();
    }


    void Update ()
    {
        //if (path.Count > 0)
        //{
        //    SetObjective();
        //    MoveTowardObjective();
        //}
    }


    private void OnDrawGizmos()
    {
        if (wisp != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(wisp.transform.position, wisp.transform.TransformDirection(Vector3.forward * 100));
            Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 100));

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(wisp.transform.position, (current_objective.position - wisp.transform.position));
            Gizmos.DrawRay(transform.position, (current_objective.position - transform.position));
        }
    }


    void CircleLocation(Vector3 _center)
    {
        Vector3 forward_motion = transform.TransformDirection(Vector3.forward);
        forward_motion.y = 0;

        Vector3 central_motion = (_center - forward_motion);
        central_motion.y = 0;

        if (Vector3.Angle(forward_motion, central_motion) > angle) {
            Vector3 new_facing = Vector3.RotateTowards(transform.forward, central_motion, 1f * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(new_facing);
        }

        Vector3 new_position = (forward_motion) * 10f * Time.deltaTime;

        transform.position += new_position;
    }


    void LayPath()
    {
        foreach (var milestone in milestones)
        {
            Circle opportunity = Circle.New(milestone, 15f);
            Vector3 position = opportunity.RandomContainedPoint();

            Breadcrumb breadcrumb = new Breadcrumb();
            breadcrumb.position = position;
            breadcrumb.remaining_distance = Vector3.Distance(transform.position, position);
            path.Add(breadcrumb);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Crumb";
            cube.transform.position = breadcrumb.position;
        }

        Circle main_target = Circle.New(target.transform.position, 15f);
        Vector3 final_position = main_target.RandomContainedPoint();

        Breadcrumb last_crumb = new Breadcrumb();
        last_crumb.position = final_position;
        last_crumb.remaining_distance = Vector3.Distance(transform.position, final_position);
        path.Add(last_crumb);
    }


    void MoveTowardObjective()
    {
        Vector3 forward_motion = wisp.transform.TransformDirection(Vector3.forward);
        forward_motion.y = 0;

        Vector3 central_motion = (current_objective.position - wisp.transform.position) - forward_motion;
        central_motion.y = 0;

        if (Vector3.Angle(forward_motion, central_motion) > angle)
        {
            Vector3 new_facing = Vector3.RotateTowards(wisp.transform.forward, central_motion, 1f * Time.deltaTime, 0f);
            wisp.transform.rotation = Quaternion.LookRotation(new_facing);
        }

        Vector3 new_position = (forward_motion) * 5f * Time.deltaTime;
        wisp.transform.position += new_position;
        current_objective.remaining_distance = Vector3.Distance(wisp.transform.position, current_objective.position);

        if (current_objective.remaining_distance < 10f) {
            path.RemoveAt(0);
        }

        if (Vector3.Distance(transform.position, wisp.transform.position) > 5f) {
            agent.SetDestination(wisp.transform.position);
        } else {
            agent.ResetPath();
        }
    }


    void SetMilestones()
    {
        float position = 4;

        for (int s = 1; s < position - 1; s++)
        {
            Vector3 breadcrumb = Vector3.Lerp(transform.position, target.transform.position, s / (position - 1));
            milestones.Add(breadcrumb);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Milestone";
            cube.transform.position = breadcrumb;
        }
    }


    bool SetObjective()
    {
        if (path.Count <= 0) return false;
        current_objective = path[0];
        return true;
    }
}