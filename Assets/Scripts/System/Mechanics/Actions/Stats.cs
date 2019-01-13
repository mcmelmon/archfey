using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public Actor Me { get; set; }

    public int CharismaProficiency { get; set; }
    public int DexterityProficiency { get; set; }
    public int ConstitutionProficiency { get; set; }
    public int IntelligenceProficiency { get; set; }
    public int StrengthProficiency { get; set; }
    public int WisdomProficiency { get; set; }

    public int ArmorClass { get; set; }
    public int DefenseRating { get; set; }
    public Dictionary<Weapon.DamageType, int> Resistances { get; set; }


    // Unity


    private void Awake()
    {
        SetComponents();
    }

    private void OnValidate()
    {
        if (ArmorClass > 30) ArmorClass = 30;
        if (ArmorClass < 1) ArmorClass = 1;

        if (CharismaProficiency > 10) CharismaProficiency = 10;
        if (CharismaProficiency < 0) CharismaProficiency = 0;

        if (DexterityProficiency > 10) DexterityProficiency = 10;
        if (DexterityProficiency < 0) DexterityProficiency = 0;

        if (ConstitutionProficiency > 10) ConstitutionProficiency = 10;
        if (ConstitutionProficiency < 0) ConstitutionProficiency = 0;

        if (IntelligenceProficiency > 10) IntelligenceProficiency = 10;
        if (IntelligenceProficiency < 0) IntelligenceProficiency = 0;

        if (StrengthProficiency > 10) StrengthProficiency = 10;
        if (StrengthProficiency < 0) StrengthProficiency = 0;

        if (WisdomProficiency > 10) WisdomProficiency = 10;
        if (WisdomProficiency < 0) WisdomProficiency = 0;
    }


    // public


    public int DamageAfterDefenses(int _damage, Weapon.DamageType _type)
    {
        return DamageAfterResistance(_damage, _type);
    }


    public void SetResistances(Dictionary<Weapon.DamageType, int> _resistances)
    {
        Resistances = _resistances;
    }


    // private


    private int DamageAfterResistance(int _damage, Weapon.DamageType _type)
    {
        return (_damage <= 0 || Resistances == null) ? _damage : (_damage -= _damage * (Resistances[_type] / 100));
    }


    private void SetComponents()
    {
        Me = GetComponentInParent<Actor>();

        DefenseRating = ArmorClass + Me.Stats.DexterityProficiency;
    }
}
