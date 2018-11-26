using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Leaf : MonoBehaviour {

    private Transform target;

    public float speed = 70f;
    public GameObject impact;

    void Update()
    {
        if (target != null)
        {
            Attack();
        }
        else {
            Destroy(gameObject);
            return;
        }
    }


    public void Seek(Transform _target)
    {
        target = _target;
    }

    private void Hit()
    {
        GameObject _impact = Instantiate(impact, transform.position, transform.rotation);
        Destroy(gameObject);
        Destroy(_impact, 2f);
        Destroy(target.gameObject);
    }

    private void Attack() 
    {
        Vector3 direction = target.position - transform.position;
        float distanceTraveled = speed * Time.deltaTime;
        transform.Translate(direction.normalized * distanceTraveled, Space.World);

        if (direction.magnitude <= distanceTraveled)
        {
            Hit();
            return;
        }
    }
}