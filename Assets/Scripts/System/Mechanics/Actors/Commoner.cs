using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Commoner : TemplateMelee
{
    // properties

    public HarvestingNode MyHarvest { get; set; }
    public List<HarvestingNode> MyHarvestPath {get; set; }
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

    public override void OnDamagedFriendlyStructuresSighted()
    {
        if (!RepairStructure()) {
            FindDamagedStructure();
            Me.Actions.Movement.SetDestination(Me.Actions.Movement.Destinations[Movement.CommonDestination.Repair]);
        }
    }

    public override void OnFriendsInNeed()
    {
        if (Me.Actions.Decider.FriendsInNeed.First() != null) {
            Me.Actions.Movement.SetDestination(Me.Actions.Decider.FriendsInNeed.First().transform);
        }
        Me.Actions.Attack();
        Me.Actions.Decider.FriendsInNeed.Clear();
    }

    public override void OnFullLoad()
    {
        FindWarehouse();
    }
    public override void OnHarvesting()
    {
        if (Me.Senses.PerceptionCheck(true, MyHarvest.ChallengeRating)) {
            Debug.Log("Spotted node");
            Harvest();
        } else {
            Debug.Log("Failed to spot node");
            ChooseHarvest();
        }
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
        }
    }
    public override void OnNeedsRest()
    {
        Me.Actions.SheathWeapon();
        Me.Actions.Movement.Home();
    }
    public override void OnReachedGoal()
    {
        Debug.Log("Reached goal");
        if (!Harvest()) {
            Debug.Log("Failed to harvest");
            if (!Craft()) {
                Debug.Log("No craft");
                if (!Warehouse()) {
                    Debug.Log("No warehouse");
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

    private bool ChooseHarvest()
    {
        if (MyHarvestPath.Any()) {
            if (MyHarvest == null) {
                MyHarvest = MyHarvestPath.First();
            } else {
                int current_harvest_index = MyHarvestPath.IndexOf(MyHarvest);
                int next_harvest_index = current_harvest_index += 1;
                MyHarvest = MyHarvestPath[next_harvest_index % MyHarvestPath.Count];
            }
            
            if (MyHarvest != null) {
                Me.HasTask = true;
                Me.Actions.Movement.SetDestination(MyHarvest.transform);
                return true;
            }
        }

        Me.HasTask = false;
        Me.Actions.Movement.ClearCurrentDestination();
        return false;
    }

    private bool ChooseWarehouse()
    {
        if (Me.Inventory.StorageCount() > 0) {
            foreach (var item in Me.Inventory.Contents) {
                List<Structure> available = FindObjectsOfType<Structure>().Where(s => s.IsOpenToMe(Me) && s.WillAccept(item)).ToList();

                if (available.Any()) {
                    MyWarehouse = available.OrderBy(s => Vector3.Distance(transform.position, s.transform.position)).First();
                    Me.HasTask = true;
                    return true;
                }
            }
        } 
        
        MyWarehouse = null;
        Me.HasTask = false;
        return false;
    }

    private bool ChooseWorkshop()
    {
        List<Workshop> available = FindObjectsOfType<Workshop>().Where(ws => ws.UsefulTo(Me) && ws.Structure.IsOpenToMe(Me)).ToList();
        if (available.Any()) {
            MyWorkshop = available.OrderBy(ws => Vector3.Distance(ws.transform.position, transform.position)).First();
            if (MyWorkshop == null) return false;
            Me.HasTask = true;
            Me.Actions.Movement.SetDestination(MyWorkshop.Structure.NearestEntranceTo(Me.transform));
            return true;
        }

        return false;
    }

    private bool Craft()
    {
        if (!Proficiencies.Instance.IsArtisan(Me) || MyWorkshop == null) return false;

        return Me.HasTask && Me.Actions.Movement.AtCurrentDestination() && MyWorkshop.CraftByArtisan(Me);
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

        if (ChooseWarehouse()) {
            Me.Actions.Movement.SetDestination(MyWarehouse.NearestEntranceTo(Me.transform));
        }
    }

    private void FindWork()
    {
        if (MyHarvest != null || MyWarehouse != null || MyWorkshop != null) return; // work has found us

        if (Proficiencies.Instance.IsHarvester(Me)) {
            if (ChooseHarvest()) return;
        } else if (Proficiencies.Instance.IsArtisan(Me)) {
            if (ChooseWorkshop()) return;
        }

        // No work is available, so see if we can drop anything off despite not having a full load
        if (ChooseWarehouse()) {
            Me.Actions.Movement.SetDestination(MyWarehouse.NearestEntranceTo(Me.transform));
        } else {
            Me.Actions.Movement.ClearCurrentDestination(); // nothing to do and nowhere to be
        }
    }

    private void GoHome()
    {
        Me.Actions.Movement.Home();
    }


    private bool Harvest()
    {
        if (!Proficiencies.Instance.IsHarvester(Me) || MyHarvest == null) return false;

        if (MyHarvest.CurrentlyAvailable <= 0) return ChooseHarvest();

        return Me.HasTask && Me.Actions.Movement.AtCurrentDestination() && MyHarvest.HarvestResource(Me);
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

        if (distance <= Me.Actions.Movement.StoppingDistance()) {
            structure.GainStructure(Me.Stats.ProficiencyBonus * 2);
            return true;
        }

        return false;
    }

    private void SetAdditionalStats()
    {
        Me.Actions.Combat.EquipArmor(Armors.Instance.GetArmorNamed(Armors.ArmorName.None));
        Me.Actions.Combat.EquipMeleeWeapon(Weapons.Instance.GetWeaponNamed(Weapons.WeaponName.Club));
        Me.Stats.Resistances = Characters.resistances[Characters.Template.Base];
    }

    private void SetComponents()
    {
        Me = GetComponent<Actor>();
        SetHarvestPath();
        SetAdditionalStats();
    }

    private void SetHarvestPath()
    {
        List<HarvestingNode> available = FindObjectsOfType<HarvestingNode>().Where(node => node.AccessibleTo(Me)).ToList();
        if (available.Any()) {
            MyHarvestPath = available.OrderBy(h => Vector3.Distance(h.transform.position, transform.position)).ToList();
        }
    }


    private bool Warehouse()
    {
        if (!Me.Inventory.HasContents() || MyWarehouse == null || MyWarehouse.Inventory == null) return false;
        
        if (Me.Actions.Movement.AtCurrentDestination()) {
            MyWarehouse.DeliverMaterials(Me);
            return true;
        }

        return false;
    }
}
