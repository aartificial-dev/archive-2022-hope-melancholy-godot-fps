using Godot;
using System;

public class AdaptiveChannel : Resource { 
    [Export]
    public String channelName = "channel";

    [Export]
    public bool channelEnabled = true;

    [Export]
    public AudioStream[] channelAudio;

    [Export(PropertyHint.Range, "0,1,0.01")]
    public float channelVolume = 1f;
}