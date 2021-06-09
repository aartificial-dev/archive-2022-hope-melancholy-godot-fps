using Godot;
using System;

public class SwitchButtonState : SwitchButton { 
    [Export]
    protected bool autoStateChange = true;
    [Export]
    public bool StartState {set {startState = value; ChangeState();} get => startState;}
    protected bool startState = false;

    private SpatialMaterial material;
    private Color redCol = Color.ColorN("red");
    private Color greenCol = Color.ColorN("green");

    public override void _Ready() {
        // size = new Vector3(0.5f, 0.5f, 0.2f);
        // name = "Switch Button";

        base._Ready();

        material = (SpatialMaterial) GetNode<CSGBox>("CSGBox").Material;
        state = startState;
        ChangeState();
    }

	public override void _Process(float delta) {
        base._Process(delta);
	}

    public override void ChangeState() {
        if (material is null) material = (SpatialMaterial) GetNode<CSGBox>("CSGBox").Material;
        if (Engine.EditorHint) {
            material.AlbedoColor = startState ? greenCol : redCol;
            return;
        }
        material.AlbedoColor = state ? greenCol : redCol;
    }

    public override void Use() {
        if (GetNode<Timer>("Timer").TimeLeft != 0) return;
        if (autoStateChange) {state = !state; ChangeState(); };
        this.EmitSignal(nameof(SwitchSignal), state);
        GetNode<AnimationPlayer>("AnimationPlayer").Play("press");
        GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D").Play();
        GetNode<Timer>("Timer").Start(0.2f);
    }
}