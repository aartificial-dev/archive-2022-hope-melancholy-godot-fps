using Godot;
using System;

[Tool]
public class Decal : Spatial { 
    [Export]
    private Texture decal;
    public Texture DecalTexture { set {decal = value; Update();} get => decal; }
    [Export]
    private Texture normal;
    public Texture NormalMap { set {normal = value; Update();} get => normal; }
    [Export]
    private bool update = false;

    private ShaderMaterial decalShader;
    private Timer timer;

    public override void _Ready() {
        if (decalShader is null) GetMaterial();

        if ( ! (decal is null) ) {
            decalShader.SetShaderParam("albedo", decal);
        }
        if ( ! (normal is null) ) {
            decalShader.SetShaderParam("normal_map", normal);
        }

        timer = GetNode<Timer>("Timer");
        timer.Connect("timeout", this, nameof(Destroy));
    }

    public override void _Process(float delta) {
        if (update) {
            Update();
        }
    }

	public void Destroy() {
        this.GetParent().RemoveChild(this);
        this.QueueFree();
    }

    public void SetTimer(float time) {
        timer.Start(time);
    }

    private void GetMaterial() {
        CSGBox box = GetNode<CSGBox>("CSGBox");
        decalShader = (ShaderMaterial) box.Material;
    }

    public void Update() {
        if (decalShader is null) GetMaterial();

        if ( ! (decal is null) ) {
            decalShader.SetShaderParam("albedo", decal);
        }
        if ( ! (normal is null) ) {
            decalShader.SetShaderParam("normal_map", normal);
        }
    }
}