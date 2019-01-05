using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveControlPoint : MonoBehaviour
{
    // Inspector settings

    public Slider control_indicator;

    // properties

    public float ControlResistanceRating { get; set; }
    public float CurrentOccupationPoints { get; set; }
    public Conflict.Faction Faction { get; set; }
    public float MaximumOccupationPoints { get; set; }
    public Actor NearestActor { get; set; }
    public Objective Objective { get; set; }
    public bool Occupied { get; set; }


    // Unity


    private void Start()
    {
        SetComponents();
        UpdateControlIndicator();
        StartCoroutine(CheckOccupation());
        StartCoroutine(ControlIndicatorFaceCamera());
    }


    // private


    private IEnumerator CheckOccupation()
    {
        while (true) {
            NearestActor = FindNearestActor();

            if (NearestActor == null) { // everyone is dead
                break;
            } else {
                float nearest_actor_distance = Vector3.Distance(NearestActor.transform.position, transform.position);

                if (UnderAttack(nearest_actor_distance)) {
                    ReduceOccupation();
                } else {
                    if (Reinforce(nearest_actor_distance)) {
                        BoostOccupation();
                    }
                }
            }

            UpdateControlIndicator();
            yield return new WaitForSeconds(Turn.action_threshold);
        }

        // TODO: everyone is dead, scenario over
        ResetControl();
    }


    private void BoostOccupation()
    {
        if (CurrentOccupationPoints >= MaximumOccupationPoints) return;

        CurrentOccupationPoints += Mathf.Clamp((NearestActor.Actions.ObjectiveControlRating - ControlResistanceRating), 0, NearestActor.Actions.ObjectiveControlRating);
        if (Faction == Conflict.Faction.None) Faction = NearestActor.Faction;
        if (CurrentOccupationPoints >= MaximumOccupationPoints) {
            CurrentOccupationPoints = MaximumOccupationPoints;
            Occupied = true;
            foreach (var rend in Objective.renderers) {
                rend.material = (Faction == Conflict.Faction.Ghaddim) ? Objective.ghaddim_skin : Objective.mhoddim_skin;
            }
        }
    }


    private IEnumerator ControlIndicatorFaceCamera()
    {
        while (true) {
            yield return null;
            Vector3 control_position = transform.position;
            Vector3 player_position = Player.Instance.viewport.transform.position;

            Quaternion rotation = Quaternion.LookRotation(player_position - control_position, Vector3.up);
            control_indicator.transform.rotation = rotation;
        }
    }


    public float CurrentControlPercentage()
    {
        return (float)CurrentOccupationPoints / (float)MaximumOccupationPoints;
    }


    private Actor FindNearestActor()
    {
        float shortest_distance = float.MaxValue;
        float distance;
        Actor nearest_actor = null;

        for (int i = 0; i < Conflict.Units.Count; i++) {
            GameObject _unit = Conflict.Units[i];
            if (_unit == null || transform == null) continue;

            distance = Vector3.Distance(transform.position, _unit.transform.position);
            if (distance < shortest_distance) {
                shortest_distance = distance;
                nearest_actor = _unit.GetComponent<Actor>();
            }
        }

        return nearest_actor;
    }


    private bool Reinforce(float nearest_actor_distance)
    {
        return Faction == NearestActor.Faction && nearest_actor_distance < Route.reached_threshold;
    }


    private void ReduceOccupation()
    {
        if (CurrentOccupationPoints <= 0) return;

        CurrentOccupationPoints -= Mathf.Clamp((NearestActor.Actions.ObjectiveControlRating - ControlResistanceRating), 0, NearestActor.Actions.ObjectiveControlRating);
        if (CurrentOccupationPoints <= 0) {
            CurrentOccupationPoints = 0;
            Occupied = false;
            Faction = Conflict.Faction.None;
            foreach (var rend in Objective.renderers) {
                rend.material = Objective.unclaimed_skin;
            }
        }
    }


    private void ResetControl()
    {
        Occupied = false;
        Faction = Objective.initial_control;
        CurrentOccupationPoints = MaximumOccupationPoints;
    }


    private void SetComponents()
    {
        ControlResistanceRating = 2;  // TODO: specify in Inspector
        Objective = GetComponentInParent<Objective>();
        Occupied = false;

        Faction = Objective.Control;
        CurrentOccupationPoints = (Faction == Conflict.Faction.None) ? 0 : 100; // TODO: specify in Inspector

    }


    private bool UnderAttack(float nearest_actor_distance)
    {
        return NearestActor.Faction != Faction && nearest_actor_distance < Route.reached_threshold;
    }



    public void UpdateControlIndicator()
    {
        control_indicator.value = CurrentControlPercentage();
        if (Mathf.Approximately(control_indicator.value, 0) || Mathf.Approximately(control_indicator.value, 1)) {
            control_indicator.gameObject.SetActive(false);
        } else {
            control_indicator.gameObject.SetActive(true);
        }
    }
}