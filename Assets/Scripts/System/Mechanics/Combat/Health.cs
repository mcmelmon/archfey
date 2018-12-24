using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    // inspector settings

    public Slider health_bar;


    // properties

    public Actor Actor { get; set; }
    public int CurrentHealth { get; set; }
    public int RecoveryAmount { get; set; }
    public int MaximumHealth { get; set; }


    // Unity


    private void Awake()
    {
        Actor = GetComponent<Actor>();
        StartCoroutine(HealthBarFaceCamera());
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
        UpdateHealthBar();
    }


    public void LoseHealth(float amount)
    {
        CurrentHealth -= Mathf.RoundToInt(amount);
        UpdateHealthBar();
    }


    public void RecoverHealth(int amount)
    {
        if (amount == 0 || CurrentHealth == MaximumHealth) return;

        CurrentHealth += amount;
        if (CurrentHealth > MaximumHealth) CurrentHealth = MaximumHealth;

        UpdateHealthBar();
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
        UpdateHealthBar();
    }


    // private


    public float CurrentHealthPercentage()
    {
        return (float)CurrentHealth / (float)MaximumHealth;
    }


    private IEnumerator HealthBarFaceCamera()
    {
        while (true) {
            yield return null;
            Vector3 health_position = transform.position;
            Vector3 player_position = Player.Instance.viewport.transform.position;

            Quaternion rotation = Quaternion.LookRotation(player_position - health_position, Vector3.up);
            health_bar.transform.rotation = rotation;
        }
    }


    public void UpdateHealthBar()
    {
        health_bar.value = CurrentHealthPercentage();
        if (health_bar.value >= 1) {
            health_bar.gameObject.SetActive(false);
        } else {
            health_bar.gameObject.SetActive(true);
        }
    }
}
