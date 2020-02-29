using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class DialogNode : Node
{
    public string GUID;
    public string text = "This is dialog text.";
    public bool entry_point = false;
}
