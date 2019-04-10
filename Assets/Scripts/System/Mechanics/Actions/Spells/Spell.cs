using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public enum SpellName { 
        CureWounds,
        EldritchSmite,
        RayOfFrost,
        SacredFlame,
        Sanctuary 
    };

    // Inspector settings

    public SpellName spell_name;
    public float range;
}
