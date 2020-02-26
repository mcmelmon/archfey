using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    // properties

    public Actor Me { get; set; }
    public int CurrentHitPoints { get; set; }
    public int CurrentTemporaryHitPoints { get; set; }
    public Dictionary<int, int> HitDice { get; set; }
    public int MaximumHitPoints { get; set; }
    public Action OnHealthChange;
    public int TemporaryHitPoints { get; set; }

    // Unity

    private void Awake()
    {
        SetComponents();
    }

    private void OnValidate()
    {
        if (MaximumHitPoints < 1) MaximumHitPoints = 1;
        if (CurrentHitPoints < 0) CurrentHitPoints = 0;
    }

    // public 

    public void AddHitDice(int dice_type, int number_of_dice)
    {
        if (HitDice.ContainsKey(dice_type)) {
            HitDice[dice_type] += number_of_dice;
        } else {
            HitDice[dice_type] = number_of_dice;
        }

        SetCurrentAndMaxHitPoints();
    }

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

    public int LargestHitDie()
    {
        List<int> dice_types = new List<int>(HitDice.Keys);
        dice_types.Sort();
        return dice_types[dice_types.Count - 1];
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
        OnHealthChange?.Invoke();
    }

    public void RecoverHealth(int amount)
    {
        if (amount == 0 || CurrentHitPoints == MaximumHitPoints) return;

        CurrentHitPoints += amount;
        if (CurrentHitPoints > MaximumHitPoints) CurrentHitPoints = MaximumHitPoints;

        Me.Stats.UpdateStatBars();
        OnHealthChange?.Invoke();
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
        int hit_points = 0;

        // use the "fake roll" (e.g. 1d8 = 5) for each hit dice
        foreach (KeyValuePair<int, int> hit_die in HitDice) {
            hit_points += Me.Stats.GetAdjustedAttributeModifier(Proficiencies.Attribute.Constitution) * hit_die.Value;
            hit_points += Mathf.RoundToInt(hit_die.Value * ((hit_die.Key / 2f) + 1));
        }

        // add in half of the largest hit die (giving multiclass the benefit of its largest, not first, die)
        hit_points += Mathf.RoundToInt(LargestHitDie() / 2f) - 1;

        CurrentHitPoints = MaximumHitPoints = hit_points;
        CurrentTemporaryHitPoints = TemporaryHitPoints = 0;
    }

    // private

    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        CurrentTemporaryHitPoints = TemporaryHitPoints = 0;
        HitDice = new Dictionary<int, int>();
    }
}
