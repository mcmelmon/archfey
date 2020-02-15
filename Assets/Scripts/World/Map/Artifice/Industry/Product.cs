using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : MonoBehaviour
{
    // Inspector settings
    [SerializeField] string product;
    [SerializeField] Resource required_material;
    [SerializeField] int required_material_amount;
    [SerializeField] Proficiencies.Tool required_tool;
}
