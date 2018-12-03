using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

    Dictionary<string, GameObject> senses = new Dictionary<string, GameObject>();
    Movement mover;
    Attack attack_transform;
    Defend defense_transform;


    // Unity


    private void Awake()
    {
        if (GetComponent<SphereCollider>() == null) AddSenses();
        mover = GetComponent<Movement>();
    }


    private void Start()
    {
        attack_transform = GetComponent<Attack>();
        defense_transform = GetComponent<Defend>();
    }


    private void Update()
    {

    }


    // public


    public void Attack()
    {

    }


    public Attack GetAttackTransform()
    {
        if (attack_transform == null) attack_transform = GetComponent<Attack>();
        return attack_transform;
    }


    public Defend GetDefenseTransform()
    {
        if (defense_transform == null) defense_transform = GetComponent<Defend>();
        return defense_transform;
    }


    public bool Friend()
    {
        return true;
    }


    public void Move(Vector3 objective)
    {
        mover.MoveToward(objective);
    }


    // private


    private void AddSenses()
    {
        transform.gameObject.AddComponent<Senses>();
        GetComponent<SphereCollider>().isTrigger = true;
    }
}