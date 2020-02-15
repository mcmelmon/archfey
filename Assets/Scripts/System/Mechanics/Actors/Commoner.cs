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
            Me.Actions.Movement.SetDestination(Me.Actions.Movement.Destinations[Movement.CommonDestination.Repair]);
        }
    }

    public override void OnFullLoad()
    {
        FindWarehouse();
    }
    public override void OnHarvesting()
    {
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
        Me.HasTask = false;
        MyHarvest = null;
        MyWarehouse = null;
        MyWorkshop = null;

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
        Me.Inventory.Empty();
    }

    private bool Craft()
    {
        if (!Proficiencies.Instance.IsArtisan(Me) || MyWorkshop == null) return false;

        return Me.HasTask && Me.Actions.Movement.AtCurrentDestination() && MyWorkshop.Craft(Me);
    }

    private void FindDamagedStructure()
    {
        Structure damaged_structure = FindObjectsOfType<Structure>()
            .Where(s => s.Faction == Me.CurrentFaction && s.CurrentHitPointPercentage() < 1f)
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
            .Where(s => s.Faction == Me.CurrentFaction && s.Use == Structure.Purpose.Military)
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        // Don't set as a CommonDestination just yet, because the commoner should run to nearest based on current location
        Me.Actions.Movement.SetDestination(nearest_military_structure.transform);
    }

    private void FindShrine()
    {
        Structure nearest_sacred_structure = FindObjectsOfType<Structure>()
            .Where(s => s.Faction == Me.CurrentFaction && s.Use == Structure.Purpose.Sacred)
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .ToList()
            .First();

        Me.Actions.Movement.SetDestination(nearest_sacred_structure.transform);
    }

    private void FindWarehouse()
    {
        if (!Me.Inventory.HasContents()) return;

        if (MyWarehouse != null) {
            if (Warehouse()) {
                Me.HasTask = false;
            }
            return;
        }

        foreach (var item in Me.Inventory.Contents) {
            Resource material = item.GetComponent<Resource>();
            Product product = item.GetComponent<Product>();

            if (material != null) {
                MyWarehouse = FindObjectsOfType<Structure>()
                    .Where(s => s.Faction == Me.CurrentFaction && s.Storage != null && s.Storage.materials_accepted.Contains(material))
                    .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
                    .ToList()
                    .First();

                if (MyWarehouse != null) break;
            } else if (product != null) {
                MyWarehouse = FindObjectsOfType<Structure>()
                    .Where(s => s.Faction == Me.CurrentFaction && s.Storage != null && s.Storage.products_accepted.Contains(product))
                    .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
                    .ToList()
                    .First();

                if (MyWarehouse != null) break;
            }
        }

        if (MyWarehouse == null) return;
        Me.HasTask = true;
        Me.Actions.Movement.SetDestination(MyWarehouse.RandomEntrance().transform.position);
    }

    private void FindWork()
    {
        if (MyHarvest != null || MyWorkshop != null) return; // work has found us

        if (Proficiencies.Instance.IsHarvester(Me)) {
            MyHarvest = new List<HarvestingNode>(FindObjectsOfType<HarvestingNode>())
                .Where(r => r.Owner == Me.CurrentFaction && r.AccessibleTo(Me))
                .OrderBy(r => Vector3.Distance(transform.position, r.transform.position))
                .ToList()
                .First();
            if (MyHarvest == null) return;
            Me.HasTask = true;
            Collider target_collider = MyHarvest.GetComponent<Collider>();
            Me.Actions.Movement.SetDestination(MyHarvest.transform);
        } else if (Proficiencies.Instance.IsArtisan(Me)) {
            MyWorkshop = FindObjectsOfType<Workshop>().First(ws => ws.UsefulTo(Me));
            if (MyWorkshop == null) return;
            Me.HasTask = true;
            Me.Actions.Movement.SetDestination(MyWorkshop.Storage.Structure.NearestEntranceTo(Me.transform));
        }
    }

    private void GoHome()
    {
        Me.Actions.Movement.Home();
    }


    private bool Harvest()
    {
        if (!Proficiencies.Instance.IsHarvester(Me) || MyHarvest == null) return false;

        if (Me.HasTask && Me.Actions.Movement.AtCurrentDestination()) {
            return MyHarvest.HarvestResource(Me);
        }

        return false;
    }


    private bool RepairStructure()
    {
        if (!Me.Actions.Movement.Destinations.ContainsKey(Movement.CommonDestination.Repair)) return false;

        var damaged_structures = FindObjectsOfType<Structure>()
            .Where(s => s.Faction == Me.CurrentFaction && s.CurrentHitPointPercentage() < 1f)
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
        if (!Me.Inventory.HasContents() || MyWarehouse == null || MyWarehouse.Storage == null) return false;
        
        if (Me.Actions.Movement.AtCurrentDestination()) {
            MyWarehouse.Storage.DeliverMaterials(Me);
            return true;
        }

        return false;
    }
}
