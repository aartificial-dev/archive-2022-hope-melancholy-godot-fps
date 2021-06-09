using Godot;
using System;

public class VFXAutoStart : Particles { 
    [Export]
    private bool autoStart = false;
    [Export]
    private bool oneShot = false;

    public override void _Ready() {
        this.OneShot = oneShot;
        if (autoStart) {
            this.Emitting = true;
        }
    }
}