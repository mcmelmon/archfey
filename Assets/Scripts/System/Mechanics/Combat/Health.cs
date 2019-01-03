using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    // properties

    public Actor Actor { get; set; }
    public int CurrentHitPoints { get; set; }
    public int HitDice { get; set; }
    public int HitDiceType { get; set; }
    public int MaximumHitPoints { get; set; }


    // Unity


    private void Awake()
    {
        Actor = GetComponent<Actor>();
    }


    private void OnValidate()
    {
        if (MaximumHitPoints < 1) MaximumHitPoints = 1;
        if (CurrentHitPoints < 0) CurrentHitPoints = 0;
    }


    // public 


    public void ApplyDamageOverTime()
    {
        // TODO
        Actor.Resources.UpdateStatBars();
    }


    public void LoseHealth(float amount, Actor _attacker = null)
    {
        CurrentHitPoints -= Mathf.RoundToInt(amount);
        if (_attacker != null) {
            Actor.Threat.AddThreat(_attacker, amount);
            Actor.Threat.SpreadThreat(_attacker, amount);
        }
        Actor.Resources.UpdateStatBars();
    }


    public void RecoverHealth(int amount)
    {
        if (amount == 0 || CurrentHitPoints == MaximumHitPoints) return;

        CurrentHitPoints += amount;
        if (CurrentHitPoints > MaximumHitPoints) CurrentHitPoints = MaximumHitPoints;

        Actor.Resources.UpdateStatBars();
    }


    public bool Persist()
    {
        if (CurrentHitPoints <= 0) {
            Conflict.Instance.AddCasualty(Actor.Faction);
            Destroy(gameObject);
            return false;
        }

        return true;
    }


    // private


    public float CurrentHealthPercentage()
    {
        return (float)CurrentHitPoints / (float)MaximumHitPoints;
    }
}
