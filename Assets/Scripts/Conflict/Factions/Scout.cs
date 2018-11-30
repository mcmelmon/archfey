using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : MonoBehaviour {

    Geography geography;
    Attack offense;
    Defend defense;

    // Unity

    private void Awake()
    {
        geography = GetComponentInParent<World>().GetComponentInChildren<Geography>();
        offense = GetComponentInParent<Attack>();
        defense = GetComponentInParent<Defend>();
        GetComponent<Senses>().radius = 40f;
    }


    private void Start () {
        ChoosePath();
    }


    private void Update () {
		
	}


    // private

    private void ChoosePath()
    {
        if (defense == null && offense != null) {  // attacker; not a Fey
            float to_edge = Mathf.Infinity;
            string nearest_edge = "";

            Dictionary<string, float> distances = geography.DistanceToEdges(transform.position);
            foreach (KeyValuePair<string, float> keyValue in distances)
            {
                if (keyValue.Value <= to_edge) {
                    to_edge = keyValue.Value;
                    nearest_edge = keyValue.Key;
                } 
            }

            MoveAwayFromPrimary(geography.GetBorder(nearest_edge));
        }
    }

    private void MoveAwayFromPrimary(Vector3[] nearest_edge)
    {
        Vector3 primary_center = GetComponentInParent<Offense>().GetAttackCircles()["primary"].center;
        Vector3 first_vertex = nearest_edge[0];
        Vector3 second_vertex = nearest_edge[1];
        Vector3 corner, destination;
        
        if ( Vector3.Distance(primary_center, first_vertex) > Vector3.Distance(primary_center, second_vertex) ) {
            corner = nearest_edge[0];
        } else {
            corner = nearest_edge[1];
        }

        destination = geography.PointBetween(corner, geography.GetCenter(), .1f, true);

        GetComponentInParent<Actor>().SetDestination(destination);
    }
}
