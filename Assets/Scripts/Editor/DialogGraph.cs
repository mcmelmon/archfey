using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class DialogGraph : EditorWindow
{
    private DialogGraphView graph_view;

    [MenuItem("Graph/Dialog Graph")]
    public static void OpenDialogGraphWindow()
    {
        var window = GetWindow<DialogGraph>();
        window.titleContent = new GUIContent(text: "Dialog Graph");
    }

    private void OnEnable() {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void OnDisable() {
        rootVisualElement.Remove(graph_view);
    }

    private void ConstructGraphView()
    {
        graph_view = new DialogGraphView {
            name = "Dialog Graph View"
        };

        graph_view.StretchToParentSize();
        rootVisualElement.Add(graph_view);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();

        var create_button = new Button(clickEvent: () => {
            graph_view.CreateNode("Dialog Node");
        });

        create_button.text = "Create Node";
        toolbar.Add(create_button);
        rootVisualElement.Add(toolbar);
    }
}
