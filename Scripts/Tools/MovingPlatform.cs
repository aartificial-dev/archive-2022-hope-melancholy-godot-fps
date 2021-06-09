using Godot;
using System;

public class MovingPlatform : CSGBox { 

    private Vector3 velocity = Vector3.Zero;
    public Vector3 Velocity {get => velocity;}
    private float torque = 0f;
    public float Torque {get => torque;}

    private Vector3 prevPosition;
    private float prevRotation;

    public override void _Ready() {
        prevPosition = this.GlobalTransform.origin;
        prevRotation = new Vector2(this.GlobalTransform.basis.z.x, this.GlobalTransform.basis.z.z).AngleTo(new Vector2(0f, -1f));
    }

	public override void _PhysicsProcess(float delta) {
        velocity = this.GlobalTransform.origin - prevPosition;
        torque = new Vector2(this.GlobalTransform.basis.z.x, this.GlobalTransform.basis.z.z).AngleTo(new Vector2(0f, -1f)) - prevRotation;

        prevPosition = this.GlobalTransform.origin;
        prevRotation = new Vector2(this.GlobalTransform.basis.z.x, this.GlobalTransform.basis.z.z).AngleTo(new Vector2(0f, -1f));
	}
}