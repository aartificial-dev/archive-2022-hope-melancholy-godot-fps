using System;
using Godot;

public abstract class InteractiveBase : Area {

    [Export]
    protected String name = "undefined";
    [Export]
    protected Vector3 size = Vector3.One;

    public String GetObjectName() {return name;}
    public Vector3 GetObjectSize() {return size;}

    protected Position3D[] position3Ds;
    
    public Position3D[] GetPoints() { return position3Ds; }

    public override void _Ready() {
        UpdateGeometry();
    }

    public override void _Process(float delta) {

    }

    protected void UpdateGeometry() {
        position3Ds = new Position3D[8];
        position3Ds[0] = new Position3D();
        this.AddChild(position3Ds[0]);
        position3Ds[0].Translation = new Vector3(+ size.x / 2f, + size.y / 2f, + size.z / 2f);
        
        position3Ds[1] = new Position3D();
        this.AddChild(position3Ds[1]);
        position3Ds[1].Translation = new Vector3(- size.x / 2f, + size.y / 2f, + size.z / 2f);
        
        position3Ds[2] = new Position3D();
        this.AddChild(position3Ds[2]);
        position3Ds[2].Translation = new Vector3(- size.x / 2f, + size.y / 2f, - size.z / 2f);
        
        position3Ds[3] = new Position3D();
        this.AddChild(position3Ds[3]);
        position3Ds[3].Translation = new Vector3(+ size.x / 2f, + size.y / 2f, - size.z / 2f);
        
        position3Ds[4] = new Position3D();
        this.AddChild(position3Ds[4]);
        position3Ds[4].Translation = new Vector3(+ size.x / 2f, - size.y / 2f, + size.z / 2f);
        
        position3Ds[5] = new Position3D();
        this.AddChild(position3Ds[5]);
        position3Ds[5].Translation = new Vector3(- size.x / 2f, - size.y / 2f, + size.z / 2f);
        
        position3Ds[6] = new Position3D();
        this.AddChild(position3Ds[6]);
        position3Ds[6].Translation = new Vector3(- size.x / 2f, - size.y / 2f, - size.z / 2f);
        
        position3Ds[7] = new Position3D();
        this.AddChild(position3Ds[7]);
        position3Ds[7].Translation = new Vector3(+ size.x / 2f, - size.y / 2f, - size.z / 2f);
    }

    public abstract void Use();
}
