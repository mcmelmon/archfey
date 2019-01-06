using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveControlPoint : MonoBehaviour
{
    // Inspector settings

    public Slider control_indicator;

    // properties

    public List<Actor> Attackers { get; set; }
    public Conflict.Faction ControllingFaction { get; set; }
    public float ControlResistanceRating { get; set; }
    public float CurrentOccupationPoints { get; set; }
    public List<Actor> Defenders { get; set; }
    public float MaximumOccupationPoints { get; set; }
    public Conflict.Faction OccupyingFaction { get; set; }
    public Objective Objective { get; set; }
    public bool Controlled { get; set; }


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
            IdentifyFriendAndFoe();

            // don't use foreach because the units will get destroyed in flight
            for (int i = 0; i < Defenders.Count; i++) {
                BoostOccupation(Defenders[i]);
            }

            for (int i = 0; i < Attackers.Count; i++) {
                ReduceOccupation(Attackers[i]);
            }

            UpdateControlIndicator();
            yield return new WaitForSeconds(Turn.action_threshold);
        }
    }


    private void BoostOccupation(Actor defender)
    {
        if (defender == null) return;

        if (CurrentOccupationPoints >= MaximumOccupationPoints) return;

        CurrentOccupationPoints += Mathf.Clamp((defender.Actions.ObjectiveControlRating - ControlResistanceRating), 0, defender.Actions.ObjectiveControlRating);
        if (CurrentOccupationPoints >= MaximumOccupationPoints) {
            CurrentOccupationPoints = MaximumOccupationPoints;
            ControllingFaction = OccupyingFaction = defender.Faction;
            Controlled = true;
            foreach (var rend in Objective.renderers) {
                rend.material = (ControllingFaction == Conflict.Faction.Ghaddim) ? Objective.ghaddim_skin : Objective.mhoddim_skin;
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


    private void IdentifyFriendAndFoe()
    {
        float distance;

        for (int i = 0; i < Attackers.Count; i++) {
            Attackers[i].Actions.Decider.ObjectiveUnderContention = null;
        }

        for (int i = 0; i < Defenders.Count; i++) {
            Defenders[i].Actions.Decider.ObjectiveUnderContention = null;
        }

        Attackers.Clear();
        Defenders.Clear();

        for (int i = 0; i < Conflict.Units.Count; i++) {
            if (Conflict.Units[i] == null) continue;

            Actor _unit = Conflict.Units[i].GetComponent<Actor>();
            if (_unit == null) continue;

            distance = Vector3.Distance(transform.position, _unit.transform.position);
            if (distance < Route.reached_threshold) {
                if (_unit.Faction == ControllingFaction) {
                    Defenders.Add(_unit);
                } else if (ControllingFaction == Conflict.Faction.None && _unit.Faction == OccupyingFaction) {
                    Defenders.Add(_unit);
                    _unit.Actions.Decider.ObjectiveUnderContention = this;
                }
                else {
                    Attackers.Add(_unit);
                    _unit.Actions.Decider.ObjectiveUnderContention = this;
                }
            }
        }
    }


    private void ReduceOccupation(Actor attacker)
    {
        if (attacker == null) return;

        if (CurrentOccupationPoints <= 0) {
            CurrentOccupationPoints = 0;
            OccupyingFaction = attacker.Faction;
            Controlled = false;
            BoostOccupation(attacker);
        } else {
            CurrentOccupationPoints -= Mathf.Clamp((attacker.Actions.ObjectiveControlRating - ControlResistanceRating), 0, attacker.Actions.ObjectiveControlRating);
            if (CurrentOccupationPoints <= 0) {
                CurrentOccupationPoints = 0;
                OccupyingFaction = Conflict.Faction.None;
                ControllingFaction = Conflict.Faction.None;
                Controlled = false;

                foreach (var rend in Objective.renderers) {
                    rend.material = Objective.unclaimed_skin;
                }
            }
        }
    }


    private void SetComponents()
    {
        Objective = GetComponentInParent<Objective>();  // define before referencing!

        Attackers = new List<Actor>();
        ControllingFaction = Objective.initial_control;
        ControlResistanceRating = 2;  // TODO: specify in Inspector
        CurrentOccupationPoints = (ControllingFaction == Conflict.Faction.None) ? 0 : 100; // TODO: specify in Inspector
        Defenders = new List<Actor>();
        MaximumOccupationPoints = 100; // TODO: specify in Inspector
        OccupyingFaction = ControllingFaction;
        Controlled = false;
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