using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainOfHealing : MonoBehaviour
{
    // Inspector settings
    public GameObject fountain_of_healing_prefab;

    // properties

    public Abilities Abilities { get; set; }
    public int ManaCost { get; set; }
    public int PrimaryHealthGainPerTurn { get; set; }
    public int SecondaryHealthGainPerTurn { get; set; }
    public int Turns { get; set; }
    public Dictionary<Actor, int> TurnsActive { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }


    // public


    public void Cast(Actor _target, bool dispel_effect)
    {
        if (_target != null) {
            if (Abilities.CurrentMana >= ManaCost) {
                Abilities.CurrentMana -= ManaCost;
                CommandBarOne.Instance.mana_bar.value = Abilities.CurrentManaPercentage();
                StartCoroutine(OverTime(_target));
            }
        }
    }


    // private


    private IEnumerator OverTime(Actor _target)
    {
        GameObject fountain = Instantiate(fountain_of_healing_prefab, _target.transform.position, _target.transform.rotation);
        fountain.transform.position += new Vector3(0, 3, 0);
        fountain.transform.parent = _target.transform;
        TurnsActive[_target] = 0;

        while (TurnsActive[_target] < Turns) {
            _target.Health.RecoverHealth(Mathf.RoundToInt(PrimaryHealthGainPerTurn * Abilities.magic_potency));
            foreach (var friend in _target.IdentifyFriends()) {
                friend.Health.RecoverHealth(Mathf.RoundToInt(SecondaryHealthGainPerTurn * Abilities.magic_potency));
            }
            TurnsActive[_target]++;
            yield return new WaitForSeconds(Turn.action_threshold);
        }

        TurnsActive.Remove(_target);
        Destroy(fountain);
        StopCoroutine(OverTime(_target));
    }


    private void SetComponents()
    {
        Abilities = GetComponent<Abilities>();
        ManaCost = 100;
        PrimaryHealthGainPerTurn = 10;
        SecondaryHealthGainPerTurn = 5;
        Turns = 5;
        TurnsActive = new Dictionary<Actor, int>();
    }
}
