using Godot;
using System;

namespace PrototypeTools {
    [Tool]
    public class PrototypeCube : Spatial { 
        [Export]
        private Material PrototypeMaterial {
            set { prototypeMaterial = value; UpdateMaterial(); }
            get { return prototypeMaterial; }
        }

        private Material prototypeMaterial = ResourceLoader.Load<ShaderMaterial>("res://addons/prototype_tools/prototype_material_shader.tres");
        private static PackedScene scene = ResourceLoader.Load<PackedScene>("res://addons/prototype_tools/PrototypeCube.tscn");
        private bool isInit = false;

        public override void _Ready() {
            this.AddChild(scene.Instance());
            isInit = true;
        }

        private void UpdateMaterial() {
            if (!isInit) return;
            this.GetNode<MeshInstance>("PrototypeCube/Cube").SetSurfaceMaterial(0, prototypeMaterial);
        }
    }
}