using Godot;
using System;

public class SpatialEditorOnly : Spatial { 
    private enum RemoveMode {
        hide, delete, disable
    }

    [Export]
    private RemoveMode removeMode = RemoveMode.hide;

    public override void _Ready() {
        switch (removeMode) {
            case RemoveMode.hide:
                this.Visible = false;
            return;
            case RemoveMode.delete:
                this.GetParent().RemoveChild(this);
                this.QueueFree();
            return;
        }
    }
}