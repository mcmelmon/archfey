using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Senses : MonoBehaviour
{

    // properties

    public Actor Me { get; set; }
    public float Darkvision { get; set; }
    public float PerceptionRange { get; set; }
    public List<Actor> Actors { get; set; }
    public List<Structure> Structures { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void SetRange(float _range)
    {
        GetComponent<SphereCollider>().radius = PerceptionRange = _range;
    }


    public void Sense()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, PerceptionRange);

        Actors = colliders.Select(collider => collider.gameObject.GetComponent<Actor>()).OfType<Actor>().Distinct().ToList();
        Structures = colliders.Select(collider => collider.gameObject.GetComponent<Structure>()).OfType<Structure>().Distinct().ToList();
        Me.Actions.Attack.SetEnemyRanges();
    }


    // private

    private void SetComponents()
    {
        Actors = new List<Actor>();
        Me = GetComponent<Actor>();
        Structures = new List<Structure>();

        transform.gameObject.AddComponent<SphereCollider>();
        GetComponent<SphereCollider>().isTrigger = true;
    }
}