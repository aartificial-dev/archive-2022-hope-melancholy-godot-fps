using Godot;
using System;

[Tool]
public class LadderCollder : Area { 
    
    private float width = 0.4f;
    private float height = 1f;
    private float depth = 0.1f;

    private BoxShape shape;
    private ImmediateGeometry geometry;

    [Export(PropertyHint.Range, "0.1,100,0.01")]
    public float Height { get => height; set { height = value; UpdateShape(); } }
    [Export(PropertyHint.Range, "0.1,100,0.01")]
    public float Width { get => width; set { width = value; UpdateShape(); } }

    private bool showBBox = true;
    [Export]
    public bool ShowBBox { set { showBBox = value; UpdateShape(); } get => showBBox; }

    public override void _Ready() {
        shape = (BoxShape) GetNode<CollisionShape>("CollisionShape").Shape;
        geometry = GetNode<ImmediateGeometry>("ImmediateGeometry");
        UpdateShape();
    }

    private void UpdateShape() {
        if (shape is null) return;
        
        geometry.Clear();
        shape.Extents = new Vector3(width, height, depth);

        if (showBBox && Engine.EditorHint) {
            geometry.Begin(Mesh.PrimitiveType.Lines);

            geometry.AddVertex(new Vector3(width, height, depth));
            geometry.AddVertex(new Vector3(-width, height, depth));

            geometry.AddVertex(new Vector3(width, height, depth));
            geometry.AddVertex(new Vector3(width, height, -depth));

            geometry.AddVertex(new Vector3(-width, height, -depth));
            geometry.AddVertex(new Vector3(width, height, -depth));

            geometry.AddVertex(new Vector3(-width, height, -depth));
            geometry.AddVertex(new Vector3(-width, height, depth));

            geometry.AddVertex(new Vector3(width, height, depth));
            geometry.AddVertex(new Vector3(width, -height, depth));

            geometry.AddVertex(new Vector3(-width, height, depth));
            geometry.AddVertex(new Vector3(-width, -height, depth));

            geometry.AddVertex(new Vector3(-width, height, -depth));
            geometry.AddVertex(new Vector3(-width, -height, -depth));
            
            geometry.AddVertex(new Vector3(width, height, -depth));
            geometry.AddVertex(new Vector3(width, -height, -depth));
            
            geometry.AddVertex(new Vector3(width, -height, depth));
            geometry.AddVertex(new Vector3(-width, -height, depth));

            geometry.AddVertex(new Vector3(width, -height, depth));
            geometry.AddVertex(new Vector3(width, -height, -depth));

            geometry.AddVertex(new Vector3(-width, -height, -depth));
            geometry.AddVertex(new Vector3(width, -height, -depth));

            geometry.AddVertex(new Vector3(-width, -height, -depth));
            geometry.AddVertex(new Vector3(-width, -height, depth));

            geometry.End();
        }
    }
}