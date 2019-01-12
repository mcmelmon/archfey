using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    // Inspector settings
    public int armor_class = 13;
    public int damage_resistance = 0;
    public int maximum_hit_points = 100;
    public int current_hit_points = 100;


    // public


    public void GainStructure(int _amount)
    {
        if (current_hit_points == maximum_hit_points) return;

        current_hit_points += _amount;
        if (current_hit_points > maximum_hit_points) current_hit_points = maximum_hit_points;
        UpdateStructure();
    }


    public void LoseStructure(int _amount)
    {
        if (current_hit_points == 0) return;

        int reduced_amount = (_amount - damage_resistance > 0) ? _amount - damage_resistance : 0;
        current_hit_points -= reduced_amount;
        if (current_hit_points <= 0) current_hit_points = 0;
        UpdateStructure();
    }


    // private


    private float CurrentHitPointPercentage()
    {
        return ((float)current_hit_points / (float)maximum_hit_points);
    }


    private void UpdateStructure()
    {
        if (current_hit_points == maximum_hit_points) return;

        Vector3 scaling = transform.localScale;
        if (CurrentHitPointPercentage() < 0.66f)
        {
            scaling.y = scaling.y * 0.66f;
        }
        else if (CurrentHitPointPercentage() < 0.33f)
        {
            scaling.y = scaling.y * 0.33f;
        }
        else if (Mathf.Approximately(0f, CurrentHitPointPercentage()))
        {
            scaling.y = 0f;
        }

        transform.localScale = scaling;
    }
}
