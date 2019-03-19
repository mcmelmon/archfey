using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    // Inspector settings
    public CinemachineFreeLook viewport;

    // properties

    public bool GodOfRage { get; set; }
    public static Player Instance { get; set; }
    public PlayerInventory Inventory { get; set; }
    public Actor Me { get; set; }
    public EldritchSmite EldritchSmite { get; set; }


    // Unity


    private void Awake()
    {
        if (Instance != null) {
            Debug.LogError("More than one player");
            Destroy(this);
            return;
        }
        Instance = this;
        Inventory = GetComponent<PlayerInventory>();
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


    public int AdditionalDamage(GameObject target, bool is_ranged)
    {
        Actor victim = target.GetComponent<Actor>();

        int additional_damage = Me.Actions.Combat.HasSurprise(victim) ? Me.Actions.RollDie(6, 5) : 0;  // TODO: rogue only needs advantage, not surprise
        return additional_damage;
    }


    public void CastEldritchSmite(Actor target)
    {
        if (target != null && Vector3.Distance(target.transform.position, transform.position) < EldritchSmite.Range) {
            EldritchSmite.Cast(target);
        }
    }


    public void Enrage()
    {
        GodOfRage = true;
        SetSkills();
        Me.Actions.Movement.AdjustSpeed(0.1f); // results in 20 percent boost
        Me.Health.GainTemporaryHitPoints(100);
        StartCoroutine(GodOfRageCountdown());
    }

    public void OnIdle()
    {
        Me.Actions.SheathWeapon();
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
        Me.Stats.RageTick = 0;

        while (GodOfRage && Me.Stats.RageTick < Me.Stats.RageDuration) {
            Me.Stats.RageTick++;
            yield return new WaitForSeconds(1);
        }

        Unrage();
    }


    private IEnumerator HandleMovement()
    {
        while (true) {
            float translation = Input.GetAxis("Vertical") * Me.Actions.Movement.GetAdjustedSpeed() * Time.deltaTime;
            float straffe = Input.GetAxis("Straffe") * Me.Actions.Movement.GetAdjustedSpeed() * Time.deltaTime;
            float rotation = Input.GetAxis("Horizontal") * 60f * Time.deltaTime;

            Me.Actions.Movement.NonAgentMovement = false;

            if (!Mathf.Approximately(0, translation) && !Mathf.Approximately(0, straffe)) {
                Me.Actions.Movement.NonAgentMovement = true;
                Me.Actions.CanTakeAction = false;
            }

            transform.Translate(straffe, 0, translation);
            transform.Rotate(0, rotation, 0);

            if (Me.IsGrounded() && Input.GetKeyDown(KeyCode.Space)) {
                Me.Actions.Movement.Jump();
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
    }


    private void SetNormalState()
    {
        Me.Stats.BaseArmorClass = 10;
        Me.Stats.BaseAttributes[Proficiencies.Attribute.Charisma] = 3;
        Me.Stats.BaseAttributes[Proficiencies.Attribute.Constitution] = 1;
        Me.Stats.BaseAttributes[Proficiencies.Attribute.Dexterity] = 5;
        Me.Stats.BaseAttributes[Proficiencies.Attribute.Intelligence] = 0;
        Me.Stats.BaseAttributes[Proficiencies.Attribute.Strength] = -1;
        Me.Stats.BaseAttributes[Proficiencies.Attribute.Wisdom] = -1;

        Me.Senses.Darkvision = true;
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];

        Me.Health.HitDice = 7;
        Me.Health.HitDiceType = 8;
        Me.Health.SetCurrentAndMaxHitPoints();
        Me.Stats.ProficiencyBonus = 3;
        Me.Stats.Family = "Humanoid (goblinoid)";
        Me.Stats.Size = "Small";

        Me.Actions.Movement.ReachedThreshold = 2f;
        Me.Actions.Movement.BaseSpeed = 5;
        Me.Actions.Movement.Agent.speed = 5;

        Me.Actions.Combat.Raging = true;
        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.None));

        Me.Actions.OnIdle = OnIdle;
        Me.Actions.OnReachedGoal = OnReachedGoal;
        Me.Actions.Combat.CalculateAdditionalDamage = AdditionalDamage;
    }


    private void SetSkills()
    {
        Me.Stats.ClassFeatures.Clear();
        Me.Stats.ExpertiseInSkills.Clear();
        Me.Stats.SavingThrows.Clear();
        Me.Stats.Skills.Clear();
        Me.Stats.Tools.Clear();

        if (GodOfRage) {
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Constitution, 5);
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Dexterity, -3);
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Strength, 5);

            Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Constitution);
            Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Dexterity);
            Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Strength);

            Me.Stats.ClassFeatures.Add("Devil's Sight");
            Me.Stats.ClassFeatures.Add("Eldritch Smite");
            Me.Stats.ClassFeatures.Add("Ghostly Gaze");
            Me.Stats.ClassFeatures.Add("Thirsting Blade");

            Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Battleaxe, "lost_eye_axe"));
            Me.Actions.Combat.AttacksPerAction = 2;

            CommandBarOne.Instance.ActivateButtonSet("Warlock");
            if (EldritchSmite == null) EldritchSmite = gameObject.AddComponent<EldritchSmite>();
        }
        else {
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Constitution, 0);
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Dexterity, 0);
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Strength, 0);
            Me.Stats.AdjustAttribute(Proficiencies.Attribute.Wisdom, 0);

            Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Dexterity);
            Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Intelligence);

            Me.Stats.ClassFeatures.Add("Cunning Action");
            Me.Stats.ClassFeatures.Add("Evasion");
            Me.Stats.ClassFeatures.Add("Fast Hands");
            Me.Stats.ClassFeatures.Add("Second Story Work");
            Me.Stats.ClassFeatures.Add("Sneak Attack");
            Me.Stats.ClassFeatures.Add("Thieves' Cant");
            Me.Stats.ClassFeatures.Add("Uncanny Dodge");

            Me.Stats.ExpertiseInSkills.Add(Proficiencies.Skill.Perception);
            Me.Stats.ExpertiseInSkills.Add(Proficiencies.Skill.Performance);
            Me.Stats.ExpertiseInSkills.Add(Proficiencies.Skill.SleightOfHand);
            Me.Stats.ExpertiseInSkills.Add(Proficiencies.Skill.Stealth);

            Me.Stats.Skills.Add(Proficiencies.Skill.Perception);
            Me.Stats.Skills.Add(Proficiencies.Skill.Performance);
            Me.Stats.Skills.Add(Proficiencies.Skill.SleightOfHand);
            Me.Stats.Skills.Add(Proficiencies.Skill.Stealth);
            Me.Stats.Tools.Add(Proficiencies.Tool.Thief);

            Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Dagger));
            Me.Actions.Combat.EquipOffhand(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Dagger));
            Me.Actions.Combat.AttacksPerAction = 1;
            Me.Actions.Combat.Raging = false;
            CommandBarOne.Instance.ActivateButtonSet("Thief");
        }
    }


    private void Unrage()
    {
        GodOfRage = false;
        Me.Actions.SheathWeapon();
        Me.Health.ClearTemporaryHitPoints();
        Me.Actions.Movement.ResetSpeed();
        Me.ExhaustionLevel++;

        SetSkills();
    }
}
