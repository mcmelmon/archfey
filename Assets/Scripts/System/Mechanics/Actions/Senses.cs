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
    //public List<Item> Items { get; set; }
    public List<Structure> Structures { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void SetRange(float _range)
    {
        PerceptionRange = _range;
    }


    public void Sense()
    {
        Actors = FindObjectsOfType<Actor>()
            .Where(actor => Vector3.Distance(transform.position, actor.transform.position) < PerceptionRange)
            .Select(collider => collider.gameObject.GetComponent<Actor>()).OfType<Actor>().Distinct().ToList();
        Structures = FindObjectsOfType<Structure>()
            .Where(structure => Vector3.Distance(transform.position, structure.transform.position) < PerceptionRange)
            .Select(collider => collider.gameObject.GetComponent<Structure>()).OfType<Structure>().Distinct().ToList();

        RemoveHidden();

        Me.Actions.Attack.SetEnemyRanges();
    }


    // private


    private void RemoveHidden()
    {
        List<Actor> the_sneaking = Actors.Where(actor => actor.Actions.Stealth.Hiding).ToList();
        foreach (var sneaker in the_sneaking) {
            bool spotted = Me.Actions.PerceptionCheck(false, sneaker.Actions.Stealth.ChallengeRatting);  // TODO: include environmental detail for obscurity
            if (!spotted) Actors.Remove(sneaker);
        }
    }


    private void SetComponents()
    {
        Actors = new List<Actor>();
        Me = GetComponent<Actor>();
        Structures = new List<Structure>();
    }
}