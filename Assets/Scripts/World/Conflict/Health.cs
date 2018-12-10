using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public int starting_health;
    public int current_health;
    public float recovery_rate;


    // Unity


    private void Awake()
    {

    }


    private void Update()
    {

    }


    private void OnValidate()
    {
        if (starting_health < 1) starting_health = 1;
        if (current_health < 0) current_health = 0;
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


    public void SetRecoveryRate(float rate)
    {
        recovery_rate = rate;
    }


    public void SetStartingHealth(int amount)
    {
        starting_health = amount;
        current_health = amount;
    }
}
