using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainOfHealing : MonoBehaviour
{
    // properties

    public int ManaCost { get; set; }
    public int PrimaryHealthGainPerTurn { get; set; }
    public int SecondaryHealthGainPerTurn { get; set; }
    public Actor Target { get; set; }
    public int Turns { get; set; }
    public Dictionary<Actor, int> TurnsActive { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void Cast(Actor _target, bool dispel_effect = false)
    {
        if (_target != null) {
            Player.Instance.CurrentMana -= ManaCost;
            Player.Instance.UpdateManaBar();
            StartCoroutine(OverTime(_target));
        }
    }


    // private


    private IEnumerator OverTime(Actor _target)
    {
        if (_target == null) yield return null;
        Target = _target;

        GameObject fountain = Instantiate(SpellEffects.Instance.fountain_of_healing_prefab, _target.transform.position, _target.transform.rotation);
        fountain.transform.position += new Vector3(0, 3, 0);
        fountain.transform.parent = _target.transform;
        TurnsActive[_target] = 0;

        while (TurnsActive[_target] < Turns) {
            Target.Health.RecoverHealth(Mathf.RoundToInt(PrimaryHealthGainPerTurn * Player.Instance.magic_potency));
            foreach (var friend in Target.Actions.Decider.IdentifyFriends()) {
                friend.Health.RecoverHealth(Mathf.RoundToInt(SecondaryHealthGainPerTurn * Player.Instance.magic_potency));
            }
            TurnsActive[Target]++;
            yield return new WaitForSeconds(Turn.ActionThreshold);
        }

        TurnsActive.Remove(Target);
        Destroy(fountain);
    }


    private void SetComponents()
    {
        ManaCost = 100;
        PrimaryHealthGainPerTurn = 10;
        SecondaryHealthGainPerTurn = 5;
        Turns = 5;
        TurnsActive = new Dictionary<Actor, int>();
    }
}
