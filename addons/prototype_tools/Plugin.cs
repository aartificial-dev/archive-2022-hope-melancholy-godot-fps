#if TOOLS
using System;
using Godot;

namespace PrototypeTools {
	[Tool]
	public class Plugin : EditorPlugin	{

		private Script script = ResourceLoader.Load<Script>("res://addons/prototype_tools/PrototypeCube.cs");
		private Texture texture = ResourceLoader.Load<Texture>("res://addons/prototype_tools/PrototypeCubeIcon.svg");

		public override void _EnterTree() {
			// Initialization of the plugin goes here.
		AddCustomType("PrototypeCube", "Spatial", script, texture);
		}

		public override void _ExitTree() {
			// Clean-up of the plugin goes here.
		RemoveCustomType("PrototypeCube");
		}
	}
}
#endif
