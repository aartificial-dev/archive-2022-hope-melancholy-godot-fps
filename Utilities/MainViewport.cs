using Godot;
using System;

public class MainViewport : ViewportContainer { 
    
    private Vector2 resolution = new Vector2(1920f, 1080f);
    private ShaderMaterial shader;

    public override void _Ready() {
        // resolution = this.RectSize;
        shader = (ShaderMaterial) this.Material;
    }

	public override void _Process(float delta) {
        Vector2 screen = OS.WindowSize;

        float scale = resolution.x / screen.x;
        // GD.Print(scale, " , ", Mathf.Max(Mathf.Floor(scale * 100f) / 100f, 1.5f));

        if (Input.IsActionJustPressed("key_f4") && !Input.IsKeyPressed( (int) Godot.KeyList.Alt)) {
            OS.WindowFullscreen = !OS.WindowFullscreen;
        }

        shader.SetShaderParam("crt_scale", Mathf.Max(scale, 1f));

        if (Input.IsActionJustPressed("key_esc")) {
            this.GetTree().Quit();
        }
        if (Input.IsActionJustPressed("key_debug_reload")) {
            this.GetTree().ChangeScene(this.GetTree().CurrentScene.Filename);
        }
	}
}