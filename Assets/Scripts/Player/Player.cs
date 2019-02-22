using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    // Inspector settings
    public float speed = 15f;
    public CinemachineFreeLook viewport;

    // properties

    public static Player Instance { get; set; }
    public Actor Me { get; set; }

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
        SetComponents();
        StartCoroutine(AdjustCameraDistance());
    }


    // public


    public void OnIdle()
    {
        Me.Actions.Movement.ResetPath();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }

    public void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }

    // private


    private IEnumerator AdjustCameraDistance()
    {
        while (true) {
            float proximity = Input.GetAxis("Mouse ScrollWheel") * 30f;
            if (!Mathf.Approximately(proximity, 0f)) {
                CinemachineFreeLook.Orbit[] orbits = viewport.m_Orbits;
                for (int i = 0; i < orbits.Length; i++) {
                    float orbit = orbits[i].m_Radius;
                    orbit += Mathf.Lerp(0, proximity, Time.deltaTime * 5f);
                    orbits[i].m_Radius = Mathf.Clamp(orbit, 2f, 50f);
                }
            }

            yield return null;
        }
    }


    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        Me.Actions = GetComponentInChildren<Actions>();
        Me.Alignment = Conflict.Alignment.Neutral;
        Me.Faction = GetComponent<Faction>();
        Me.Health = GetComponent<Health>();
        Me.Load = new Dictionary<HarvestingNode, int>();
        Me.RestCounter = 0;
        Me.Senses = GetComponent<Senses>();
        Me.Stats = GetComponent<Stats>();

        Me.Stats.ArmorClass = 18;
        Me.Stats.AttributeProficiency[Proficiencies.Attribute.Charisma] = 5;
        Me.Stats.AttributeProficiency[Proficiencies.Attribute.Constitution] = 2;
        Me.Stats.AttributeProficiency[Proficiencies.Attribute.Dexterity] = 4;
        Me.Stats.AttributeProficiency[Proficiencies.Attribute.Intelligence] = 3;
        Me.Stats.AttributeProficiency[Proficiencies.Attribute.Strength] = 1;
        Me.Stats.AttributeProficiency[Proficiencies.Attribute.Wisdom] = 5;
        Me.Stats.ProficiencyBonus = 6;
        Me.Stats.Family = "Humanoid";
        Me.Stats.Size = "Medium";

        Me.Actions.ActionsPerRound = 1;
        Me.Actions.Movement.ReachedThreshold = 1.5f;
        Me.Actions.Movement.Speed = speed;
        Me.Actions.Movement.Agent.speed = speed;

        Me.Health.HitDice = 20;
        Me.Health.HitDiceType = 8;

        Me.Health.SetCurrentAndMaxHitPoints();

        Me.Actions.Attack.AvailableWeapons = Characters.available_weapons[Characters.Template.Player];
        Me.Actions.Attack.EquipMeleeWeapon();
        Me.Actions.Attack.EquipRangedWeapon();
        Me.Senses.Darkvision = Characters.darkvision_range[Characters.Template.Player];
        Me.Senses.PerceptionRange = Characters.perception_range[Characters.Template.Base];
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];

        Me.Actions.OnIdle = OnIdle;
        Me.Actions.OnReachedGoal = OnReachedGoal;
    }
}
