using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resources : MonoBehaviour
{

    // Inspector settings
    public float energy_haste_factor;
    public float energy_pool_maximum;
    public float energy_potency;
    public float energy_reduction_rate;
    public float mana_haste_factor;
    public float mana_pool_maximum;
    public float magic_potency;
    public float mana_replenishment_rate;
    public float amber_pool_maximum;

    public Slider health_bar;
    public Slider mana_bar;
    public Transform stat_bars;

    // properties

    public Actor Actor { get; set; }
    public float CurrentAmber { get; set; }
    public float CurrentEnergy { get; set; }
    public float CurrentMana { get; set; }
    public bool IsCaster { get; set; }
    public bool IsPlayer { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    private void Start()
    {
        if (!IsPlayer) {
            StartCoroutine(StatBarsFaceCamera());
            StartCoroutine(ManageStatBars());
        }

        ManageResources();
    }

    // public


    public float CurrentAmberPercentage()
    {
        return CurrentAmber / amber_pool_maximum;
    }


    public float CurrentEnergyPercentage()
    {
        return CurrentEnergy / energy_pool_maximum;
    }


    public float CurrentManaPercentage()
    {
        return CurrentMana / mana_pool_maximum;
    }


    public void DecreaseEnergy(float _amount)
    {
        CurrentEnergy -= _amount;
        if (CurrentEnergy < 0) CurrentEnergy = 0;
    }


    public void DecreaseMana(float _amount)
    {
        CurrentMana -= _amount;
        if (CurrentMana < 0) CurrentMana = 0;
    }


    public void IncreaseEnergy(float _amount)
    {
        CurrentEnergy += _amount;
        if (CurrentEnergy > energy_pool_maximum) CurrentEnergy = energy_pool_maximum;
    }


    public void UpdateStatBars()
    {
        if (mana_bar != null)
        {
            mana_bar.value = CurrentManaPercentage();
            if (mana_bar.value >= 1 || !IsCaster)
            {
                mana_bar.gameObject.SetActive(false);
            }
            else
            {
                mana_bar.gameObject.SetActive(true);
            }
        }

        if (health_bar != null)
        {
            health_bar.value = Actor.Health.CurrentHealthPercentage();
            if (health_bar.value >= 1)
            {
                health_bar.gameObject.SetActive(false);
            }
            else
            {
                health_bar.gameObject.SetActive(true);
            }
        }
    }


    // private


    private IEnumerator DegenerateEnergy()
    {
        while (true)
        {
            if (CurrentEnergy > 0)  // don't check in the while condition; it will be true at first and prevent the coroutine from starting
            {
                CurrentEnergy -= energy_pool_maximum * energy_reduction_rate;
                UpdateStatBars();
            }
            else
            {
                CurrentEnergy = 0;
            }

            yield return new WaitForSeconds(Turn.action_threshold);
        }
    }


    private void ManageResources()
    {
        if (IsPlayer) {
            StartCoroutine(RegenerateAmber());
            StartCoroutine(RegenerateMana());
        } else {
            StartCoroutine(DegenerateEnergy());
            StartCoroutine(RegenerateMana());
        }

    }


    private IEnumerator ManageStatBars()
    {
        while (!Conflict.Victory && Actor.Health.MaximumHealth > 0)
        {
            UpdateStatBars();
            yield return new WaitForSeconds(Turn.action_threshold);
        }
    }


    private IEnumerator RegenerateAmber()
    {
        while (true)
        {
            if (CurrentAmber < amber_pool_maximum)
            {
                CurrentAmber++;
                CommandBarOne.Instance.amber_bar.value = CurrentAmberPercentage();
            } 
            else 
            {
                CurrentAmber = amber_pool_maximum;
            }

            yield return new WaitForSeconds(Turn.action_threshold);
        }
    }


    private IEnumerator RegenerateMana()
    {
        while (true) {
            if (IsCaster && CurrentMana < mana_pool_maximum) // don't check IsCaster in while (or outside enumerator); it will be false at first and prevent the coroutine from starting
            {  
                CurrentMana += mana_pool_maximum * mana_replenishment_rate;
                if (IsPlayer) CommandBarOne.Instance.mana_bar.value = CurrentManaPercentage();
            } 
            else 
            {
                CurrentMana = mana_pool_maximum;
            }

            yield return new WaitForSeconds(Turn.action_threshold);
        }
    }


    private void SetComponents()
    {
        Actor = GetComponentInParent<Actor>();
        CurrentEnergy = 0;
        CurrentMana = mana_pool_maximum;
        CurrentAmber = 0;
        IsCaster = false;
        IsPlayer = GetComponentInParent<Player>() != null;
    }


    private IEnumerator StatBarsFaceCamera()
    {
        while (true)
        {
            Vector3 stats_position = transform.position;
            Vector3 player_position = Player.Instance.viewport.transform.position;

            Quaternion rotation = Quaternion.LookRotation(player_position - stats_position, Vector3.up);
            stat_bars.rotation = rotation;

            yield return null;
        }
    }
}
