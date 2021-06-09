using Godot;
using System;

public class SecurityAlarm : Spatial { 
    private AudioStreamPlayer3D audioPlayer;
    private bool state = false;

    [Export]
    public Godot.Collections.Array<NodePath> alarms = new Godot.Collections.Array<NodePath>();
    [Export]
    public NodePath switchPath = "";

    private SwitchBase switchNode;

    public override void _Ready() {
        foreach (NodePath path in alarms) {
            Node n = GetNode<Node>(path);
            if (n is IAlarmSetter alarm) {
                n.Connect(nameof(IAlarmSetter.AlarmSignal), this, nameof(ActivateAlarm));
            }
        }
        if (GetNode(switchPath) is SwitchBase sw) {
            switchNode = sw;
            sw.Connect(nameof(SwitchBase.SwitchSignal), this, nameof(DeactivateAlarm));
            switchNode.State = state;
        }

        audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
    }

	public override void _Process(float delta) {
	}

    public void ActivateAlarm() {
        if (!audioPlayer.Playing) {
            audioPlayer.Play();
            GetNode<AudioStreamPlayer3D>("AudioAlertTerminated").Stop();
            GetNode<AudioStreamPlayer3D>("AudioThreat").Play();
        }
        state = true;
        switchNode.State = state;
    }

    public void DeactivateAlarm(bool _b) {
        if (audioPlayer.Playing) {
            audioPlayer.Stop();
            GetNode<AudioStreamPlayer3D>("AudioThreat").Stop();
            GetNode<AudioStreamPlayer3D>("AudioAlertTerminated").Play();
        }
        state = false;
        switchNode.State = state;
    }
}