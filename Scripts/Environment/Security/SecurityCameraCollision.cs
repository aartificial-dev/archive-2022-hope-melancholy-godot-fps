using Godot;
using System;

public class SecurityCameraCollision : BreakableBase { 
    [Signal]
    public delegate void SignalHit();

    public override void Hit() {
        this.EmitSignal(nameof(SignalHit), new object[]{});
    }
}