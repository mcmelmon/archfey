using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour {

    NavMeshAgent agent;
    GameObject wisp;

    // TODO: Movement should take route management duties from Scout

    // Unity


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

	
	void Update () {

	}


    // public


    public void MoveToward(Vector3 objective)
    {
        agent.SetDestination(objective);
    }
}