using Godot;
using System;

public class PlayerCamera : Spatial { 
    [Export]
    private Vector2 mouseSensetivity = new Vector2(0.25f, 0.25f);
    [Export]
    private bool invertMouseX = false;
    [Export]
    private bool invertMouseY = false;
    [Export]
    private readonly Vector2 maxTilt = new Vector2(-45f, 45f);

    private Camera camera;
    private Player player;

    private Area collisionArea; // camera collider to detect player

    private Vector3 defaultCameraPos = new Vector3(0.7f, 0f, 2f);
    private Vector3 desiredCameraPos = Vector3.Zero;

    private readonly Vector3 tiltLookUp = new Vector3(0.7f, 0.25f, 2f);
    private readonly Vector3 tiltLookForward = new Vector3(0.7f, 0f, 2f);
    private readonly Vector3 tiltLookDown = new Vector3(0.7f, 0.25f, 1.5f);

    private Vector3 desiredRotation = Vector3.Zero;

    private Vector2 mousePos = Vector2.Zero;

    private bool isShoulderSwitched = false;

    private readonly float shoulderDefPos = 0.7f;
    private float shoulderPos = 0.7f;

    public override void _Ready() {
        desiredCameraPos = defaultCameraPos;
        camera = GetNode<Camera>("Camera");
        player = GetParent<Player>();

        collisionArea = GetNode<Area>("Camera/Area");
        collisionArea.Connect("body_entered", this, nameof(BodyEntered));
        collisionArea.Connect("body_exited", this, nameof(BodyExited));

        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    public override void _Process(float delta) {
        ProcessMouse(delta);

        if (isShoulderSwitched) {
            // change camera pos
            // cast ray to camera pos * 2
        } else {

        }

        float tilt = Mathf.Rad2Deg(this.Rotation.x);
        float mAng = 25f;
        float amount = (Mathf.Abs(tilt) - mAng) / (45 - mAng);

        if (tilt < -mAng) {
            defaultCameraPos = tiltLookForward.LinearInterpolate(tiltLookDown, amount);
        } else if (tilt > mAng) {
            defaultCameraPos = tiltLookForward.LinearInterpolate(tiltLookUp, amount);
        } else {
            defaultCameraPos = tiltLookForward;
        }

        float zoomLerpSpd = DeltifyValue(0.8f, delta); // 0.25
        if (Input.IsActionPressed("key_aim")) {
            // new Vector3(1.5f, 0f, 1.5f)
            desiredCameraPos.x = Mathf.Lerp(desiredCameraPos.x, 1f, zoomLerpSpd);
            desiredCameraPos.z = Mathf.Lerp(desiredCameraPos.z, 1.35f, zoomLerpSpd);
            desiredCameraPos.y = Mathf.Lerp(desiredCameraPos.y, defaultCameraPos.y, zoomLerpSpd);
        } else {
            desiredCameraPos = desiredCameraPos.LinearInterpolate(defaultCameraPos, zoomLerpSpd);
        }        

        Vector3 defCamPos = desiredCameraPos;
        Vector3 camPos = defCamPos;
        if (GetNode<RayCast>("RayCast").IsColliding()) {

            Vector3 pos = GetNode<RayCast>("RayCast").GetCollisionPoint();
            Vector3 ori = this.GlobalTransform.origin;
            Vector3 desired = defCamPos;
            float len = ori.DistanceTo(pos);
            float maxlen = Vector3.Zero.DistanceTo(desired);
            len = Mathf.Min(len - 1f, maxlen);
            camPos = desired * (len / maxlen);
        }
        float camLerpSpd = DeltifyValue(0.8f, delta); // 0.5f
        camera.Translation = camera.Translation.LinearInterpolate(camPos, camLerpSpd);

    }
    
    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseMotion eventMouseMotion) {
            //mousePos = eventMouseMotion.Position / viewScale;
            mousePos = eventMouseMotion.Relative;
        }
    }

    public void InterpolateY(float y, float amount) {
        desiredRotation.y = Mathf.LerpAngle(desiredRotation.y, y, amount);
    }

    public void InterpolateX(float x, float amount) {
        desiredRotation.x = Mathf.LerpAngle(desiredRotation.x, x, amount);
    }

    public void InterpolateFOV(float fov, float amount) {
        camera.Fov = Mathf.Lerp(camera.Fov, fov, amount);
    }
    
    public Camera GetCamera() {
        return camera;
    }

    private void ProcessMouse(float delta) {
        float cameraPan = mousePos.x;
        float cameraTilt = mousePos.y;

        if (Input.IsActionJustPressed("key_switch_shoulder")) {
            // float normalLerpSpd = DeltifyValue(0.05f / camRecoverTime, delta);
            // cameraHolder.InterpolateY(desiredModelRotation, normalLerpSpd);
            // cameraHolder.InterpolateX(Mathf.Deg2Rad(-10f), normalLerpSpd);
            // desiredRotation.x = Mathf.Deg2Rad(-15f);
            // desiredRotation.y = player.GetModelRotation().y;
            isShoulderSwitched = !isShoulderSwitched;
        }

        Vector3 wantedRotation = desiredRotation;

        wantedRotation.x += cameraTilt * delta * mouseSensetivity.y * (invertMouseY ? 1f : -1f);
        wantedRotation.y += cameraPan * delta * mouseSensetivity.x * (invertMouseX ? 1f : -1f);

        wantedRotation.x = Mathf.Clamp(wantedRotation.x, Mathf.Deg2Rad(maxTilt.x), Mathf.Deg2Rad(maxTilt.y));

        wantedRotation.y = Mathf.Wrap(wantedRotation.y, 0, Mathf.Tau);

        if (Mathf.Abs(Mathf.Rad2Deg(AngleDiff(this.Rotation.y, wantedRotation.y))) <= 90f) {
            desiredRotation.y = wantedRotation.y;
        }
        desiredRotation.x = wantedRotation.x;
        
        Vector3 rot = this.Rotation;
        float turnLerpSpd = DeltifyValue(0.8f, delta); // 0.1f // 0.05f
        rot.x = Mathf.LerpAngle(rot.x, desiredRotation.x, turnLerpSpd);
        rot.y = Mathf.LerpAngle(rot.y, desiredRotation.y, turnLerpSpd);

        rot.y = Mathf.Wrap(rot.y, 0, Mathf.Tau);
        this.Rotation = rot;

        mousePos = Vector2.Zero;
    }

    private float AngleDiff(float from, float to) {
        float ans = Mathf.PosMod(to - from, Mathf.Tau);
        if (ans > Mathf.Pi) ans -= Mathf.Tau;
        return ans;
    }

    private void BodyEntered(Node body) {
        if (body is Player pl) {
            pl.Visible = false;
        }
    }

    private void BodyExited(Node body) {
        if (body is Player pl) {
            pl.Visible = true;
        }
    }

    /// <summary>
    /// Makes value follow delta speed
    /// </summary>
    /// <param name="value"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    private float DeltifyValue(float value, float delta) {
        return (value / 0.1666f) * delta;
    }
}