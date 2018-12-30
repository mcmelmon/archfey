using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainOfHealing : MonoBehaviour
{
    // properties

    public Actor Actor { get; set; }
    public int ManaCost { get; set; }
    public int PrimaryHealthGainPerTurn { get; set; }
    public Resources Resources { get; set; }
    public int SecondaryHealthGainPerTurn { get; set; }
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
            if (Resources.CurrentMana >= ManaCost) {
                Resources.CurrentMana -= ManaCost;
                CommandBarOne.Instance.mana_bar.value = Resources.CurrentManaPercentage();
                StartCoroutine(OverTime(_target));
            }
        }
    }


    // private


    private IEnumerator OverTime(Actor _target)
    {
        GameObject fountain = Instantiate(SpellEffects.Instance.fountain_of_healing_prefab, _target.transform.position, _target.transform.rotation);
        fountain.transform.position += new Vector3(0, 3, 0);
        fountain.transform.parent = _target.transform;
        TurnsActive[_target] = 0;

        while (TurnsActive[_target] < Turns) {
            _target.Health.RecoverHealth(Mathf.RoundToInt(PrimaryHealthGainPerTurn * Resources.magic_potency));
            foreach (var friend in _target.IdentifyFriends()) {
                friend.Health.RecoverHealth(Mathf.RoundToInt(SecondaryHealthGainPerTurn * Resources.magic_potency));
            }
            TurnsActive[_target]++;
            yield return new WaitForSeconds(Turn.action_threshold);
        }

        TurnsActive.Remove(_target);
        Destroy(fountain);
    }


    private void SetComponents()
    {
        Resources = GetComponent<Resources>();
        Actor = GetComponentInParent<Actor>();
        ManaCost = 100;
        PrimaryHealthGainPerTurn = 10;
        SecondaryHealthGainPerTurn = 5;
        Turns = 5;
        TurnsActive = new Dictionary<Actor, int>();
    }
}
