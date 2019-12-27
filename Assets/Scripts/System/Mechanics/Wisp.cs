using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Wisp : MonoBehaviour
{

    public Transform haunt;
    public float haunt_radius = 15f;
    public bool reverse_haunt = false;

    public Light candle_light;
    public Color candle_color = Color.white;
    public float candle_intensity = 1f;
    public float candle_range = 10f;
    List<Vector3> milestones = new List<Vector3>();
    List<Breadcrumb> path = new List<Breadcrumb>();
    bool looping_path = false;
    Breadcrumb current_objective;
    readonly float approach_angle = 30f; // TODO: allow specification of the approach angle.
    bool has_objective = false;

    public struct Breadcrumb
    {
        public Vector3 position;
        public float remaining_distance;
    }


    // static


    public static GameObject CallWisp(Transform actor)
    {
        GameObject _wisp = new GameObject();  // we need the transform, but inheriting from Monobehavior prevents "new Wisp()"
        _wisp.AddComponent<Wisp>();
        _wisp.name = "Wisp";
        _wisp.transform.position = actor.transform.position;
        _wisp.transform.parent = actor.transform.GetComponentInParent<Conflict>().transform;

        return _wisp;
    }


    // Unity

    private void Awake() {
        if (haunt != null) {
            has_objective = true;
            candle_light.color = candle_color;
            candle_light.intensity = candle_intensity;
            candle_light.range = candle_range;

            // Create a circle around our haunt

            Circle haunting_circle = Circle.New(haunt.position, haunt_radius);

            for (int i = haunting_circle.VertexCount - 1; i <=0; i--) {
                path.Add(CreateBreadcrumb(haunting_circle.Vertices[i]));
            }

            foreach (var point in haunting_circle.Vertices) {
                path.Add(CreateBreadcrumb(point));
            }
            looping_path = true;
            transform.position = path[0].position;
            current_objective = path[0];
        }
    }


    private void Update()
    {
        if (current_objective.remaining_distance < 1f && path.Count > 0) {
            if (looping_path) path.Add(current_objective);
            path.RemoveAt(0);
            has_objective = SetCurrentObjective();
        }

        if (has_objective) Move();
    }


    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 100));

    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawRay(transform.position, (current_objective.position - transform.position));
    // }


    // public


    public void FindPathToMilestone(Vector3 objective)
    {
        SetMilestones(objective);
        DropBreadcrumbsAroundMilestones(objective);
        has_objective = SetCurrentObjective();
    }


    public void Move()
    {
        // float terrain_height = Geography.Terrain != null ? Geography.Terrain.SampleHeight(current_objective.position) : current_objective.position.y;

        Vector3 forward_motion = transform.TransformDirection(Vector3.forward);
        forward_motion.y = 0;

        Vector3 central_motion = (current_objective.position - transform.position) - forward_motion;
        central_motion.y = 0;

        if (Vector3.Angle(forward_motion, central_motion) > approach_angle) {
            Vector3 new_facing = Vector3.RotateTowards(transform.forward, central_motion, 1f * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(new_facing);
        }

        Vector3 new_position = (forward_motion) * 3f * Time.deltaTime;
        transform.position += new_position;
        // transform.position = new Vector3(transform.position.x, terrain_height, transform.position.z);
        transform.position = FindSurface(transform.position);
        current_objective.remaining_distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(current_objective.position.x, 0, current_objective.position.z));
    }


    // private


    Breadcrumb CreateBreadcrumb(Vector3 objective)
    {
        Breadcrumb breadcrumb = new Breadcrumb();
        breadcrumb.position = objective;
        breadcrumb.remaining_distance = Vector3.Distance(transform.position, objective);
        return breadcrumb;
    }


    void DropBreadcrumbsAroundMilestones(Vector3 objective)
    {
        foreach (var milestone in milestones) {
            Circle opportunity = Circle.New(milestone, haunt_radius);
            Vector3 spot = opportunity.RandomContainedPoint();
            path.Add(CreateBreadcrumb(spot));

            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.name = "Crumb";
            //cube.transform.position = breadcrumb.position;
        }

        Circle main_target = Circle.New(objective, haunt_radius);
        Vector3 final_position = main_target.RandomContainedPoint();
        path.Add(CreateBreadcrumb(final_position));

        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.name = "Crumb";
        //cube.transform.position = last_crumb.position;
    }


    Vector3 FindSurface(Vector3 position)
    {
        Vector3 above = position + Vector3.up * 500f;
        RaycastHit hit;
        if (Physics.Raycast(above, Vector3.down, out hit, Mathf.Infinity)) {
            // Debug.DrawRay(above, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            return hit.point + Vector3.up * 2;
        }
        return position;
    }


    void SetMilestones(Vector3 objective)
    {
        float steps = 4;

        for (int s = 1; s < steps - 1; s++) {
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