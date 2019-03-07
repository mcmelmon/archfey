using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Route))]
public class RouteEditor : Editor
{

    Route route;

    float preview_position = 0;

    private void OnEnable()
    {
        preview_position = 0;
        route = target as Route;

        if (!EditorApplication.isPlayingOrWillChangePlaymode)
            RoutePreview.CreateNewPreview(route);
    }

    private void OnDisable()
    {
        RoutePreview.DestroyPreview();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        Route.RouteType route_type = (Route.RouteType)EditorGUILayout.EnumPopup("Route Type", route.route_type);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Changed route type");
            route.route_type = route_type;
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUI.BeginChangeCheck();
        preview_position = EditorGUILayout.Slider("Preview position", preview_position, 0.0f, 1.0f);
        if (EditorGUI.EndChangeCheck())
        {
            MovePreview();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (GUILayout.Button("Add Stop"))
        {
            Undo.RecordObject(target, "added stop");


            Vector3 stop = route.local_stops[route.local_stops.Length - 1] + Vector3.right;

            ArrayUtility.Add(ref route.local_stops, stop);
            ArrayUtility.Add(ref route.wait_times, 0);
        }

        EditorGUIUtility.labelWidth = 64;
        int delete = -1;
        for (int i = 0; i < route.local_stops.Length; ++i)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginVertical();
            int size = 64;
            EditorGUILayout.BeginVertical(GUILayout.Width(size));
            EditorGUILayout.LabelField("Stop " + i, GUILayout.Width(size));
            if (i != 0 && GUILayout.Button("Delete", GUILayout.Width(size)))
            {
                delete = i;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            Vector3 new_stop;
            if (i == 0)
                new_stop = route.local_stops[i];
            else
                new_stop = EditorGUILayout.Vector3Field("Stop", route.local_stops[i]);
            int new_time = EditorGUILayout.IntField("Wait Time", route.wait_times[i]);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "changed time or position");
                route.wait_times[i] = new_time;
                route.local_stops[i] = new_stop;
            }
        }
        EditorGUIUtility.labelWidth = 0;

        if (delete != -1)
        {
            Undo.RecordObject(target, "Removed stop");

            ArrayUtility.RemoveAt(ref route.local_stops, delete);
            ArrayUtility.RemoveAt(ref route.wait_times, delete);
        }
    }


    private void OnSceneGUI()
    {
        MovePreview();

        for (int i = 0; i < route.local_stops.Length; ++i)
        {
            Vector3 worldPos;
            if (Application.isPlaying)
            {
                worldPos = route.WorldStops[i];
            }
            else
            {
                worldPos = route.transform.TransformPoint(route.local_stops[i]);
            }


            Vector3 newWorld = worldPos;
            if (i != 0)
                newWorld = Handles.PositionHandle(worldPos, Quaternion.identity);

            Handles.color = Color.red;

            if (i == 0)
            {

                if (Application.isPlaying)
                {
                    Handles.DrawDottedLine(worldPos, route.WorldStops[route.WorldStops.Length - 1], 10);
                }
                else
                {
                    Handles.DrawDottedLine(worldPos, route.transform.TransformPoint(route.local_stops[route.local_stops.Length - 1]), 10);
                }
            }
            else
            {
                if (Application.isPlaying)
                {
                    Handles.DrawDottedLine(worldPos, route.WorldStops[i - 1], 10);
                }
                else
                {
                    Handles.DrawDottedLine(worldPos, route.transform.TransformPoint(route.local_stops[i - 1]), 10);
                }

                if (worldPos != newWorld)
                {
                    Undo.RecordObject(target, "moved point");
                    route.local_stops[i] = route.transform.InverseTransformPoint(newWorld);
                }
            }
        }
    }


    void MovePreview()
    {
        //compute pos from 0-1 preview pos

        if (Application.isPlaying)
            return;

        float step = 1.0f / (route.local_stops.Length - 1);

        int starting = Mathf.FloorToInt(preview_position / step);

        if (starting > route.local_stops.Length - 2)
            return;

        float localRatio = (preview_position - (step * starting)) / step;

        Vector3 localPos = Vector3.Lerp(route.local_stops[starting], route.local_stops[starting + 1], localRatio);

        RoutePreview.preview.transform.position = route.transform.TransformPoint(localPos);

        SceneView.RepaintAll();
    }
}


public class RoutePreview
{
    static public RoutePreview s_Preview = null;
    static public GameObject preview;

    static protected Route the_route;

    static RoutePreview()
    {
        Selection.selectionChanged += SelectionChanged;
    }

    static void SelectionChanged()
    {
        if (the_route != null && Selection.activeGameObject != the_route.gameObject)
        {
            DestroyPreview();
        }
    }

    static public void DestroyPreview()
    {
        if (preview == null)
            return;

        Object.DestroyImmediate(preview);
        preview = null;
        the_route = null;
    }

    static public void CreateNewPreview(Route origin)
    {
        if (preview != null)
        {
            Object.DestroyImmediate(preview);
        }

        the_route = origin;

        preview = Object.Instantiate(origin.gameObject);
        preview.hideFlags = HideFlags.DontSave;
        Route plt = preview.GetComponentInChildren<Route>();
        Object.DestroyImmediate(plt);


        Color c = new Color(0.2f, 0.2f, 0.2f, 0.4f);
        SpriteRenderer[] rends = preview.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < rends.Length; ++i)
            rends[i].color = c;
    }
}