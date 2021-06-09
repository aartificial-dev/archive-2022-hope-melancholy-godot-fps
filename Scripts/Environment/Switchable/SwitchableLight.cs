using Godot;
using System;

public class SwitchableLight : OmniLight { 
    [Export]
    public NodePath switchPath = "";

    private SwitchBase switchNode;

    public override void _Ready() {
        if (GetNode(switchPath) is SwitchBase sw) {
            switchNode = sw;
        }
    }

	public override void _Process(float delta) {
        if (switchNode is null) return;
        this.Visible = switchNode.State;
	}
}