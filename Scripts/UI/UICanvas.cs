using Godot;
using System;

public class UICanvas : Control { 

    // private Position3D posTR;
    // private Position3D posTL;
    // private Position3D posBR;
    // private Position3D posBL;

    private DynamicFont font = ResourceLoader.Load<DynamicFont>("res://Assets/Fonts/Consolas.tres");

    public override void _Ready() {
        // posTR = GetNode<Position3D>("../PlayerCamera/Camera/HandsHolder/Hands/CSGBox/posTR");
        // posTL = GetNode<Position3D>("../PlayerCamera/Camera/HandsHolder/Hands/CSGBox/posTL");
        // posBR = GetNode<Position3D>("../PlayerCamera/Camera/HandsHolder/Hands/CSGBox/posBR");
        // posBL = GetNode<Position3D>("../PlayerCamera/Camera/HandsHolder/Hands/CSGBox/posBL");
    }

	public override void _Process(float delta) {
        if (Input.IsActionJustPressed("key_inventory")) {
            Input.SetMouseMode( (Input.GetMouseMode() == Input.MouseMode.Captured) ? Input.MouseMode.Visible : Input.MouseMode.Captured );
        }

        if (Input.GetMouseMode() == Input.MouseMode.Visible && Input.IsActionJustPressed("key_attack")) {
            // UnprojectMousePos();
        }
        Update();
	}

    // public override void _Draw() {
    //     if (Input.GetMouseMode() != Input.MouseMode.Visible) {
    //         return;
    //     }
    //     // correct aspect ration in which UV.y will work normally
    //     float uvYCorrect = 1.7666f;
    //     float aRatio = (OS.WindowSize.x / OS.WindowSize.y);

    //     Vector2 mousePos = this.GetGlobalMousePosition() * (this.RectSize / OS.WindowSize);
    //     Camera cam = Helper.GetCamera().GetCamera();

    //     Vector2 posTopLeft = cam.UnprojectPosition(posTL.GlobalTransform.origin);
    //     Vector2 posTopRight = cam.UnprojectPosition(posTR.GlobalTransform.origin);
    //     Vector2 posBottomLeft = cam.UnprojectPosition(posBL.GlobalTransform.origin);
    //     Vector2 posBottomRight = cam.UnprojectPosition(posBR.GlobalTransform.origin);
        
    //     Vector2 uv = UnprojectUV(posTopLeft, posTopRight, posBottomRight, posBottomLeft, mousePos);
        
    //     // FUCKING MAGIC NUMBER I DON'T KNOW HOW I GOT IT BUT IT WORKS DON'T TOUCH IT
    //     float ratioFix = Mathf.Abs(uvYCorrect - aRatio) * 3.425f; 
    //     if (uvYCorrect > aRatio) {
    //         uv.y += ratioFix;
    //     } else {
    //         uv.y -= ratioFix;
    //     }

    //     Color colRed = Color.ColorN("red");
    //     Color colBlue = Color.ColorN("blue");

    //     bool isMouseIn = !((uv.x < 0f || uv.x > 1f) || (uv.y < 0f || uv.y > 1f));

    //     this.DrawLine(posTopLeft, posTopRight, isMouseIn ? colBlue : colRed, 2);
    //     this.DrawLine(posTopRight, posBottomRight, isMouseIn ? colBlue : colRed, 2);
    //     this.DrawLine(posBottomRight, posBottomLeft, isMouseIn ? colBlue : colRed, 2);
    //     this.DrawLine(posBottomLeft, posTopLeft, isMouseIn ? colBlue : colRed, 2);

    //     this.DrawString(font, posTopLeft, posTopLeft.ToString(), isMouseIn ? colRed : colRed);
    //     this.DrawString(font, posTopRight, posTopRight.ToString(), isMouseIn ? colRed : colRed);
    //     this.DrawString(font, posBottomRight, posBottomRight.ToString(), isMouseIn ? colRed : colRed);
    //     this.DrawString(font, posBottomLeft, posBottomLeft.ToString(), isMouseIn ? colRed : colRed);

    //     this.DrawLine(posTopLeft.LinearInterpolate(posTopRight, uv.x), posBottomLeft.LinearInterpolate(posBottomRight, uv.x), colRed, 2);
    //     this.DrawLine(posTopLeft.LinearInterpolate(posBottomLeft, uv.y), posTopRight.LinearInterpolate(posBottomRight, uv.y), colRed, 2);

    //     this.DrawCircle(mousePos, 5, colRed);
    // }

    // private Vector2 UnprojectMousePos() {
    //     Vector2 mousePos = Helper.GetFixedGlobalMousePosition(this);//this.GetGlobalMousePosition();
    //     Camera cam = Helper.GetCamera().GetCamera();

