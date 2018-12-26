using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spells : MonoBehaviour
{
    // Inspector settings
    public GameObject fey_sparkles_prefab;

    // properties

    public static Spells Instance { get; set; }


    // Unity

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one player");
            Destroy(this);
            return;
        }
        Instance = this;
    }


    // public


    public void FountainOfHealing(GameObject _caster, Actor _target, bool dispel_effect)
    {
        int mana_cost = 60;

        if (_target != null) {
            Spellcasting spellcasting = _caster.GetComponent<Spellcasting>();
            float available_mana = spellcasting.CurrentMana;

            if (available_mana > 0) {
                spellcasting.CurrentMana -= mana_cost;
                StartCoroutine(FountainOfHealingOngoing(spellcasting, _target));
                if (_caster.GetComponent<Player>() != null) {
                    CommandBarOne.Instance.mana_bar.value = spellcasting.CurrentManaPercentage();
                }
            }
        }
    }


    // private


    private IEnumerator FountainOfHealingOngoing(Spellcasting _spellcasting, Actor _target)
    {
        int ticks = 0;
        GameObject fountain = Instantiate(fey_sparkles_prefab, Mouse.SelectedObject.transform.position, Mouse.SelectedObject.transform.rotation);
        fountain.transform.position += new Vector3(0, 2, 0);
        fountain.transform.parent = Mouse.SelectedObject.transform;
        _target.AppliedUIEffects.Add(fountain);

        while (ticks < 5) {
            _target.Health.RecoverHealth(Mathf.RoundToInt(10 * _spellcasting.spell_potency));
            ticks++;
            yield return new WaitForSeconds(Turn.action_threshold);
        }

        _target.AppliedUIEffects.Remove(fountain);
        Destroy(fountain);
        StopCoroutine(FountainOfHealingOngoing(_spellcasting, _target));
    }
}
