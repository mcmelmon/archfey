using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogGraphView : GraphView
{
    private readonly Vector2 default_node_size = new Vector2(x: 150, y: 200);

    // constructor
    public DialogGraphView()
    {
        styleSheets.Add(UnityEngine.Resources.Load<StyleSheet>("DialogGraph"));
        
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(index: 0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
    }

    // public

    public DialogNode CreateDialogNode(string _name)
    {
        var node = new DialogNode{
            title = _name,
            text = _name,
            GUID = System.Guid.NewGuid().ToString()
        };

        var input_port = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
        input_port.portName = "Input";
        node.inputContainer.Add(input_port);

        var new_branch_button = new Button(clickEvent: () => {
            AddChoicePort(node);
        });
        new_branch_button.text = "New Branch";
        node.titleButtonContainer.Add(new_branch_button);

        node.RefreshExpandedState();
        node.RefreshPorts();
        node.SetPosition(new Rect(position: Vector2.zero, default_node_size));

        return node;
    }

    public void CreateNode(string _name)
    {
        AddElement(CreateDialogNode(_name));
    }

        private DialogNode GenerateEntryPointNode()
    {
        var node = new DialogNode {
            title = "Start Dialog",
            GUID = System.Guid.NewGuid().ToString(),
            text = "This is the first line of dialog",
            entry_point = true
        };

        var generated_port = GeneratePort(node, Direction.Output);
        generated_port.portName = "Next";
        node.outputContainer.Add(generated_port);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(x: 100, y: 200, width: 100, height: 150));

        return node;
    }

    public override List<Port> GetCompatiblePorts(Port _start, NodeAdapter _adapter)
    {
        List<Port> compatible_ports = new List<Port>();

        ports.ForEach(port => {
            if (_start != port && _start.node != port.node) compatible_ports.Add(port);
        });

        return compatible_ports;
    }

    // private

    private void AddChoicePort(DialogNode _node)
    {
        Port port = GeneratePort(_node, Direction.Output);
        int output_port_count = _node.outputContainer.Query(name: "connector").ToList().Count;
        string output_port_name = $"Choice {output_port_count}";

        _node.outputContainer.Add(port);
        _node.RefreshPorts();
        _node.RefreshExpandedState();
    }

    private Port GeneratePort(DialogNode _node, Direction _direction, Port.Capacity _capacity = Port.Capacity.Single)
    {
        return _node.InstantiatePort(Orientation.Horizontal, _direction, _capacity, typeof(float));  // final argument is for passing data
    }
}
