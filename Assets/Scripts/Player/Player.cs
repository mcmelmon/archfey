using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour{

    // Inspector settings
    public CinemachineFreeLook viewport;
    public Faction player_faction;

    // properties

    public Actor Me { get; set; }
    public static Player Instance { get; set; }
    public PlayerInventory Inventory { get; set; }
    public Warlock Warlock { get; set; }

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
        SetComponents();
    }

    private void Start()
    {
        SetAdditionalComponents();

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

    public void Respawn()
    {
        transform.position = Me.Actions.Movement.Destinations[Movement.CommonDestination.Home];
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
                    transform.Translate(strafe, 0, translation);
                }

                transform.Rotate(0, rotation, 0);

                if (Me.IsGrounded() && Input.GetKeyDown(KeyCode.Space)) {
                    Me.Actions.Movement.Jump();
                }
            }

            yield return null;
        }
    }

    private void SetAdditionalComponents()
    {
        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Quarterstaff));
        Me.Actions.Combat.AttacksPerAction = 1;

        if (CommandBarOne.Instance != null) CommandBarOne.Instance.ActivateButtonSet("Warlock");
    }

    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        Me.Actions = GetComponentInChildren<Actions>();
        Me.Alignment = Conflict.Alignment.Neutral;
        Me.CurrentFaction = player_faction;
        Me.Health = GetComponent<Health>();
        Me.RestCounter = 0;
        Me.Senses = GetComponent<Senses>();
        Me.Stats = GetComponent<Stats>();
        
        Warlock = Me.gameObject.AddComponent<Warlock>();
    }

}
