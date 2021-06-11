using Godot;
using System;

public class PlayerHands : Spatial { 

    private AnimationPlayer animationPlayer;
    private Timer timer;
    private RayCast meleeRayCast;
    private RayCast rangeRayCast;

    private PackedScene vfx_concrete = ResourceLoader.Load<PackedScene>("res://Scenes/VFX/VFX_concrete_impact.tscn");
    private PackedScene prototypeBallScene = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/PrototypeBall.tscn");
    private PackedScene projBulletScene = ResourceLoader.Load<PackedScene>("res://Scenes/Projectiles/ProjectileBullet.tscn");

    public override void _Ready() {
        ChangeVisualLayer(this);

        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.AnimationSetNext("pipe_attack", "pipe_hold-loop");
        animationPlayer.Play("pipe_hold-loop");

        timer = GetNode<Timer>("../../Timer");
        timer.Connect("timeout", this, nameof(TimeOutAttack));

        meleeRayCast = GetNode<RayCast>("../../MeleeRayCast");
        rangeRayCast = GetNode<RayCast>("../../RangeRayCast");
    }

	public override void _Process(float delta) {
        if (Input.IsActionJustPressed("key_attack") && animationPlayer.CurrentAnimation == "pipe_hold-loop") {
            animationPlayer.Play("pipe_attack");
            GetNode<AudioStreamPlayer3D>("AudioAttack").Play();
            timer.Start(0.45f);
        }

        if (Input.IsActionJustPressed("key_aim")) {
            ProjectileBullet projBullet = projBulletScene.Instance<ProjectileBullet>();
            Spatial level = (Spatial) Helper.GetLevel();
            PlayerCamera camera = Helper.GetCamera();
            level.AddChild(projBullet);
            projBullet.Translation = camera.GlobalTransform.origin;
            projBullet.Rotation = camera.GlobalTransform.basis.GetEuler();
            // projBullet.ApplyCentralImpulse(-camera.GlobalTransform.basis.z * 20f);
            projBullet.relativeVector = -camera.GlobalTransform.basis.z;

            GetNode<AudioStreamPlayer3D>("AudioPistolShoot").Play();
        }
	}

    private void ChangeVisualLayer(Node parent) {
        Godot.Collections.Array childs = parent.GetChildren();
        
        foreach (Node child in childs) {
            if (child is MeshInstance mesh) {
                mesh.SetLayerMaskBit(0, false);
                mesh.SetLayerMaskBit(19, true);
                mesh.CastShadow = GeometryInstance.ShadowCastingSetting.Off;
            }
            ChangeVisualLayer(child);
        }
    }

    private void TimeOutAttack() {
        AttackMeleeTube();
    }

    private void AttackMeleeTube() {
        meleeRayCast.ForceRaycastUpdate();
        if (meleeRayCast.IsColliding()) {
            GetNode<AudioStreamPlayer3D>("AudioHit").Play();
            Spatial part = vfx_concrete.Instance<Spatial>();
            Vector3 pos = meleeRayCast.GetCollisionPoint();
            Spatial level = (Spatial) Helper.GetLevel();
            level.AddChild(part);
            Transform tr = part.GlobalTransform;
            tr.origin = pos;
            part.GlobalTransform = tr;
        }
    }
}