    //     Vector2 posTopLeft = cam.UnprojectPosition(posTL.GlobalTransform.origin);
    //     Vector2 posTopRight = cam.UnprojectPosition(posTR.GlobalTransform.origin);
    //     Vector2 posBottomLeft = cam.UnprojectPosition(posBL.GlobalTransform.origin);
    //     Vector2 posBottomRight = cam.UnprojectPosition(posBR.GlobalTransform.origin);
    //     GD.Print(' ');
    //     GD.Print("TL: ", (posTopLeft / 100f).Floor());
    //     GD.Print("TR: ", (posTopRight / 100f).Floor());
    //     GD.Print("BL: ", (posBottomLeft / 100f).Floor());
    //     GD.Print("BR: ", (posBottomRight / 100f).Floor());
    //     GD.Print("MP: ", (mousePos / 100f).Floor());
    //     Vector2 unprojPos = UnprojectUV(posTopLeft, posTopRight, posBottomLeft, posBottomRight, mousePos);

    //     GD.Print("UP: ", unprojPos);
    //     GD.Print("UK: ", unprojPos * Helper.GetWindowSize());
    //     GetNode<ColorRect>("Pointer").RectPosition = unprojPos * Helper.GetWindowSize() - new Vector2(8f, 8f);
    //     return unprojPos;
    // }

    // /// points are supplied in clockwise order starting from top left
    // private Vector2 UnprojectUV(Vector2 x, Vector2 y, Vector2 z, Vector2 w, Vector2 p) {
    //     float v_pow = Mathf.Pow( (z.x * p.y - w.x * p.y - z.x * x.y + 2 * w.x * x.y - w.x * y.y + x.x * (p.y + z.y - 2 * w.y) + y.x * (-1 * p.y + w.y) + p.x * (-1 * x.y + y.y - z.y + w.y)) , 2);
    //     float v_sqrt = Mathf.Sqrt( 4 * ( (z.x - w.x) * (x.y - y.y) - (x.x - y.x) * (z.y - w.y)) * (w.x * (-1 * p.y + x.y) + x.x * (p.y - w.y) + p.x * (-1 * x.y + w.y)) + v_pow );

    //     float u_pow = Mathf.Pow( (z.x * p.y - w.x * p.y - z.x * x.y + 2 * w.x * x.y - w.x * y.y + x.x * (p.y + z.y - 2 * w.y) + y.x * (-1 * p.y + w.y) + p.x * (-1 * x.y + y.y - z.y + w.y)) , 2);
    //     float u_sqrt = Mathf.Sqrt( 4 * ((z.x - w.x) * (x.y - y.y) - (x.x - y.x) * (z.y - w.y)) * (  w.x * (-1 * p.y + x.y) + x.x * (p.y - w.y) + p.x * (-1 * x.y + w.y)) + u_pow );

    //     float k = 1 / (2 * ((z.x - w.x) * (x.y - y.y) - (x.x - y.x) * (z.y - w.y)));
    //     float l = 1 / (2 * ((x.x - w.x) * (y.y - z.y) - (y.x - z.x) * (x.y - w.y)));

    //     // float v1 = l * (y.x * p.y - z.x * p.y + w.x * p.y + p.x * x.y - 2 * y.x * x.y + z.x * x.y - p.x * y.y - w.x * y.y + p.x * z.y - x.x * (p.y - 2 * y.y + z.y) - p.x * w.y + y.x * w.y + v_sqrt);
    //     // float u1 = -1 * k * (-y.x * p.y + z.x * p.y - p.x * x.y - z.x * x.y + 2 * w.x * x.y + p.x * y.y - w.x * y.y - p.x * z.y + x.x * (p.y + z.y - 2 * w.y) + p.x * w.y + y.x * w.y + u_sqrt);

    //     float v2 = -1 * l * (x.x * p.y + z.x * p.y - w.x * p.y - p.x * x.y - 2 * z.x * x.y + p.x * y.y - (-2) * x.x * y.y + w.x * y.y - p.x * z.y + x.x * z.y + p.x * w.y - y.x * (p.y - 2 * x.y + w.y) + v_sqrt);
    //     float u2 = k * (y.x * p.y - z.x * p.y + w.x * p.y + p.x * x.y + z.x * x.y - 2 * w.x * x.y - p.x * y.y + w.x * y.y + p.x * z.y - x.x * (p.y + z.y - 2 * w.y) - p.x * w.y - y.x * w.y + u_sqrt);
    //     return new Vector2(u2, v2);
    // }
}