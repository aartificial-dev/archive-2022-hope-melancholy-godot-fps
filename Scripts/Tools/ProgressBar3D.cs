using Godot;
using System;

[Tool]
public class ProgressBar3D : Spatial { 

    private const float unitSize = 128f;

    private float minValue = 0f;
    private float maxValue = 100f;
    private float value = 100f;
    private bool rounded = false;
    private SpatialMaterial.BillboardMode billboardMode = SpatialMaterial.BillboardMode.Disabled;
    private Vector2 size = new Vector2(1f, 1f);
    private StyleBox fgStyle;
    private StyleBox bgStyle;
    private bool unshaded = true;

    [Export]
    public float Value { get => value; set {this.value = value; ChangeValue();} }
    [Export]
    public float MinValue { get => minValue; set {this.minValue = value; ChangeValue();} }
    [Export]
    public float MaxValue { get => maxValue; set {this.maxValue = value; ChangeValue();} }
    [Export]
    public bool Rounded { get => rounded; set {this.rounded = value; ChangeValue();} }
    [Export]
    public SpatialMaterial.BillboardMode BillboardMode { get => billboardMode; set {billboardMode = value; ChangeMaterial();} }
    [Export]
    public Vector2 Size { get => size; set {size = value; ChangeSize();} }
    [Export]
    public bool Unshaded { get => unshaded; set {unshaded = value; ChangeMaterial();} }
    [Export]
    public StyleBox FGStyle { get => fgStyle; set {fgStyle = value; ChangeStyle();} }
    [Export]
    public StyleBox BGStyle { get => bgStyle; set {bgStyle = value; ChangeStyle();} }
    [Export]
    public bool editorOnly = true;

    private Viewport viewport;
    private QuadMesh mesh;
    private Control control;
    private ProgressBar progressBar;
    private SpatialMaterial material;

    public enum BillboardModeEnum {
        Disabled, Enabled, YBillboard
    }

    public override void _Ready() {
        if (editorOnly && !Engine.EditorHint) {
            this.Visible = false;
            this.GetParent().RemoveChild(this);
            this.QueueFree();
            return;
        }
        viewport = GetNode<Viewport>("Viewport");
        mesh = (QuadMesh) GetNode<CSGMesh>("CSGMesh").Mesh;
        material = (SpatialMaterial) GetNode<CSGMesh>("CSGMesh").Material;
        control = GetNode<Control>("Viewport/Control");
        progressBar = GetNode<ProgressBar>("Viewport/Control/ProgressBar");

        ChangeValue();
        ChangeStyle();
        ChangeMaterial();
        ChangeSize();
    }

    private void ChangeSize() {
        if (mesh is null || viewport is null || control is null) return;
        mesh.Size = size;
        viewport.Size = (size * unitSize);
        control.RectSize = (size * unitSize);
        
        viewport.UpdateWorlds();
    }

    private void ChangeValue() {
        if (progressBar is null) return;

        progressBar.Rounded = rounded;
        
        if (minValue > maxValue) {
            minValue = maxValue;
        }
        if (maxValue < minValue) {
            maxValue = minValue;
        }
        value = Mathf.Clamp(value, minValue, maxValue);

        progressBar.MinValue = minValue;
        progressBar.MaxValue = maxValue;
        progressBar.Value = value;

        viewport.UpdateWorlds();
    }

    private void ChangeStyle() {
        if (progressBar is null) return;
        
        progressBar.Set("custom_styles/fg", fgStyle);
        progressBar.Set("custom_styles/bg", bgStyle);

        viewport.UpdateWorlds();
    }

    private void ChangeMaterial() {
        if (material is null) return;
        material.ParamsBillboardMode = billboardMode;
        material.FlagsUnshaded = unshaded;

        viewport.UpdateWorlds();
    }
}