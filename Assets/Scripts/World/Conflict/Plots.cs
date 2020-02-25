using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plots : MonoBehaviour
{
    // properties

    public List<Plot> ActivePlots { get; set; }
    public List<Plot> InactivePlots { get; set; }

    // Unity

    private void Awake() {
        SetComponents();
    }

    // private

    private void SetComponents()
    {
        ActivePlots = new List<Plot>(GetComponentsInChildren<Plot>().Where(p => p.Active));
        InactivePlots = new List<Plot>();
    }
    
}
