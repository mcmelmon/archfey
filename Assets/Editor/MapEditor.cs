using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Map))]
public class MapEditor : Editor {

    public override void OnInspectorGUI()
    {
        Map map = (Map)target;

        DrawDefaultInspector();

        if (GUILayout.Button ("Generate")) {
            foreach (Transform child in map.GetGeography().transform)
            {
                DestroyImmediate(child.gameObject);
            }
            map.GetCenter();
        }
    }
}