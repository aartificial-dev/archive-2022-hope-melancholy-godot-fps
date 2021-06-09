using Godot;
using System;

public class MovingPlatformPath : Path { 

    [Export(PropertyHint.Range, "0.001,10")]
    private float platformSpeed = 1f;

    private bool isForward = true;

    private PathFollow pathFollow;

    public override void _Ready() {
        pathFollow = GetNode<PathFollow>("PathFollow");
    }

	public override void _Process(float delta) {
        if (isForward) {
            pathFollow.UnitOffset += platformSpeed * delta;
            if (pathFollow.UnitOffset >= 1.0f) {
                isForward = false;
            }
        } else {
            pathFollow.UnitOffset -= platformSpeed * delta;
            if (pathFollow.UnitOffset <= 0f) {
                isForward = true;
            }
        }
	}
}