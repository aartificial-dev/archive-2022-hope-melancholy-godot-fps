using Godot;
using System;

public class SwitchablePath : PathFollow { 
    [Export]
    public NodePath switchPath = "";

    private SwitchBase switchNode;

    public override void _Ready() {
        if (GetNode(switchPath) is SwitchBase sw) {
            switchNode = sw;
        }
    }

	public override void _Process(float delta) {
        if (switchNode.State) {
            this.UnitOffset = Mathf.Lerp(this.UnitOffset, 1f, delta * 5f);
        } else {
            this.UnitOffset = Mathf.Lerp(this.UnitOffset, 0f, delta * 5f);
        }
	}
}