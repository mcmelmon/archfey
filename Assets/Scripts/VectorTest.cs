using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VectorTest : MonoBehaviour {

    public NavMeshAgent agent;
    public Transform first_point;
    public Transform second_point;
    public Vector3 to_first;
    public Vector3 to_second;
    public float distance = Mathf.Infinity;
    public Vector3 target_position;

    private GameObject wisp;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        to_first = first_point.transform.position - transform.position;
        to_second = second_point.transform.position - transform.position;

        wisp = new GameObject();
        wisp.name = "Wisp";
        wisp.transform.position = transform.position;
        wisp.transform.rotation = transform.rotation;
    }


    void Start () {
    }


    void Update () {
        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.position += transform.TransformDirection(Vector3.forward * 10f * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            transform.position -= transform.TransformDirection(Vector3.forward * 10f * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            CircleLocation(to_first);
        } else {
            AgentMotion();
        }

        to_first = first_point.transform.position - wisp.transform.position;
        to_second = second_point.transform.position - wisp.transform.position;
    }


    private void OnDrawGizmos()
    {
        if (wisp != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(wisp.transform.position, wisp.transform.TransformDirection(Vector3.forward * 100));
            Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 100));

            Gizmos.color = Color.green;
            Gizmos.DrawRay(wisp.transform.position, to_first.normalized * 100);
            Gizmos.DrawRay(transform.position, to_first.normalized * 100);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(wisp.transform.position, to_second.normalized * 100);
            Gizmos.DrawRay(transform.position, to_second.normalized * 100);
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


    void AgentMotion()
    {
        Vector3 forward_motion = wisp.transform.TransformDirection(Vector3.forward);
        forward_motion.y = 0;

        Vector3 central_motion = (to_second - forward_motion);
        central_motion.y = 0;

        if (Vector3.Angle(forward_motion, central_motion) > 90f)
        {
            Vector3 new_facing = Vector3.RotateTowards(wisp.transform.forward, central_motion, 1f * Time.deltaTime, 0f);
            wisp.transform.rotation = Quaternion.LookRotation(new_facing);
        }

        Vector3 new_position = (forward_motion) * Random.Range(0,3f) * Time.deltaTime;

        wisp.transform.position += new_position;
        if (Vector3.Distance(wisp.transform.position, transform.position) > 10f) {
            agent.SetDestination(wisp.transform.position);
        } else {
            agent.ResetPath();
        }
    }
}