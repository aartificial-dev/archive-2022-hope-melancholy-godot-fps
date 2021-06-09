using Godot;
using System;

[Tool]
public class Icon3D : Spatial { 

    private Texture texture;
    private float scale = 1;
    private bool shaded = false;
    private SpatialMaterial.BillboardMode billboardMode = SpatialMaterial.BillboardMode.FixedY;

    [Export]
    public Texture Icon {get => texture; set {texture = value; ChangeIcon();}}
    [Export(PropertyHint.Range, "0.01,10,0.01")]
    public float Size {get => scale; set {scale = value; ChangeIcon();}}
    [Export]
    public SpatialMaterial.BillboardMode Billboard {get => billboardMode; set {billboardMode = value; ChangeIcon();}}
    [Export]
    public bool Shaded {get => shaded; set {shaded = value; ChangeIcon();}}

    private Sprite3D sprite;

    public override void _Ready() {
        sprite = GetNode<Sprite3D>("Sprite3D");
        ChangeIcon();
    }
    
    private void ChangeIcon() {
        if (sprite is null) return;
        sprite.Texture = texture;
        sprite.Scale = new Vector3(scale, scale, scale);
        sprite.Billboard = billboardMode;
        sprite.CastShadow = shaded ? GeometryInstance.ShadowCastingSetting.DoubleSided : GeometryInstance.ShadowCastingSetting.Off;
        sprite.Shaded = shaded;
    }
}