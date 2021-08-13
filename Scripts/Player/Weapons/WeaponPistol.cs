using Godot;
using System;

[Tool]
public class WeaponPistol : Spatial { 
    private Vector2 cockMaxPos = new Vector2(0f, -0.2f);
    
    [Export(PropertyHint.Range,"0,0.2")]
    public float cockPos = 0f;
    [Export]
    public bool muzzleFlashVisible = false;
    [Export]
    public bool muzzleFlashRandomize = false;

    public MeshInstance cockMesh;
    public MeshInstance muzzleFlash;

    private RandomNumberGenerator rng = new RandomNumberGenerator();

    public override void _Ready() {
        cockMesh = GetNode<MeshInstance>("Cock");
        muzzleFlash = GetNode<MeshInstance>("MuzzleFlash");

        rng.Randomize();

    }

	public override void _Process(float delta) {
        cockMesh.Translation = new Vector3(0f, 0f, Mathf.Clamp(-cockPos, cockMaxPos.y, cockMaxPos.x));

        muzzleFlash.Visible = muzzleFlashVisible;
        if (muzzleFlashRandomize) {
            muzzleFlashRandomize = false;
            muzzleFlash.RotationDegrees = new Vector3(0f, -180f, rng.RandfRange(0f, 360f));
        }
	}
}