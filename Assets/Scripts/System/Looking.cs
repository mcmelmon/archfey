using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Looking : MonoBehaviour {

    Vector3 looking;
    Vector3 smooth_v;
    public float sensitivity = 3.0f;
    public float smoothing = 1.0f;

    GameObject raven;

	void Start () {
        raven = this.transform.parent.gameObject;
	}
	
	void Update () {

        var md = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smooth_v.x = Mathf.Lerp(smooth_v.x, md.x, 1f / smoothing);
        smooth_v.y = Mathf.Lerp(smooth_v.y, md.y, 1f / smoothing);
        looking += smooth_v;

        raven.transform.localRotation = Quaternion.AngleAxis(looking.x, raven.transform.up);
    }
}