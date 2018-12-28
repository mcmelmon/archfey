using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultRange : MonoBehaviour
{
    // properties

    public Attack Attack { get; set; }


    // Unity

    private void Awake()
    {
        Attack = GetComponent<Attack>();
    }


    //void Start()
    //{
    //    if (range == Range.Melee) StartCoroutine(Seek());
    //}


    // public


    public void Strike(Actor _target)
    {
        foreach (var weapon in Attack.AvailableWeapons())
        {

            // TODO: potentially disadvantage ranged attacks against melee targets
        }
    }


    // private


    //private IEnumerator Seek()
    //{
    //    while (true)
    //    {
    //        if (range == Range.Ranged)
    //        {
    //            if (Target == null)
    //            {
    //                Destroy(gameObject);  // destroy ranged "ammunition"
    //                yield return null;
    //            }

    //            float separation = float.MaxValue;
    //            Vector3 direction = Target.transform.position - transform.position;
    //            float distance = projectile_speed * Time.deltaTime;
    //            transform.position += distance * direction;
    //            separation = Vector3.Distance(Target.transform.position, transform.position);

    //            if (separation <= .5f) Hit();
    //        }

    //        yield return null;
    //    }
    //}
}
