using Godot;
using System;

public class FPSLabel : Label { 

    public override void _Ready() {

    }

	public override void _Process(float delta) {
        this.Text = GD.Str("fps: ", Engine.GetFramesPerSecond());
	}
}