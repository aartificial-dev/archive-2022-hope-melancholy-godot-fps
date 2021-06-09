using Godot;
using System;

public class AudioPlayerRandom : AudioStreamPlayer { 
    [Export]
    public Godot.Collections.Array<AudioStream> samples = new Godot.Collections.Array<AudioStream>();
    [Export]
    public bool randomSound = false;
    [Export]
    public bool randomStart = false;
    [Export]
    public bool autoStart = false;

    private RandomNumberGenerator rnd = new RandomNumberGenerator();

    public override void _Ready() {
        rnd.Randomize();
        if (autoStart) {
            PlaySound();
        }
    }

    public void PlaySound() {
        AudioStream sample = this.Stream;
        if (randomSound && samples.Count > 0) {
            sample = samples[ rnd.RandiRange(0, samples.Count - 1) ];
        }
        this.Stream = sample;

        float startPos = 0f;
        if (randomStart) {
            startPos = rnd.RandfRange(0f, sample.GetLength());
        }

        this.Play(startPos);
    }
}