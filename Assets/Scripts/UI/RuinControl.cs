using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuinControl : MonoBehaviour
{
    public static RuinControl Instance { get; set; }


    private void Awake() {
        if (Instance != null)
        {
            Debug.LogError("More than one ruin control instance");
            Destroy(this);
            return;
        }
        Instance = this;
    }


}
