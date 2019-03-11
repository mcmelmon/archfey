using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    // Inspector settings
    public float speed = 5;
    public CinemachineFreeLook viewport;

    // properties

    public bool GodOfRage { get; set; }
    public static Player Instance { get; set; }
    public Inventory Inventory { get; set; }
    public Actor Me { get; set; }

    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one player");
            Destroy(this);
            return;
        }
        Instance = this;
        Inventory = GetComponent<Inventory>();
    }


    private void Start()
    {
        SetComponents();
        SetNormalState();
        Enrage();
        StartCoroutine(AdjustCameraDistance());
        StartCoroutine(HandleMovement());
    }


    // public

    public void Enrage()
    {
        GodOfRage = true;
        SetSkills();
        Me.Actions.Movement.SpeedAdjustment = 2;
        Me.Actions.Movement.SpeedAdjustment += Me.Actions.Movement.SpeedAdjustment; // TODO: rework speed to use GetAdjustedSpeed
        Me.Health.GainTemporaryHitPoints(100);
        StartCoroutine(GodOfRageCountdown());
    }

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


    public void Unrage()
    {
        GodOfRage = false;
        SetSkills();
        Me.Health.TemporaryHitPoints = 0;
        Me.Actions.Movement.SpeedAdjustment -= Me.Actions.Movement.SpeedAdjustment;
        Me.Actions.Movement.SpeedAdjustment = 0;
        Me.ExhaustionLevel++;
    }

    // private


    private IEnumerator AdjustCameraDistance()
    {
        while (true) {
            float proximity = Input.GetAxis("Mouse ScrollWheel") * 30f;
            if (!Mathf.Approximately(proximity, 0f))
            {
                CinemachineFreeLook.Orbit[] orbits = viewport.m_Orbits;
                for (int i = 0; i < orbits.Length; i++) {
                    float orbit = orbits[i].m_Radius;
                    orbit -= Mathf.Lerp(0, proximity, Time.deltaTime * 5f);
                    orbits[i].m_Radius = Mathf.Clamp(orbit, 2f, 50f);
                }
            }

            yield return null;
        }
    }


    private IEnumerator GodOfRageCountdown()
    {
        int tick = 0;

        while (GodOfRage && tick < 60) {
            tick++;
            yield return new WaitForSeconds(1);
        }

        Unrage();
    }


    private IEnumerator HandleMovement()
    {
        while (true) {
            float translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float straffe = Input.GetAxis("Straffe") * speed * Time.deltaTime;
            float rotation = Input.GetAxis("Horizontal") * 60f * Time.deltaTime;

            transform.Translate(straffe, 0, translation);
            transform.Rotate(0, rotation, 0);

            if (Me.IsGrounded() && Input.GetKeyDown(KeyCode.Space))
            {
                Me.Actions.Movement.Jump();
            }

            if (Input.GetKeyDown("escape")) Cursor.lockState = CursorLockMode.None;

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
    }


    private void SetNormalState()
    {
        Me.Stats.BaseArmorClass = 10; // TODO: build up from equipment, dex, etc.
        Me.Stats.BaseAttributes[Proficiencies.Attribute.Charisma] = 3;
        Me.Stats.BaseAttributes[Proficiencies.Attribute.Constitution] = 1;
        Me.Stats.BaseAttributes[Proficiencies.Attribute.Dexterity] = 5;
        Me.Stats.BaseAttributes[Proficiencies.Attribute.Intelligence] = 0;
        Me.Stats.BaseAttributes[Proficiencies.Attribute.Strength] = -1;
        Me.Stats.BaseAttributes[Proficiencies.Attribute.Wisdom] = 0;

        Me.Senses.Darkvision = true;
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];

        Me.Health.HitDice = 8;
        Me.Health.HitDiceType = 8;
        Me.Health.SetCurrentAndMaxHitPoints();
        Me.Stats.ProficiencyBonus = 4;
        Me.Stats.Family = "Humanoid (goblinoid)";
        Me.Stats.Size = "Small";

        Me.Actions.Movement.ReachedThreshold = 2f;
        Me.Actions.Movement.BaseSpeed = speed;
        Me.Actions.Movement.Agent.speed = speed;

        Me.Actions.Attack.AttacksPerAction = 1;
        Me.Actions.Attack.AvailableWeapons = new List<Weapon>() { Weapons.Instance.GetWeaponNamed("lost_eye_axe"), Weapons.Instance.GetWeaponNamed("shortbow") };
        Me.Actions.Attack.EquipMeleeWeapon();
        Me.Actions.Attack.EquipRangedWeapon();

        Me.Actions.OnIdle = OnIdle;
        Me.Actions.OnReachedGoal = OnReachedGoal;
    }


    private void SetSkills()
    {
        Me.Stats.ClassFeatures.Clear();
        Me.Stats.Expertise.Clear();
        Me.Stats.SavingThrows.Clear();
        Me.Stats.Skills.Clear();
        Me.Stats.Tools.Clear();

        if (GodOfRage) {
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Constitution, 5);
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Dexterity, 2);
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Strength, 5);
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Wisdom, -1);

            Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Constitution);
            Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Dexterity);
            Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Strength);

            Me.Stats.ClassFeatures.Add("Battle Readiness");
            Me.Stats.ClassFeatures.Add("Danger Sense");
            Me.Stats.ClassFeatures.Add("Extra Attack");
            Me.Stats.ClassFeatures.Add("Improved Critical");
            Me.Stats.ClassFeatures.Add("Indomitable");
            Me.Stats.ClassFeatures.Add("Secondwind");
            Me.Stats.ClassFeatures.Add("Unarmored Defense");

            Me.Stats.Skills.Add(Proficiencies.Skill.Athletics);
            Me.Stats.Skills.Add(Proficiencies.Skill.Intimidation);

            Me.Actions.Attack.AttacksPerAction = 2;
            Me.Actions.Attack.Raging = true;
        } else {
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Constitution, 0);
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Dexterity, 0);
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Strength, 0);
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Wisdom, 0);

            Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Dexterity);
            Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Intelligence);

            Me.Stats.ClassFeatures.Add("Cunning Action");
            Me.Stats.ClassFeatures.Add("Evasion");
            Me.Stats.ClassFeatures.Add("Fast Hands");
            Me.Stats.ClassFeatures.Add("Helpful");
            Me.Stats.ClassFeatures.Add("Jack of Many Trades");
            Me.Stats.ClassFeatures.Add("Supreme Sneak");
            Me.Stats.ClassFeatures.Add("Inspiring Help");

            Me.Stats.Expertise.Add(Proficiencies.Skill.Perception);
            Me.Stats.Expertise.Add(Proficiencies.Skill.Performance);
            Me.Stats.Expertise.Add(Proficiencies.Skill.SleightOfHand);
            Me.Stats.Expertise.Add(Proficiencies.Skill.Stealth);

            Me.Stats.Skills.Add(Proficiencies.Skill.Perception);
            Me.Stats.Skills.Add(Proficiencies.Skill.Performance);
            Me.Stats.Skills.Add(Proficiencies.Skill.SleightOfHand);
            Me.Stats.Skills.Add(Proficiencies.Skill.Stealth);
            Me.Stats.Tools.Add("Thieves' tools");

            Me.Actions.Attack.AttacksPerAction = 1;
            Me.Actions.Attack.Raging = false;
        }
    }
}
