using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruin : MonoBehaviour
{

    public Material ghaddim_skin;
    public Material mhoddim_skin;
    public Material unclaimed_skin;

    // properties

    public static float MinimumRuinSpacing { get; set; }

    // static


    public static Ruin InstantiateRuin(Ruin prefab, Vector3 point, Ruins _ruins)
    {
        Ruin _ruin = Instantiate(prefab, point, _ruins.transform.rotation, _ruins.transform);
        _ruin.transform.localScale += new Vector3(4, 16, 4);
        _ruin.transform.position += new Vector3(0, _ruin.transform.localScale.y / 2, 0);
        _ruin.SetComponents();

        return _ruin;
    }

    // private


    private void SetComponents()
    {
        MinimumRuinSpacing = 20f;
    }
}
