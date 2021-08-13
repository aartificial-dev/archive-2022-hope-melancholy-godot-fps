using Godot;
using System;

public class ShootingTarget : BreakableBase { 
    
    private bool isHit = false;
    private Spatial spatial;
    private Timer timer;

    public override void _Ready() {
        spatial = GetNode<Spatial>("Spatial");
        timer = GetNode<Timer>("Timer");
        timer.Connect("timeout", this, nameof(Recover));
    }

	public override void _Process(float delta) {
        if (isHit && timer.TimeLeft == 0f) {
            spatial.Rotation = spatial.Rotation.LinearInterpolate(new Vector3(0f, Mathf.Deg2Rad(90f), 0f), delta * 10f);
            if (spatial.RotationDegrees.y >= 85f) {
                timer.Start(0.5f);
            }
        }
        if (!isHit && spatial.RotationDegrees.y > 0) {
            spatial.Rotation = spatial.Rotation.LinearInterpolate(new Vector3(0f, 0f, 0f), delta * 2f);
        }
	}

    public void Recover() {
        isHit = false;
    }

    public override void Hit() {
        if (isHit || spatial.RotationDegrees.y > 5f) return;
        isHit = true;
        GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D").Play();
    }
}