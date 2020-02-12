using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour, IAct {

    // Inspector settings
    public CinemachineFreeLook viewport;
    public Faction player_faction;

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
        SetSkills();

        StartCoroutine(AdjustCameraDistance());
        StartCoroutine(HandleMovement());

        // Set home here to avoid overwriting it during respawns
        Me.Actions.Movement.AddDestination(Movement.CommonDestination.Home, transform.position);
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
        if (target != null && Vector3.Distance(target.transform.position, transform.position) < EldritchSmite.Range)
        {
            EldritchSmite.Cast(target);
        }
    }


    public void OnBadlyInjured() { }
    public void OnCrafting() { }
    public void OnFriendsInNeed() { }
    public void OnFriendlyActorsSighted() { }
    public void OnFullLoad() { }
    public void OnDamagedFriendlyStructuresSighted() { }
    public void OnHarvesting() { }
    public void OnHasObjective() { }
    public void OnHostileActorsSighted() { }
    public void OnHostileStructuresSighted() { }
    public void OnInCombat() { }
    public void OnMedic() { }
    public void OnMovingToGoal() { }
    public void OnNeedsRest() { }
    public void OnUnderAttack() { }
    public void OnWatch() { }


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


    public void Respawn()
    {
        SetComponents();
        SetNormalState();
        SetSkills();
        transform.position = Me.Actions.Movement.Destinations[Movement.CommonDestination.Home];
        Me.ChangeFaction(player_faction);
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


    private IEnumerator HandleMovement()
    {
        while (true) {

            // TODO: Use the new InputSystem


            // Touch to move
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
               Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
               int ground_mask = LayerMask.GetMask("Ground");

               if (Physics.Raycast(ray, out RaycastHit hit, 150f, ground_mask, QueryTriggerInteraction.Ignore)) {
                   Me.Actions.Movement.SetDestination(hit.point);
               }
            } else {
                float rotation, strafe, translation;

                translation = CrossPlatformInputManager.GetAxis("Vertical") * Me.Actions.Movement.GetAdjustedSpeed() * Time.deltaTime;
                strafe = CrossPlatformInputManager.GetAxis("Strafe") * Me.Actions.Movement.GetAdjustedSpeed() * Time.deltaTime;
                rotation = CrossPlatformInputManager.GetAxis("Horizontal") * 60f * Time.deltaTime;

                if (!Mathf.Approximately(0, translation) || !Mathf.Approximately(0, strafe)) {
                    Me.Actions.CanTakeAction = false;
                }

                transform.Translate(strafe, 0, translation);
                transform.Rotate(0, rotation, 0);

                if (Me.IsGrounded() && Input.GetKeyDown(KeyCode.Space)) {
                    Me.Actions.Movement.Jump();
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
        Me.CurrentFaction = player_faction;
        Me.Health = GetComponent<Health>();
        Me.Load = new Dictionary<HarvestingNode, int>();
        Me.RestCounter = 0;
        Me.Senses = GetComponent<Senses>();
        Me.Stats = GetComponent<Stats>();
    }


    private void SetNormalState()
    {
        Me.Stats.BaseArmorClass = 10;
        Me.Stats.Attributes[Proficiencies.Attribute.Charisma] = 10;
        Me.Stats.Attributes[Proficiencies.Attribute.Constitution] = 10;
        Me.Stats.Attributes[Proficiencies.Attribute.Dexterity] = 10;
        Me.Stats.Attributes[Proficiencies.Attribute.Intelligence] = 10;
        Me.Stats.Attributes[Proficiencies.Attribute.Strength] = 10;
        Me.Stats.Attributes[Proficiencies.Attribute.Wisdom] = 10;

        Me.Senses.Darkvision = false;
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];

        Me.Health.HitDice = 1;
        Me.Health.HitDiceType = 8;
        Me.Health.SetCurrentAndMaxHitPoints();
        Me.Stats.ProficiencyBonus = 2;
        Me.Stats.Family = Stats.CreatureFamily.Humanoid;
        Me.Stats.Subfamily = Stats.CreatureSubfamily.Human;
        Me.Stats.Size = Stats.Sizes.Medium;

        Me.Actions.Movement.ReachedThreshold = 2f;
        Me.Actions.Movement.BaseSpeed = 5;
        Me.Actions.Movement.Agent.speed = 5;

        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.Leather));

        Me.Actions.Combat.CalculateAdditionalDamage = AdditionalDamage;
    }


    private void SetSkills()
    {
        Me.Stats.ClassFeatures.Clear();
        Me.Stats.ExpertiseInSkills.Clear();
        Me.Stats.SavingThrows.Clear();
        Me.Stats.Skills.Clear();
        Me.Stats.Tools.Clear();

        Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Intelligence);
        Me.Stats.SavingThrows.Add(Proficiencies.Attribute.Wisdom);

        Me.Stats.ClassFeatures.Add("Druid State Class Feature");  // TODO: the features will provide benefits after "leveling up"
        Me.Stats.ExpertiseInSkills.Add(Proficiencies.Skill.Insight);

        Me.Stats.Skills.Add(Proficiencies.Skill.Insight);
        Me.Stats.Skills.Add(Proficiencies.Skill.Perception);
        Me.Stats.Tools.Add(Proficiencies.Tool.Herbalist);

        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Quarterstaff));
        Me.Actions.Combat.AttacksPerAction = 1;
        CommandBarOne.Instance.ActivateButtonSet("Druid");
    }
}
