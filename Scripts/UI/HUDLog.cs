using Godot;
using System;

public class HUDLog : Control { 

    private Label labelOutput;
    private Godot.Collections.Array<Label> log = new Godot.Collections.Array<Label>();

    int prog = 0;
    int logAmount = 8;

    public override void _Ready() {
        labelOutput = GetNode<Label>("LabelOutput");

        for (int i = 0; i < logAmount; i ++) {
            log.Add(GetNode<Label>( GD.Str("VBoxContainer/Log", i) ));
        }

        GetNode<Timer>("Timer").Connect("timeout", this, nameof(UpdateBar));
        // GetNode<Timer>("Timer").Start(0.1f);
        // Clear();
        UpdateOutput(MakeLoadingBar(prog));
        // UpdateOutput("Debug info");
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
	}

    public void Clear() {
        labelOutput.Text = "";
        for (int i = 0; i < logAmount; i ++) {
            log[i].Text = "";
        }
    }

    public void UpdateLog(String newLog, int logInd) {
        logInd = Mathf.Clamp(logInd, 0, logAmount - 1);
        log[logInd].Text = newLog;
    }

    public void UpdateOutput(String newOutput) {
        labelOutput.Text = newOutput;
    }

    public void Push() {
        for (int i = logAmount - 1; i > 0; i --) {
            log[i].Text = log[i - 1].Text;
        }
        log[0].Text = labelOutput.Text;
        labelOutput.Text = "";
    }

    public void Push(String newOutput) {
        Push();
        labelOutput.Text = newOutput;
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


