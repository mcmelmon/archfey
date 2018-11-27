using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public int starting_health;
    public int current_health;
    public int recovery_rate;


    // Unity

    private void Awake()
    {
        current_health = starting_health;
    }

    private void Update()
    {

    }

    // public 


    public void LoseHealth(int amount)
    {
        current_health -= amount;
    }




    public void RecoverHealth(int amount)
    {
        current_health += amount;
    }
}
