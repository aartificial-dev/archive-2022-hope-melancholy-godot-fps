using Godot;
using System;
using System.Reflection;

public class ProjectileBullet : KinematicBody { 

    public Vector3 rotation = Vector3.Zero;
    public float speed = 4f;
    public Vector3 relativeVector = Vector3.Forward;

    private Player player;
    private AudioPlayerOnce3D audioHit;

    private AudioStream soundHit = ResourceLoader.Load<AudioStream>("res://Assets/Sounds/Weapons/PistolGeneric/snd_generic_bullet_hit.wav");

    public override void _Ready() {
        player = Helper.GetPlayer();
        audioHit = new AudioPlayerOnce3D();
        audioHit.Stream = soundHit;
        audioHit.UnitDb = 10f;
        audioHit.MaxDistance = 100f;
    }

	public override void _Process(float delta) {
        if (this.IsQueuedForDeletion()) return;

        KinematicCollision coll = this.MoveAndCollide(relativeVector * speed, false, true);
        if (!(coll is null)) {
            this.GetParent().AddChild(audioHit);

            Transform tr = new Transform();
            tr.origin = coll.Position;
            audioHit.GlobalTransform = tr;
            audioHit.Play();

            if (coll.Collider is RigidBody rb) {
                rb.ApplyCentralImpulse(relativeVector);
            }
            if (coll.Collider is BreakableBase br) {
                // br.Hit();
                Type breakType = br.GetType();
                MethodInfo hitInfo = breakType.GetMethod("Hit");
                hitInfo.Invoke(br, new object[]{});
            }

            this.GetParent().RemoveChild(this);
            this.QueueFree();
            return;
        }

        if (player.GlobalTransform.origin.DistanceTo(this.GlobalTransform.origin) > 50f) {
            this.QueueFree();
        }
	}
}