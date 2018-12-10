using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour {

    public CinemachineFreeLook viewport;
    public float speed = 12f;
    public float agility = 30f;

    // Unity

	void Start () {

	}
	
	void Update () {
        AdjustCameraDistance();
        Move();

    }


    // public


    // private

    private void AdjustCameraDistance()
    {
        float proximity = Input.GetAxis("Mouse ScrollWheel") * 20f;
        if (!Mathf.Approximately(proximity, 0f))
        {
            CinemachineFreeLook.Orbit[] orbits = viewport.m_Orbits;
            for (int i = 0; i < orbits.Length; i++)
            {
                orbits[i].m_Radius -= Mathf.Lerp(0, proximity, Time.deltaTime * 5f);
            }
        }
    }


    private void Move()
    {
        Vector3 movement = Vector3.zero;

        float forward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float rotation = Input.GetAxis("Horizontal") * agility * Time.deltaTime;

        if (!Mathf.Approximately(forward, 0) || !Mathf.Approximately(rotation, 0)) {
            transform.rotation *= Quaternion.AngleAxis(rotation, Vector3.up);
            transform.position += transform.TransformDirection(Vector3.forward) * forward;
        }
    }

}
