using Godot;
using System;

public class VFXTimer : Spatial { 
    [Export(PropertyHint.Range, "0,200,0.1")]
    private float time = 1f;

    private Timer timer;

    public override void _Ready() {
        timer = new Timer();
        this.AddChild(timer);
        timer.OneShot = true;
        timer.Connect("timeout", this, nameof(Destroy));
        timer.Start(time);
    }
    
    private void Destroy() {
        this.QueueFree();
    }
}