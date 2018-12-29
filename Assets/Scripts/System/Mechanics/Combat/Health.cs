using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    // properties

    public Actor Actor { get; set; }
    public int CurrentHealth { get; set; }
    public int RecoveryAmount { get; set; }
    public int MaximumHealth { get; set; }


    // Unity


    private void Awake()
    {
        Actor = GetComponent<Actor>();
    }


    private void OnValidate()
    {
        if (MaximumHealth < 1) MaximumHealth = 1;
        if (CurrentHealth < 0f) CurrentHealth = 0;
    }


    // public 


    public void ApplyDamageOverTime()
    {
        // TODO
        Actor.Resources.UpdateStatBars();
    }


    public void LoseHealth(float amount, Actor _attacker = null)
    {
        CurrentHealth -= Mathf.RoundToInt(amount);
        if (_attacker != null) {
            Actor.Threat.AddThreat(_attacker, amount);
            Actor.Threat.SpreadThreat(_attacker, amount);
        }
        Actor.Resources.UpdateStatBars();
    }


    public void RecoverHealth(int amount)
    {
        if (amount == 0 || CurrentHealth == MaximumHealth) return;

        CurrentHealth += amount;
        if (CurrentHealth > MaximumHealth) CurrentHealth = MaximumHealth;

        Actor.Resources.UpdateStatBars();
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
        MaximumHealth = amount;
        CurrentHealth = amount;
    }


    // private


    public float CurrentHealthPercentage()
    {
        return (float)CurrentHealth / (float)MaximumHealth;
    }
}
