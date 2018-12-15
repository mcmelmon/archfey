using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public float starting_health;
    public float current_health;
    public float recovery_rate;
    public bool taken_damage;
    readonly Dictionary<GameObject, float> damagers = new Dictionary<GameObject, float>();
    Actor actor;

    // Unity


    private void Awake()
    {
        actor = GetComponent<Actor>();
    }


    private void OnValidate()
    {
        if (starting_health < 1f) starting_health = 1f;
        if (current_health < 0f) current_health = 1f;
    }


    private void Update()
    {
        // TODO: prune damagers every now and then to be sure destroyed objects are gone
    }


    // public 


    public void AddDamager(GameObject _attacker, float _damage)
    {
        if (!damagers.ContainsKey(_attacker)) {
            damagers[_attacker] = _damage;
        } else {
            damagers[_attacker] += _damage;
        }
    }


    public void ApplyDamageOverTime()
    {
        // TODO
    }


    public Dictionary<GameObject, float> GetDamagers()
    {
        return damagers;
    }


    public void LoseHealth(float amount)
    {
        float modified_amount = amount;     // TODO: handle resistances/armore/etc

        current_health -= modified_amount;
        taken_damage = true;                // even if it turns out to be no damage after modifications, there's a bruise
    }


    public void RecoverHealth(float amount)
    {
        if (Mathf.Approximately(current_health, starting_health) || Mathf.Approximately(recovery_rate, 0)) return;

        current_health += amount;
        if (current_health > starting_health) current_health = starting_health;
    }


    public void PersistOrPerish()
    {
        if (current_health <= 0) {
            // Objects being destroyed suddenly probably lack health configuration stats...
            Destroy(gameObject);
        }
    }


    public void SetRecoveryRate(float rate)
    {
        recovery_rate = rate;
    }


    public void SetStartingHealth(float amount)
    {
        starting_health = amount;
        current_health = amount;
    }
}
