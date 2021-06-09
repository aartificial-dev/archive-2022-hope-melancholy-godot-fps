using Godot;
using System;

[Tool]
public class LaserTripwire : IAlarmSetter { 

    private ImmediateGeometry geometry;
    private RayCast rayCast;

    private Vector3 prevCollision;

    [Export]
    public bool update = false;
    [Export]
    public bool Enabled {set {enabled = value; ChangeStatus();} get => enabled;}
    private bool enabled = true;

    public override void _Ready() {
        geometry = GetNode<ImmediateGeometry>("ImmediateGeometry");
        rayCast = GetNode<RayCast>("RayCast");
        prevCollision = Vector3.Zero;
        UpdateGeometry();
    }

	public override void _Process(float delta) {
        if (!enabled) {
            return;
        }
        if (Engine.EditorHint) {
            rayCast.ForceRaycastUpdate();
        }
        if (update) {
            update = false;
            UpdateGeometry();
        }
        if (rayCast.IsColliding()) {
            if (prevCollision != rayCast.GetCollisionPoint()) {
                UpdateGeometry();
            }
            if (rayCast.GetCollider() is Player) {
                this.EmitSignal(nameof(AlarmSignal));
            }
        } else {
            if (prevCollision != Vector3.Zero) {
                UpdateGeometry();
                prevCollision = Vector3.Zero;
            }
        }
	}

    private void ChangeStatus() {
        if (enabled) {
            UpdateGeometry();
        } else {
            geometry.Clear();
        }
    }

    private void UpdateGeometry() {
        geometry.Clear();
        rayCast.ForceRaycastUpdate();
        Vector3 offset = new Vector3(0f, 0f, -10f);
        if (rayCast.IsColliding()) {
            Vector3 collisionPoint = rayCast.GetCollisionPoint();
            prevCollision = collisionPoint;
            offset = this.GlobalTransform.origin - collisionPoint;
            offset = offset.Rotated(Vector3.Up, this.Rotation.y);
        }

        geometry.Begin(Mesh.PrimitiveType.Lines);

        geometry.AddVertex(new Vector3(0f, 0f, 0f));
        geometry.AddVertex(offset);

        geometry.End();
    }
}