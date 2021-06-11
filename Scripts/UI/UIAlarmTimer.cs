using Godot;
using System;

public class UIAlarmTimer : Control { 

    private Label labelTimer;

    public override void _Ready() {
        this.Visible = false;
        labelTimer = GetNode<Label>("Timer/LabelTimer");
    }

	public override void _Process(float delta) {
        if (SecurityAlarmStatus.AlarmTime > 0f) {
            this.Visible = true;

            int iTime = Mathf.FloorToInt(SecurityAlarmStatus.AlarmTime);

            String dec = iTime.ToString();
            String fra = Mathf.FloorToInt(SecurityAlarmStatus.AlarmTime * 100f - (iTime * 100f)).ToString();
            dec = dec.Length == 2 ? dec : GD.Str(new String('0', 2 - dec.Length), dec);
            fra = fra.Length == 2 ? fra : GD.Str(new String('0', 2 - fra.Length), fra);

            labelTimer.Text = GD.Str(dec, ".", fra);

        } else {
            this.Visible = false;
        }
	}
}