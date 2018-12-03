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
        //if (wisp != null) {
        //    if (Vector3.Distance(transform.position, wisp.transform.position) > 5f)
        //    {
        //        agent.SetDestination(wisp.transform.position);
        //    }
        //    else
        //    {
        //        agent.ResetPath();
        //    }
        //}
	}


    // public


    public void MoveToward(Vector3 objective)
    {
        agent.SetDestination(objective);

        //if (wisp == null)
        //{
        //    wisp = Wisp.CallWisp(transform);
        //    wisp.GetComponent<Wisp>().FindPath(objective);
        //}
    }
}