using Godot;
using System;

public class BreakableGlass : CSGBox { 

    private AudioStreamPlayer3D audioStreamPlayer3D;
    private AudioStreamSample audioGlassShatter = ResourceLoader.Load<AudioStreamSample>("res://Assets/Sounds/Environment/glass_shatter.wav");
    private ArrayMesh glassShatterPatternMesh = ResourceLoader.Load<ArrayMesh>("res://Assets/Models/Utilities/glass_shatter_pattern.obj");

    private RandomNumberGenerator rnd;

    public override void _Ready() {
        audioStreamPlayer3D = new AudioStreamPlayer3D();
        audioStreamPlayer3D.MaxDistance = 500f;
        audioStreamPlayer3D.UnitDb = 16f;
        audioStreamPlayer3D.Stream = audioGlassShatter;
        this.AddChild(audioStreamPlayer3D);

        rnd = new RandomNumberGenerator();
        rnd.Randomize();
    }

	public override void _Process(float delta) {

	}

    public void Shatter(Vector3 pos, Vector3 velocity, Vector3 normal) {
        return;
        #pragma warning disable
        Transform tr = audioStreamPlayer3D.GlobalTransform;
        tr.origin = pos;
        audioStreamPlayer3D.GlobalTransform = tr;
        audioStreamPlayer3D.Play();

        CSGMesh mesh = new CSGMesh();
        mesh.Mesh = glassShatterPatternMesh;
        this.GetParent().AddChild(mesh);
        Transform mtr = mesh.GlobalTransform;
        mtr.origin = pos;
        mesh.GlobalTransform = mtr;
        mesh.LookAt(pos + (normal == Vector3.Zero ? velocity : normal), Vector3.Up);

        mesh.UseCollision = false;
        mesh.CollisionLayer = 0;
        mesh.CollisionMask = 0;
        mesh.CastShadow = ShadowCastingSetting.Off;

        mesh.RotateObjectLocal(Vector3.Forward, rnd.RandfRange(0f, Mathf.Pi * 2f));
        #pragma warning restore
    }
}