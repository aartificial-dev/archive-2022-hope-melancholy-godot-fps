using Godot;
using System;

public class AudioPlayerOnce3D : AudioStreamPlayer3D { 
    public override void _Ready() {
        this.Connect("finished", this, nameof(Destroy));
    }

    public void Destroy() {
        this.GetParent().RemoveChild(this);
        this.QueueFree();
    }
}