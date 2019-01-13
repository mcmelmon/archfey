using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    // properties

    public Actor Me { get; set; }
    public int CurrentHitPoints { get; set; }
    public int HitDice { get; set; }
    public int HitDiceType { get; set; }
    public int MaximumHitPoints { get; set; }


    // Unity


    private void Awake()
    {
        Me = GetComponent<Actor>();
    }


    private void OnValidate()
    {
        if (MaximumHitPoints < 1) MaximumHitPoints = 1;
        if (CurrentHitPoints < 0) CurrentHitPoints = 0;
    }


    // public 


    public void LoseHealth(float amount, Actor _attacker = null)
    {
        CurrentHitPoints -= Mathf.RoundToInt(amount);
        if (_attacker != null) {
            Me.Actions.Decider.Threat.AddThreat(_attacker, amount);
            Me.Actions.Decider.Threat.SpreadThreat(_attacker, amount);
        }
        Me.Actions.Resources.UpdateStatBars();
    }


    public void RecoverHealth(int amount)
    {
        if (amount == 0 || CurrentHitPoints == MaximumHitPoints) return;

        CurrentHitPoints += amount;
        if (CurrentHitPoints > MaximumHitPoints) CurrentHitPoints = MaximumHitPoints;

        Me.Actions.Resources.UpdateStatBars();
    }


    public bool Persist()
    {
        if (CurrentHitPoints <= 0) {
            Conflict.Instance.AddCasualty(Me.Faction);
            Destroy(gameObject);
            return false;
        }

        return true;
    }


    public void SetCurrentAndMaxHitPoints()
    {
        CurrentHitPoints = MaximumHitPoints = (Me.Stats.ConstitutionProficiency * HitDice) + (HitDice * HitDiceType / 2) + 1;
    }

    // private


    public float CurrentHealthPercentage()
    {
        return (float)CurrentHitPoints / (float)MaximumHitPoints;
    }
}
