using Godot;
using System;

public class PrototypeBall : RigidBody { 

    private bool isFinished = false;

    public override void _Ready() {
        this.Connect("body_entered", this, nameof(BodyEntered));
    }

    private void BodyEntered(Node body) {
        if (isFinished) return;
        if (body is BreakableGlass glass) {
            isFinished = true;
            RayCast ray = GetNode<RayCast>("RayCast");
            ray.ForceRaycastUpdate();
            glass.Shatter(ray.GetCollisionPoint(), this.LinearVelocity, ray.GetCollisionNormal());
        }
    }
}