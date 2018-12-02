using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VectorTest : MonoBehaviour {

    NavMeshAgent agent;
    public Transform target;
    GameObject wisp;
    public List<Vector3> milestones = new List<Vector3>();
    public List<Vector3> path = new List<Vector3>();

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        wisp = new GameObject();
        wisp.name = "Wisp";
        wisp.transform.position = transform.position;
        wisp.transform.rotation = transform.rotation;
    }


    void Start () {
        SetMilestones();
        PickPath();
    }


    void Update () {
        FollowPath();
    }


    private void OnDrawGizmos()
    {
        if (wisp != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(wisp.transform.position, wisp.transform.TransformDirection(Vector3.forward * 100));
            Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 100));

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(wisp.transform.position, (path[0] - wisp.transform.position).normalized * 100);
            Gizmos.DrawRay(transform.position, (path[0] - wisp.transform.position).normalized * 100);
        }
    }


    void CircleLocation(Vector3 _center)
    {
        Vector3 forward_motion = transform.TransformDirection(Vector3.forward);
        forward_motion.y = 0;

        Vector3 central_motion = (_center - forward_motion);
        central_motion.y = 0;

        if (Vector3.Angle(forward_motion, central_motion) > 90f) {
            Vector3 new_facing = Vector3.RotateTowards(transform.forward, central_motion, 1f * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(new_facing);
        }

        Vector3 new_position = (forward_motion) * 10f * Time.deltaTime;

        transform.position += new_position;
    }


    void FollowPath()
    {
        Vector3 forward_motion = wisp.transform.TransformDirection(Vector3.forward);
        forward_motion.y = 0;

        Vector3 central_motion = (path[0] - wisp.transform.position) - forward_motion;
        central_motion.y = 0;
        
        if (Vector3.Angle(forward_motion, central_motion) > 90f)
        {
            Vector3 new_facing = Vector3.RotateTowards(wisp.transform.forward, central_motion, 1f * Time.deltaTime, 0f);
            wisp.transform.rotation = Quaternion.LookRotation(new_facing);
        }

        Vector3 new_position = (forward_motion) * 5f * Time.deltaTime;
        wisp.transform.position += new_position;

        agent.SetDestination(wisp.transform.position);
    }


    void PickPath()
    {
        foreach (var milestone in milestones)
        {
            Circle opportunity = new Circle();
            opportunity.Inscribe(milestone, 15f);
            Vector3 breadcrumb = opportunity.RandomContainedPoint();

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = breadcrumb;
            cube.name = "breadcrumb";
            path.Add(breadcrumb);
        }
    }


    void SetMilestones()
    {
        float position = 4;

        for (int s = 1; s < position - 1; s++)
        {
            Vector3 breadcrumb = Vector3.Lerp(transform.position, target.transform.position, s / (position - 1));
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Milestone";
            cube.transform.position = breadcrumb;
            milestones.Add(breadcrumb);
        }

    }
}