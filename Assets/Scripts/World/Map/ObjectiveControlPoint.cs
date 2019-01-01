using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveControlPoint : MonoBehaviour
{
    // Inspector settings

    public Slider control_indicator;

    // properties

    public Actor Contender { get; set; }
    public float ControlResistanceRating { get; set; }
    public float CurrentResistancePoints { get; set; }
    public Conflict.Faction Faction { get; set; }
    public GameObject Marker { get; set; }
    public float MaximumResistancePoints { get; set; }
    public Actor NearestActor { get; set; }
    public Objective Objective { get; set; }
    public bool Occupied { get; set; }
    public Actor Occupier { get; set; }


    // Unity


    private void Start()
    {
        SetComponents();
        UpdateControlIndicator();
        StartCoroutine(CheckOccupation());
        StartCoroutine(FindNearestActor());
        StartCoroutine(ControlIndicatorFaceCamera());
    }


    // public


    public Actor ConfirmOccupation()
    {
        if (Occupier != NearestActor)
        {
            if (Occupier.Ghaddim == NearestActor.Ghaddim && Occupier.Mhoddim == NearestActor.Mhoddim)
            {
                Occupier.ObjectiveControlPoint = null;
                Occupier = NearestActor;
            }
        }
        return Occupier;
    }


    // private


    private IEnumerator CheckOccupation()
    {
        while (true) {
            if (NearestActor == null) { // everyone is dead
                ResetControl();
            } else {
                float nearest_actor_distance = Vector3.Distance(NearestActor.transform.position, transform.position);

                if (ChallengeForContention(nearest_actor_distance) || ContenderEliminated() || OccupierAbandonedControl(nearest_actor_distance)) {
                    ReduceControl();
                } else {
                    if (NewContender(nearest_actor_distance)) {
                        Contender = NearestActor;
                        BoostControl();
                    }
                }
            }

            UpdateControlIndicator();
            yield return new WaitForSeconds(Turn.action_threshold);
        }
    }


    private void BoostControl()
    {
        if (CurrentResistancePoints <= 0) return;

        CurrentResistancePoints -= Mathf.Clamp((Contender.ObjectiveControlRating - ControlResistanceRating), 0, Contender.ObjectiveControlRating);
        if (CurrentResistancePoints <= 0) {
            CurrentResistancePoints = 0;
            Occupied = true;
            Occupier = NearestActor;
            Contender = null;
            Faction = NearestActor.GetComponent<Actor>().Faction;
            foreach (var rend in Objective.renderers) {
                rend.material = (Faction == Conflict.Faction.Ghaddim) ? Objective.ghaddim_skin : Objective.mhoddim_skin;
            }
        }
    }


    private bool ChallengeForContention(float nearest_actor_distance)
    {
        return Contender != null && Contender != NearestActor && Contender.Faction != NearestActor.Faction && nearest_actor_distance < Route.reached_threshold;
    }


    private bool ContenderEliminated()
    {
        return Occupier == null && Contender == null && CurrentResistancePoints < MaximumResistancePoints - 1;
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
        return (float)CurrentResistancePoints / (float)MaximumResistancePoints;
    }


    private IEnumerator FindNearestActor()
    {
        while (true)
        {
            yield return new WaitForSeconds(Turn.action_threshold);

            float shortest_distance = float.MaxValue;
            float distance;
            GameObject nearest_actor = null;

            for (int i = 0; i < Conflict.Units.Count; i++)
            {
                GameObject _unit = Conflict.Units[i];
                if (_unit == null || transform == null) continue;

                distance = Vector3.Distance(transform.position, _unit.transform.position);
                if (distance < shortest_distance)
                {
                    shortest_distance = distance;
                    nearest_actor = _unit;
                }
            }

            if (nearest_actor != null)
                NearestActor = nearest_actor.GetComponent<Actor>();
        }
    }


    private bool NewContender(float nearest_actor_distance)
    {
        return !Occupied && nearest_actor_distance < Route.reached_threshold;
    }


    private bool OccupierAbandonedControl(float nearest_actor_distance)
    {
        return (Occupied && Occupier == null) || (Occupier != null && nearest_actor_distance > Route.reached_threshold);
    }


    private void ReduceControl()
    {
        if (CurrentResistancePoints >= MaximumResistancePoints) return;

        CurrentResistancePoints += ControlResistanceRating;
        if (CurrentResistancePoints >= MaximumResistancePoints) {
            CurrentResistancePoints = MaximumResistancePoints;
            Occupied = false;
            if (Occupier != null) Occupier.GetComponent<Actor>().ObjectiveControlPoint = null;
            Occupier = null;
            Faction = Conflict.Faction.None;
            foreach (var rend in Objective.renderers) {
                rend.material = Objective.unclaimed_skin;
            }
        }
    }


    private void ResetControl()
    {
        Occupied = false;
        Occupier = null;
        Faction = Conflict.Faction.None;
        CurrentResistancePoints = MaximumResistancePoints;
    }


    private void SetComponents()
    {
        ControlResistanceRating = Random.Range(1, 6);
        CurrentResistancePoints = MaximumResistancePoints = 100 + Random.Range(0, 8);
        Faction = Conflict.Faction.None;
        Objective = GetComponentInParent<Objective>();
        Occupied = false;
    }


    public void UpdateControlIndicator()
    {
        control_indicator.value = CurrentControlPercentage();
        if (control_indicator.value >= 1) {
            control_indicator.gameObject.SetActive(false);
        } else {
            control_indicator.gameObject.SetActive(true);
        }
    }
}