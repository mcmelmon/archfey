using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    // properties

    public Actor Me { get; set; }
    public int CurrentHitPoints { get; set; }
    public int CurrentTemporaryHitPoints { get; set; }
    public int HitDice { get; set; }
    public int HitDiceType { get; set; }
    public int MaximumHitPoints { get; set; }
    public int TemporaryHitPoints { get; set; }


    // Unity


    private void Awake()
    {
        Me = GetComponent<Actor>();
        CurrentTemporaryHitPoints = TemporaryHitPoints = 0;
    }


    private void OnValidate()
    {
        if (MaximumHitPoints < 1) MaximumHitPoints = 1;
        if (CurrentHitPoints < 0) CurrentHitPoints = 0;
    }


    // public 


    public bool BadlyInjured()
    {
        return CurrentHealthPercentage() <= 0.6f;
    }


    public void ClearTemporaryHitPoints()
    {
        CurrentTemporaryHitPoints = TemporaryHitPoints = 0;
    }


    public float CurrentHealthPercentage()
    {
        return Mathf.Approximately(0, MaximumHitPoints) ? 0 : (float)CurrentHitPoints / (float)MaximumHitPoints;
    }


    public float CurrentTemporaryHealthPercentage()
    {
        return Mathf.Approximately(0, TemporaryHitPoints) ? 0 : (float)CurrentTemporaryHitPoints / (float)TemporaryHitPoints;
    }


    public void GainTemporaryHitPoints(int amount)
    {
        if (CurrentTemporaryHitPoints < amount) {
            CurrentTemporaryHitPoints = TemporaryHitPoints = amount;
        }
    }


    public void LoseHealth(float amount, Actor attacker = null)
    {
        if (CurrentTemporaryHitPoints > amount) {
            CurrentTemporaryHitPoints -= Mathf.RoundToInt(amount);
        } else {
            amount -= CurrentTemporaryHitPoints;
            CurrentTemporaryHitPoints = 0;
            if (amount > 0) {
                CurrentHitPoints -= Mathf.RoundToInt(amount);
                if (attacker != null) {
                    Me.Actions.Decider.Threat.AddThreat(attacker, amount);
                }
            }
        }

        Me.Stats.UpdateStatBars();
    }


    public void RecoverHealth(int amount)
    {
        if (amount == 0 || CurrentHitPoints == MaximumHitPoints) return;

        CurrentHitPoints += amount;
        if (CurrentHitPoints > MaximumHitPoints) CurrentHitPoints = MaximumHitPoints;

        Me.Stats.UpdateStatBars();
    }


    public bool Persist()
    {
        if (CurrentHitPoints <= 0) {
            if (Me != Player.Instance.Me) {
                Destroy(gameObject);
            } else {
                Player.Instance.Respawn();
            }
            return false;
        }

        return true;
    }


    public void SetCurrentAndMaxHitPoints()
    {
        CurrentHitPoints = MaximumHitPoints = Mathf.RoundToInt((Me.Stats.BaseAttributes[Proficiencies.Attribute.Constitution] * HitDice) + (HitDice * (HitDiceType / 2f) + 1) + HitDice/2f);
        CurrentTemporaryHitPoints = TemporaryHitPoints = 0;
    }
}
