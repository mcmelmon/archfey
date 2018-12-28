using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainOfHealing : MonoBehaviour
{
    // Inspector settings
    public GameObject fountain_of_healing_prefab;

    // properties

    public Abilities Abilities { get; set; }
    public Dictionary<Actor, int> AffectedTargets { get; set; }
    public int ManaCost { get; set; }
    public int Turns { get; set; }


    // Unity


    private void Awake()
    {
        Abilities = GetComponent<Abilities>();
        AffectedTargets = new Dictionary<Actor, int>();
        ManaCost = 100;
        Turns = 5;
    }


    // public


    public void Cast(Actor _target, bool dispel_effect)
    {
        if (_target != null) {
            if (Abilities.CurrentMana >= ManaCost) {
                Abilities.CurrentMana -= ManaCost;
                StartCoroutine(OverTime(_target));
            }
        }
    }


    // private


    private IEnumerator OverTime(Actor _target)
    {
        GameObject fountain = Instantiate(fountain_of_healing_prefab, Mouse.SelectedObject.transform.position, Mouse.SelectedObject.transform.rotation);
        fountain.transform.position += new Vector3(0, 2, 0);
        fountain.transform.parent = _target.transform;
        AffectedTargets[_target] = 0;

        while (AffectedTargets[_target] < Turns) {
            _target.Health.RecoverHealth(Mathf.RoundToInt(10 * Abilities.magic_potency));
            foreach (var friend in _target.IdentifyFriends()) {
                friend.Health.RecoverHealth(Mathf.RoundToInt(5 * Abilities.magic_potency));
            }
            AffectedTargets[_target]++;
            yield return new WaitForSeconds(Turn.action_threshold);
        }

        AffectedTargets.Remove(_target);
        Destroy(fountain);
        StopCoroutine(OverTime(_target));
    }
}
