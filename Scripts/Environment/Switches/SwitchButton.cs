using Godot;
using System;

public class SwitchButton : SwitchBase { 

    public override void _Ready() {
        // size = new Vector3(0.5f, 0.5f, 0.2f);
        // name = "Button";
        
        base._Ready();
    }

	public override void _Process(float delta) {
        base._Process(delta);
	}

    public override void Use() {
        if (GetNode<Timer>("Timer").TimeLeft != 0) return;
        this.EmitSignal(nameof(SwitchSignal), true);
        GetNode<AnimationPlayer>("AnimationPlayer").Play("press");
        GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D").Play();
        GetNode<Timer>("Timer").Start(0.2f);
    }
}