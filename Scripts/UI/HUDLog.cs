using Godot;
using System;
using System.Collections.Generic;

public class HUDLog : Control { 

    [Export]
    private float disappearTime = 1f;
    [Export]
    private float charAppearSpeed = 0.075f;

    private Label labelOutput;
    private Timer timerOutput;
    
    private Godot.Collections.Array<Label> log = new Godot.Collections.Array<Label>();
    private Godot.Collections.Array<Timer> timers = new Godot.Collections.Array<Timer>();
    private Queue<String> logQueue = new Queue<String>();

    private String wantedOutput = "";

    int prog = 0;
    int logAmount = 8;

    public override void _Ready() {
        labelOutput = GetNode<Label>("LabelOutput");
        timerOutput = GetNode<Timer>("Timers/TimerOutput");

        for (int i = 0; i < logAmount; i ++) {
            log.Add(GetNode<Label>( GD.Str("VBoxContainer/Log", i) ));
        }
        for (int i = 0; i < logAmount; i ++) {
            timers.Add(GetNode<Timer>( GD.Str("Timers/Timer", i) ));
        }

        timerOutput.Connect("timeout", this, nameof(UpdateOutputString));

        // GetNode<Timer>("Timer").Connect("timeout", this, nameof(UpdateBar));
        // GetNode<Timer>("Timer").Start(0.1f);
        Clear();
        // UpdateOutput(MakeLoadingBar(prog));
        // UpdateOutput("Debug info");
        Push("Time is on our side");
        Push("You will not escape");
        Push("And therefore there's no end");
        Push("A");
        Push("A");
        Push("A");
    }

    private void UpdateBar() {
        prog ++;
        if (prog == 100) {
            GetNode<Timer>("Timer").Start(0.5f);
        } else {
            GetNode<Timer>("Timer").Start(0.1f);
        }
        if (prog > 100) {
            prog = 0;
        }
        UpdateOutput(MakeLoadingBar(prog));
    }

	public override void _Process(float delta) {
        // UpdateLog(GD.Str("FPS: ", Engine.GetFramesPerSecond(), "/", (Engine.TargetFps == 0) ? ("Unlimited") : (Engine.TargetFps.ToString())), 0);
        // UpdateLog(GD.Str("Delta: ", delta.ToString().Substring(0, Mathf.Min(6, delta.ToString().Length))), 1);
        // UpdateLog(GD.Str("Memory: ", BytesToString(OS.GetStaticMemoryUsage())), 2);
        if (wantedOutput.Length == 0 && timerOutput.TimeLeft == 0f && logQueue.Count > 0) {
            wantedOutput = logQueue.Dequeue();
            timerOutput.Start(charAppearSpeed);
        }

        for (int i = 0; i < logAmount; i ++) {
            float alpha = timers[i].TimeLeft / disappearTime;
            Color col = log[i].Modulate;
            col.a = alpha;
            log[i].Modulate = col;
        }
	}

    public void Clear() {
        labelOutput.Text = "";
        for (int i = 0; i < logAmount; i ++) {
            log[i].Text = "";
            timers[i].Stop();
        }
    }

    public void UpdateOutputString() {
        if (wantedOutput.Length == 0 && labelOutput.Text.Length > 0) {
            PushImmediately();
            timerOutput.Start(charAppearSpeed * 2.5f);
        }
        if (wantedOutput.Length > 0) {
            String c = wantedOutput.Substr(0, 1);
            labelOutput.Text = GD.Str(labelOutput.Text, c);
            // if (c != " ")
            GetNode<AudioStreamPlayer>("AudioStreamPlayer").Play();
            
            if (wantedOutput.Length - 1 == 0) {
                wantedOutput = "";
            } else {
                wantedOutput = wantedOutput.Substr(1, wantedOutput.Length - 1);
            }

            if (wantedOutput.Length == 0) {
                // PushImmediately();
                timerOutput.Start(charAppearSpeed * 5f);
            } else {
                timerOutput.Start(charAppearSpeed);
            }
        }
    }

    public void UpdateLog(String newLog, int logInd) {
        logInd = Mathf.Clamp(logInd, 0, logAmount - 1);
        log[logInd].Text = newLog;
    }

    public void UpdateOutput(String newOutput) {
        labelOutput.Text = newOutput;
    }

    /// <summary> Immediately pushes empty string to log ignoring the queue. </summary>
    public void PushImmediately() {
        if (wantedOutput.Length > 0) {
            labelOutput.Text = wantedOutput;
            wantedOutput = "";
        }
        for (int i = logAmount - 1; i > 0; i --) {
            log[i].Text = log[i - 1].Text;
            if (timers[i - 1].TimeLeft != 0f) {
                timers[i].Start(timers[i - 1].TimeLeft);
            }
        }
        timers[0].Start(disappearTime);
        log[0].Text = labelOutput.Text;
        labelOutput.Text = "";
        timerOutput.Stop();
    }

    /// <summary> Immediately pushes new string to log ignoring the queue. </summary>
    public void PushImmediately(String newOutput) {
        PushImmediately();
        labelOutput.Text = newOutput;
    }

    /// <summary> Pushes output in end of log queue </summary>
    public void Push(String output) {
        logQueue.Enqueue(output);
    }

    public String MakeLoadingBar(int percent) {
        percent = Mathf.Clamp(percent, 0, 100);

        // [█████               ] 25%
        String loadChar = "█";
        String emptyChar = " ";
        int loadLength = 20;
        int loaded = percent / (100 / loadLength);
        
        String output = "[";

        for (int i = 0; i < loaded; i ++) {
            output = GD.Str(output, loadChar);
        }
        for (int i = loaded; i < loadLength; i ++) {
            output = GD.Str(output, emptyChar);
        }
        output = GD.Str(output, "] ", percent, "%");

        return output;
    }
    

    private String BytesToString(ulong byteCount){
        String[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0)
            return "0" + suf[0];
        int place = Convert.ToInt32(Math.Floor(Math.Log(byteCount, 1024)));
        double num = Math.Round(byteCount / Math.Pow(1024, place), 1);
        return num.ToString() + suf[place];
    }
}


