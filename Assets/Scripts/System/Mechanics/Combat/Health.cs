using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    // properties

    public Actor Actor { get; set; }
    public int CurrentHealth { get; set; }
    public int RecoveryAmount { get; set; }
    public int StartingHealth { get; set; }


    // Unity


    private void Awake()
    {
        Actor = GetComponent<Actor>();
    }


    private void OnValidate()
    {
        if (StartingHealth < 1) StartingHealth = 1;
        if (CurrentHealth < 0f) CurrentHealth = 0;
    }


    private void Update()
    {
        // TODO: prune damagers every now and then to be sure destroyed objects are gone
    }


    // public 


    public void ApplyDamageOverTime()
    {
        // TODO
    }


    public void LoseHealth(float amount)
    {
        CurrentHealth -= Mathf.RoundToInt(amount);
    }


    public void RecoverHealth(int amount)
    {
        if (amount == 0 || CurrentHealth == StartingHealth) return;

        CurrentHealth += amount;
        if (CurrentHealth > StartingHealth) CurrentHealth = StartingHealth;
    }


    public bool Persist()
    {
        if (CurrentHealth <= 0) {
            Conflict.Instance.AddCasualty(Actor.Faction);
            Destroy(gameObject);
            return false;
        }

        return true;
    }


    public void SetRecoveryAmount(int amount)
    {
        RecoveryAmount = amount;
    }


    public void SetStartingHealth(int amount)
    {
        StartingHealth = amount;
        CurrentHealth = amount;
    }
}
