using System;
using Godot;

public abstract class SwitchBase : InteractiveBase {
    [Signal]
    public delegate void SwitchSignal(bool state);  

    protected bool state;
    public bool State {set { state = value; ChangeState(); } get => state;}


    public virtual void ChangeState() {}

    public override void _Ready() {
        base._Ready();
    }

	public override void _Process(float delta) {
        base._Process(delta);
	}
}
