using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    // Inspector settings
    public float agility = 30f;
    public float speed = 12f;
    public float mana_haste_factor;
    public float mana_pool_maximum;
    public float magic_potency;
    public float mana_replenishment_rate;
    public float amber_pool_maximum;

    public CinemachineFreeLook viewport;

    // properties

    public float CurrentAmber { get; set; }
    public float CurrentMana { get; set; }
    public static Player Instance { get; set; }

    // Unity


    void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one player");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    private void Start()
    {
        StartCoroutine(Movement());
        StartCoroutine(RegenerateAmber());
        StartCoroutine(RegenerateMana());
    }


    // public


    public float CurrentAmberPercentage()
    {
        return (CurrentAmber >= amber_pool_maximum) ? 1 : CurrentAmber / amber_pool_maximum;
    }



    public float CurrentManaPercentage()
    {
        return (CurrentMana >= mana_pool_maximum) ? 1 : CurrentMana / mana_pool_maximum;
    }


    public void DecreaseAmber(float _amount)
    {
        CurrentAmber -= _amount;
        if (CurrentAmber < 0) CurrentAmber = 0;
    }


    public void IncreaseAmber(float _amount)
    {
        CurrentAmber += _amount;
        if (CurrentAmber > amber_pool_maximum) CurrentAmber = amber_pool_maximum;
    }


    public void DecreaseMana(float _amount)
    {
        CurrentMana -= _amount;
        if (CurrentMana < 0) CurrentMana = 0;
    }


    public void IncreaseMana(float _amount)
    {
        CurrentMana += _amount;
        if (CurrentMana > mana_pool_maximum) CurrentMana = mana_pool_maximum;
    }


    public void UpdateAmberBar() 
    {
        CommandBarOne.Instance.amber_bar.value = CurrentAmberPercentage();

    }


    public void UpdateManaBar()
    {
        CommandBarOne.Instance.mana_bar.value = CurrentManaPercentage();
    }


    // private


    private void AdjustCameraDistance()
    {
        float proximity = Input.GetAxis("Mouse ScrollWheel") * 20f;
        if (!Mathf.Approximately(proximity, 0f)) {
            CinemachineFreeLook.Orbit[] orbits = viewport.m_Orbits;
            for (int i = 0; i < orbits.Length; i++) {
                orbits[i].m_Radius -= Mathf.Lerp(0, proximity, Time.deltaTime * 5f);
            }
        }
    }


    private IEnumerator Movement()
    {
        while (true)
        {
            yield return null;
            Vector3 movement = Vector3.zero;

            float forward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float rotation = Input.GetAxis("Horizontal") * agility * Time.deltaTime;

            if (!Mathf.Approximately(forward, 0) || !Mathf.Approximately(rotation, 0))
            {
                transform.rotation *= Quaternion.AngleAxis(rotation, Vector3.up);
                transform.position += transform.TransformDirection(Vector3.forward) * forward;
                AdjustCameraDistance();
            }
        }
    }


    private IEnumerator RegenerateAmber()
    {
        while (true)
        {
            if (CurrentAmber < amber_pool_maximum) {
                IncreaseAmber(1);
            } else {
                CurrentAmber = amber_pool_maximum;
            }

            UpdateAmberBar();
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private IEnumerator RegenerateMana()
    {
        while (true)
        {
            if (CurrentMana < mana_pool_maximum) // don't check IsCaster in while (or outside enumerator); it will be false at first and prevent the coroutine from starting 
            {
                IncreaseMana(mana_pool_maximum * mana_replenishment_rate);
            } else {
                CurrentMana = mana_pool_maximum;
            }

            UpdateManaBar();
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }
    }


    private void SetComponents()
    {
        CurrentMana = mana_pool_maximum;
        CurrentAmber = 0;
    }
}
