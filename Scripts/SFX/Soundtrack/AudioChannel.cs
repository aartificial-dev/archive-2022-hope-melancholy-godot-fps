using Godot;
using System;

public class AudioChannel : AudioStreamPlayer { 
    [Export]
    public AudioStream[] samples = new AudioStream[]{};

    private RandomNumberGenerator rnd = new RandomNumberGenerator();

    public override void _Ready() {
        rnd.Randomize();
        this.Connect("finished", this, nameof(PlaySound));
    }

    public void PlaySound() {
        if (samples is null) return;
        AudioStream sample = this.Stream;
        if (samples.Length > 0) {
            sample = samples[ rnd.RandiRange(0, samples.Length - 1) ];
        }
        this.Stream = sample;

        this.Play();
    }
}