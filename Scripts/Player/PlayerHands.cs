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

    public enum Weapons {
        Pipe, Pistol
    }
    private Weapons currentWeapon = Weapons.Pipe;

    private int bullets = 12;

    public override void _Ready() {
        ChangeVisualLayer(this);

        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.AnimationSetNext("pipe_attack", "pipe_hold-loop");
        animationPlayer.Play("pipe_hold-loop");

        animationPlayer.AnimationSetNext("pistol_shoot", "pistol_aim-loop");
        animationPlayer.AnimationSetNext("pistol_reload", "pistol_hold-loop");

        timer = GetNode<Timer>("../../Timer");
        timer.Connect("timeout", this, nameof(TimeOutAttack));

        meleeRayCast = GetNode<RayCast>("../../MeleeRayCast");
        rangeRayCast = GetNode<RayCast>("../../RangeRayCast");

        animationPlayer.Connect("animation_changed", this, nameof(AnimationChanged));
    }

	public override void _Process(float delta) {
        switch (currentWeapon) {
            case Weapons.Pipe: 
                ProcessWeaponPipe();
            break;
            case Weapons.Pistol:
                ProcessWeaponPistol();
            break;
        }

        if (Input.IsActionJustReleased("key_change_weapon") && animationPlayer.CurrentAnimation.Contains("hold-loop")) {
            if (currentWeapon == Weapons.Pipe) {
                animationPlayer.AnimationSetNext("pipe_holster", "pistol_holster");
                animationPlayer.AnimationSetNext("pistol_holster", "pistol_hold-loop");
                animationPlayer.Play("pipe_holster");
                GetNode<AudioStreamPlayer3D>("AudioPipeDraw").Play();
            } else {
                animationPlayer.AnimationSetNext("pistol_holster", "pipe_holster");
                animationPlayer.AnimationSetNext("pipe_holster", "pipe_hold-loop");
                animationPlayer.Play("pistol_holster");
                GetNode<AudioStreamPlayer3D>("AudioPistolDraw").Play();
            }
        }
	}

    private void AnimationChanged(String oldName, String newName) {
        //  GD.Print(oldName, " ", newName);
        if (oldName == newName) return;
        if (oldName.Contains("holster") && newName.Contains("holster")) {
            animationPlayer.Play(newName, -1, -1, true);
        }
        if (newName.Contains("pistol")) {
            currentWeapon = Weapons.Pistol;
            GetNode<Spatial>("Pipe").Visible = false;
            GetNode<Spatial>("Pistol").Visible = true;
            if (newName.Contains("holster")) {
                GetNode<AudioStreamPlayer3D>("AudioPistolDraw").Play();
            }
        }
        if (newName.Contains("pipe")) {
            currentWeapon = Weapons.Pipe;
            GetNode<Spatial>("Pipe").Visible = true;
            GetNode<Spatial>("Pistol").Visible = false;
            if (newName.Contains("holster")) {
                GetNode<AudioStreamPlayer3D>("AudioPipeDraw").Play();
            }
        }
    }

    private void ProcessWeaponPipe() {
        if (Input.IsActionJustPressed("key_attack") && animationPlayer.CurrentAnimation == "pipe_hold-loop") {
            animationPlayer.Play("pipe_attack");
            GetNode<AudioStreamPlayer3D>("AudioPipeAttack").Play();
            timer.Start(0.2f);
        }
    }

    private void ProcessWeaponPistol() {
        if (Input.IsActionPressed("key_aim") && animationPlayer.CurrentAnimation == "pistol_hold-loop") {
            animationPlayer.AnimationSetNext("pistol_aim", "pistol_aim-loop");
            animationPlayer.Play("pistol_aim");
            GetNode<AudioStreamPlayer3D>("AudioPistolAim").Play();
        }
        if (!Input.IsActionPressed("key_aim") && animationPlayer.CurrentAnimation == "pistol_aim-loop") {
            animationPlayer.AnimationSetNext("pistol_aim", "pistol_hold-loop");
            animationPlayer.Play("pistol_aim", -1, -1, true);
            GetNode<AudioStreamPlayer3D>("AudioPistolAim").Play();
        }

        if (Input.IsActionJustPressed("key_reload") && animationPlayer.CurrentAnimation == "pistol_hold-loop") {
            animationPlayer.Play("pistol_reload");
            GetNode<AudioStreamPlayer3D>("AudioPistolReload").Play();
            bullets = 12;
        }

        if (Input.IsActionJustPressed("key_attack") && animationPlayer.CurrentAnimation == "pistol_aim-loop") {
            if (bullets > 0) {
                bullets--;
                animationPlayer.Play("pistol_shoot");
                ProjectileBullet projBullet = projBulletScene.Instance<ProjectileBullet>();
                Spatial level = (Spatial) Helper.GetLevel();
                PlayerCamera camera = Helper.GetCamera();
                level.AddChild(projBullet);
                projBullet.Translation = camera.GlobalTransform.origin;
                projBullet.Rotation = camera.GlobalTransform.basis.GetEuler();
                // projBullet.ApplyCentralImpulse(-camera.GlobalTransform.basis.z * 20f);
                projBullet.relativeVector = -camera.GlobalTransform.basis.z;
                GetNode<AudioStreamPlayer3D>("AudioPistolShoot").Play();
            } else {
                GetNode<AudioStreamPlayer3D>("AudioPistolEmpty").Play();
            }
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
            GetNode<AudioStreamPlayer3D>("AudioPipeHit").Play();
            GetNode<AudioStreamPlayer3D>("AudioPipeAttack").Stop();
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