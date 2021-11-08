using Godot;
using System;

public class AdaptiveSoundtrack : Node { 
    
    [Export]
    private AdaptiveTrack adaptiveTrack = null;
    [Export]
    private bool isPlaying = true;
    [Export]
    private bool reload {set {Reload();} get {return false;}}

    private float minVolume = -20;
    private float maxVolume = 20;


    private AudioChannel[] audioChannels = new AudioChannel[10];
    private float[] channelVolume = new float[10];
    private bool[] channelEnabled = new bool[10];

    [Export]
    public float[] channelInfluence = new float[10];

    private RandomNumberGenerator rnd = new RandomNumberGenerator();

    public override void _Ready() {
        for (int i = 0; i < 10; i ++) {
            audioChannels[i] = GetNode<AudioChannel>($"AudioChannel{i}");
            channelInfluence[i] = 1f;
            channelVolume[i] = 1f;
            channelEnabled[i] = false;
        }

        Reload();
    }

	public override void _Process(float delta) {
        for (int i = 0; i < 10; i ++) {
            audioChannels[i].VolumeDb = Mathf.Lerp(audioChannels[i].VolumeDb, CalcVolume(i), delta / 4f);
        }
	}

    private void Reload() {
        if (adaptiveTrack is null) return;
        // getting adaptivechannels samples
        AdaptiveChannel[] channels = new AdaptiveChannel[10];
        for (int i = 0; i < 10; i ++) {
            var fchannel = adaptiveTrack.GetType().GetField($"channel{i}").GetValue(adaptiveTrack);
            if (!(fchannel is null)) {
                AdaptiveChannel channel = (AdaptiveChannel) fchannel;
                channels[i] = channel;
                channelVolume[i] = channel.channelVolume;
                channelEnabled[i] = channel.channelEnabled;
                audioChannels[i].samples = channel.channelAudio;
            } else {
                channels[i] = null;
                channelVolume[i] = 1f;
                channelEnabled[i] = false;
            }
        }

        // 
        for (int i = 0; i < 10; i ++) {
            audioChannels[i].VolumeDb = CalcVolume(i);
            if (channelEnabled[i] && isPlaying) {
                audioChannels[i].PlaySound();
            }
        }
    }

    private float CalcVolume(int channel) {
        if (!isPlaying) return minVolume;
        return minVolume + maxVolume * (channelVolume[channel] * channelInfluence[channel]);
    }
}