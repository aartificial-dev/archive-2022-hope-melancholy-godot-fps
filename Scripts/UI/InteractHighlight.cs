using Godot;
using System;

public class InteractHighlight : Control { 

    private readonly Vector2 RECT_SIZE_MIN = new Vector2(32f, 32f);

    private NinePatchRect ninePatch;
    private Label label;

    public override void _Ready() {
        ninePatch = GetNode<NinePatchRect>("NinePatchRect");
        label = GetNode<Label>("NinePatchRect/Label");
    }

	public override void _Process(float delta) {

	}

    public void ShowTooltip(String name, Vector2 pos, Vector2 size) {
        size.x = Mathf.Max(size.x, RECT_SIZE_MIN.x);
        size.y = Mathf.Max(size.y, RECT_SIZE_MIN.y);

        ninePatch.RectSize = size;
        ninePatch.RectPosition = pos - size / 2f;
        label.Text = name;

        this.Visible = true;
    }
}