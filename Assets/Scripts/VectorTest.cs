using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VectorTest : MonoBehaviour {

    public NavMeshAgent agent;
    public Transform first_point;
    public Transform second_point;
    public float first_vector_angle;
    public float second_vector_angle;

    public Vector3 to_first;
    public Vector3 to_second;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        to_first = first_point.transform.position - transform.position;
        to_second = second_point.transform.position - transform.position;
    }
    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.position += transform.TransformDirection(Vector3.forward * 10f * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            transform.position -= transform.TransformDirection(Vector3.forward * 10f * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            CircleLocation(to_first);
        } else {
            CircleLocation(to_second);
        }

        to_first = first_point.transform.position - transform.position;
        to_second = second_point.transform.position - transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 100));

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, to_first.normalized * 100);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, to_second.normalized * 100);
    }

    void CircleLocation(Vector3 _center)
    {
        Vector3 forward_motion = transform.TransformDirection(Vector3.forward);
        forward_motion.y = 0;

        Vector3 central_motion = (_center - forward_motion);
        central_motion.y = 0;

        if (Vector3.Angle(forward_motion, central_motion) > 90f) {
            // this turns rapidly toward the center, but as soon as the angle drops below 90, it no longer gets called!
            Vector3 new_facing = Vector3.RotateTowards(transform.forward, central_motion, 1f * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(new_facing);
        }

        Vector3 new_position = (forward_motion) * 10f * Time.deltaTime;

        transform.position += new_position;
    }

    Vector3 TowardLocation(Vector3 _from, Vector3 _to)
    {
        return _to - _from;
    }

    Vector3 PointBetween(Vector3 _from, Vector3 _to, float step_percentage, bool grounded)
    {
        Vector3 heading = TowardLocation(_from, _to);
        if (grounded) heading.y = 0;
        float distance = heading.magnitude * step_percentage;
        return distance * Vector3.Normalize(_to - _from) + _from;
    }
}
