using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Commoner : TemplateMelee
{
    // properties

    public HarvestingNode MyHarvest { get; set; }
    public Structure MyWarehouse { get; set; }
    public Workshop MyWorkshop { get; set; }


    // Unity


    private void Start()
    {
        SetComponents();
    }


    // public


    public override void OnBadlyInjured()
    {
        Me.Actions.Movement.Home();
    }


    public override void OnCrafting() { }


    public override void OnFriendsInNeed()
    {
        if (Me.Actions.Decider.FriendsInNeed.First() != null) {
            Me.Actions.Movement.SetDestination(Me.Actions.Decider.FriendsInNeed.First().transform);
        }
        Me.Actions.Attack();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }


    public override void OnDamagedFriendlyStructuresSighted()
    {
        if (!RepairStructure()) {
            FindDamagedStructure();
            Me.Actions.Movement.ResetPath();
            Me.Actions.Movement.SetDestination(Me.Actions.Movement.Destinations[Movement.CommonDestination.Repair]);
        }
    }


    public override void OnFullLoad()
    {
        FindWarehouse();
        Me.Actions.Movement.Warehouse();
    }


    public override void OnHarvesting()
    {
        Me.Actions.Movement.ResetPath();
        Harvest();
    }


    public override void OnHostileActorsSighted()
    {
        if (Me.Actions.Decider.FriendsInNeed.Count == 0) {
            AbandonLoad();
            FindGuard();
        }
    }


    public override void OnHostileStructuresSighted()
    {
        Me.Actions.CallForHelp();
        Me.Actions.FleeFromEnemies();
    }


    public override void OnIdle()
    {
        Me.Actions.SheathWeapon();

        if (Me.Route.local_stops.Length > 1) {
            Me.Route.MoveToNextPosition();
        } else {
            FindWork();
            Me.Actions.Movement.Work();
        }
    }


    public override void OnNeedsRest()
    {
        Me.Actions.SheathWeapon();
        Me.Actions.Movement.Home();
    }


    public override void OnReachedGoal()
    {
        Me.Actions.Movement.ResetPath();

        if (!Harvest()) {
            if (!Craft()) {
                if (!Warehouse()) {
                    if (!RepairStructure()) {
                        OnIdle();
                    }
                }
            }
        }
    }


    // private


    private void AbandonLoad()
    {
        Me.Load.Clear();
    }


    private bool Craft()
    {
        if (!Proficiencies.Instance.IsArtisan(Me) || MyWorkshop == null) return false;

        float distance = Vector3.Distance(Me.Actions.Movement.Destinations[Movement.CommonDestination.Craft], transform.position);

        return distance < Me.Actions.Movement.ReachedThreshold && MyWorkshop.Craft(Me);
    }


    private void FindDamagedStructure()
    {
        Structure damaged_structure = FindObjectsOfType<Structure>()
            .Where(s => s.alignment == Me.Alignment && s.CurrentHitPointPercentage() < 1f)
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        if (damaged_structure == null) return;

        Collider _collider = damaged_structure.GetComponent<Collider>();
        Me.Actions.Movement.AddDestination(Movement.CommonDestination.Repair, _collider.ClosestPointOnBounds(transform.position));
    }


    private void FindGuard()
    {
        Structure nearest_military_structure = FindObjectsOfType<Structure>()
            .Where(s => s.alignment == Me.Alignment && s.purpose == Structure.Purpose.Military)
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        // Don't set as a CommonDestination just yet, because the commoner should run to nearest based on current location
        Me.Actions.Movement.SetDestination(nearest_military_structure.transform);
    }


    private void FindShrine()
    {
        Structure nearest_sacred_structure = FindObjectsOfType<Structure>()
            .Where(s => s.alignment == Me.Alignment && s.purpose == Structure.Purpose.Sacred)
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        Me.Actions.Movement.SetDestination(nearest_sacred_structure.transform);
    }


    private void FindWarehouse()
    {
        MyWarehouse = FindObjectsOfType<Structure>()
            .Where(s => s.alignment == Me.Alignment && s.Storage != null && s.MaterialsWanted().Contains(Me.Load.First().Key.material))
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        if (MyWarehouse == null) return;

        Vector3 entrance = MyWarehouse.RandomEntrance().transform.position;
        Vector3 grounded_entrance = new Vector3(entrance.x, Geography.Terrain.SampleHeight(entrance), entrance.z);

        Me.Actions.Movement.AddDestination(Movement.CommonDestination.Warehouse, grounded_entrance);
    }


    private void FindWork()
    {
        if (MyHarvest != null || MyWorkshop != null) return; // work has found us

        if (Proficiencies.Instance.IsHarvester(Me)) {
            MyHarvest = new List<HarvestingNode>(FindObjectsOfType<HarvestingNode>())
                .Where(r => r.owner == Me.Alignment && r.AccessibleTo(Me))
                .OrderBy(r => Vector3.Distance(transform.position, r.transform.position))
                .ToList()
                .First();

            if (MyHarvest == null) return;

            Collider _collider = MyHarvest.GetComponent<Collider>();
            Me.Actions.Movement.AddDestination(Movement.CommonDestination.Harvest, _collider.ClosestPointOnBounds(transform.position));

        } else if (Proficiencies.Instance.IsArtisan(Me)) {
            MyWorkshop = FindObjectsOfType<Workshop>().First(ws => ws.UsefulTo(Me));
            if (MyWorkshop == null) return;

            Collider _collider = MyWorkshop.GetComponent<Collider>();
            Me.Actions.Movement.AddDestination(Movement.CommonDestination.Craft, _collider.ClosestPointOnBounds(transform.position));
        }
    }


    private void GoHome()
    {
        Me.Actions.Movement.Home();
    }


    private bool Harvest()
    {
        if (!Proficiencies.Instance.IsHarvester(Me) || MyHarvest == null) return false;

        // MyHarvest is the node itself, not the "destination"
        float distance = Vector3.Distance(Me.Actions.Movement.Destinations[Movement.CommonDestination.Harvest], transform.position);

        if (distance <= Me.Actions.Movement.ReachedThreshold) {
            MyHarvest.HarvestResource(Me);
            return true;
        }

        return false;
    }


    private bool RepairStructure()
    {
        if (!Me.Actions.Movement.Destinations.ContainsKey(Movement.CommonDestination.Repair)) return false;

        var damaged_structures = FindObjectsOfType<Structure>()
            .Where(s => s.alignment == Me.Alignment && s.CurrentHitPointPercentage() < 1f)
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position));

        if (!damaged_structures.Any()) {
            Me.Actions.Movement.Destinations.Remove(Movement.CommonDestination.Repair);
            return false;
        }

        Structure structure = damaged_structures.First();
        Vector3 destination = structure.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        float distance = Vector3.Distance(destination, transform.position);

        if (distance <= Me.Actions.Movement.ReachedThreshold) {
            structure.GainStructure(Me.Stats.ProficiencyBonus * 2);
            return true;
        }

        return false;
    }


    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        SetAdditionalStats();
    }


    private void SetAdditionalStats()
    {
        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.None));
        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Club));
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
    }


    private bool Warehouse()
    {
        if (Me.Load.Count <= 0 || MyWarehouse == null) return false;

        float distance = Vector3.Distance(Me.Actions.Movement.Destinations[Movement.CommonDestination.Warehouse], transform.position);
        
        if (distance <= Me.Actions.Movement.ReachedThreshold) {
            MyWarehouse.DeliverMaterials(Me, Random.Range(1, 12) * .1f); // TODO: base amount on resources!
            return true;
        }

        return false;
    }
}
