using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {

    public Mhoddim mhoddim;
    public Ghaddim ghaddim;
    public Fey fey;
    public Attacker attacker;
    public Defender defender;
    public Movement movement;
    public Color hover_color;

    Color my_color;
    Renderer my_renderer;

    // Unity


    private void Awake()
    {
        if (GetComponent<SphereCollider>() == null) AddSenses();

    }


    private void Start()
    {
        my_renderer = GetComponent<Renderer>();
        my_color = my_renderer.material.color;
    }


    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.forward * 50));
    //}


    private void OnMouseDown()
    {
        Debug.Log("Quit touching me!");
    }


    private void OnMouseEnter()
    {
        my_renderer.material.color = Color.blue;
    }


    private void OnMouseExit()
    {
        my_renderer.material.color = my_color;
    }


    // public


    public void Attack()
    {

    }


    public void Move(Route _route)
    {
        movement.SetRoute(_route);
    }


    public void SetComponents()
    {
        mhoddim = GetComponent<Mhoddim>();
        ghaddim = GetComponent<Ghaddim>();
        fey = GetComponent<Fey>();
        attacker = GetComponent<Attacker>();
        defender = GetComponent<Defender>();
        movement = GetComponent<Movement>();
    }


    public void SetStats()
    {
        if (mhoddim != null) {
            mhoddim.SetHealthStats(gameObject);
        }
        else if (ghaddim != null) {
            ghaddim.SetHealthStats(gameObject);
        } else if (fey != null) {

        }
    }


    // private


    private void AddSenses()
    {
        transform.gameObject.AddComponent<Senses>();
        GetComponent<SphereCollider>().isTrigger = true;
    }
}