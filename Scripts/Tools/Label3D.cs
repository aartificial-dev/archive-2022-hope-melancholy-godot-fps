using Godot;
using System;

[Tool]
public class Label3D : Spatial { 

    private const float unitSize = 128f;

    private String text = "";
    private SpatialMaterial.BillboardMode billboardMode = SpatialMaterial.BillboardMode.Disabled;
    private Vector2 size = new Vector2(1f, 1f);
    private float textScale = 1f;
    private Font font;
    private Color textColor = Color.ColorN("white");
    private Color outlineColor = Color.ColorN("white");
    private bool hasOutline = false;
    private float outlineSize = 1f;
    private bool unshaded = true;

    [Export(PropertyHint.MultilineText)]
    public String Text { get => text; set {text = value; ChangeText();} }
    [Export]
    public SpatialMaterial.BillboardMode BillboardMode { get => billboardMode; set {billboardMode = value; ChangeMaterial();} }
    [Export]
    public Vector2 Size { get => size; set {size = value; ChangeSize();} }
    [Export(PropertyHint.Range, "0.01,10,0.01")]
    public float TextScale { get => textScale; set {textScale = value; ChangeSize();} }
    [Export]
    public Font Font { get => font; set {font = value; ChangeLabel();} }
    [Export]
    public Color TextColor { get => textColor; set {textColor = value; ChangeLabel();} }
    [Export]
    public Color OutlineColor { get => outlineColor; set {outlineColor = value; ChangeLabel();} }
    [Export]
    public bool HasOutline { get => hasOutline; set {hasOutline = value; ChangeLabel();} }
    [Export(PropertyHint.Range, "0.01,20,0.01")]
    public float OutlineSize { get => outlineSize; set {outlineSize = value; ChangeLabel();} }
    [Export]
    public bool Unshaded { get => unshaded; set {unshaded = value; ChangeMaterial();} }
    [Export]
    public bool editorOnly = true;

    private Viewport viewport;
    private QuadMesh mesh;
    private Control control;
    private Label label;
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
        label = GetNode<Label>("Viewport/Control/Label");

        ChangeLabel();
        ChangeMaterial();
        ChangeSize();
        ChangeText();
    }

    private void ChangeSize() {
        if (mesh is null || viewport is null || control is null) return;
        mesh.Size = size;
        viewport.Size = (size * unitSize) / textScale;
        control.RectSize = (size * unitSize) / textScale;
        
        viewport.UpdateWorlds();
    }

    private void ChangeText() {
        if (label is null) return;
        label.Text = text;
        
        viewport.UpdateWorlds();
    }

    private void ChangeLabel() {
        if (label is null) return;
        label.Set("custom_fonts/font", font);
        label.Set("custom_colors/font_color", textColor);
        label.Set("custom_colors/font_color_shadow", outlineColor);
        
        label.Set("custom_constants/shadow_offset_x", hasOutline ? outlineSize : 0f);
        label.Set("custom_constants/shadow_offset_y", hasOutline ? outlineSize : 0f);
        label.Set("custom_constants/shadow_as_outline", hasOutline ? 1f : 0f);

        viewport.UpdateWorlds();
    }

    private void ChangeMaterial() {
        if (material is null) return;
        material.ParamsBillboardMode = billboardMode;
        material.FlagsUnshaded = unshaded;

        viewport.UpdateWorlds();
    }
}