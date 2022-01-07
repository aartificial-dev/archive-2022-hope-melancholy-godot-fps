using Godot;
using System;

public class OldPlayerCamera : Spatial { 

    [Export(PropertyHint.Range, "0,1")]
    private float sensitivity = 0.0025f;
    [Export]
    private float fov = 85;

    public float FOV { get => fov; }

    private float fovWanted;

    public bool mouseXInversed = false;
    public bool mouseYInversed = false;

    private Camera weaponCamera;
    private Spatial handsHolder;
    private Player player;
    private Camera camera;

    private Vector3 handsRest = new Vector3(0f, 0f, 0f);
    private Vector3 handsOffset = new Vector3(25f, 5f, 0f);

    private RayCast interactRay;
    private InteractiveBase interactive = null;

    private InteractHighlight hudInteract;

    public override void _Ready() {
        Input.SetMouseMode(Input.MouseMode.Captured);

        weaponCamera = GetNode<Camera>("../WeaponViewport/Viewport/WeaponCamera");
        camera = GetNode<Camera>("Camera");

        handsHolder = GetNode<Spatial>("Camera/HandsHolder");

        player = GetParent<Player>();

        interactRay = GetNode<RayCast>("InteractiveRayCast");

        hudInteract = GetNode<InteractHighlight>("../UI/InteractHighlight");

        fovWanted = fov;
    }

	public override void _Process(float delta) {
        weaponCamera.GlobalTransform = this.GlobalTransform;

        handsHolder.RotationDegrees = handsHolder.RotationDegrees.LinearInterpolate(handsRest, delta * 5f);
        handsHolder.Rotation = new Vector3(handsHolder.Rotation.x, handsHolder.Rotation.y, -this.Rotation.z * 3f);

        if (interactRay.IsColliding() && Input.IsActionJustPressed("key_use")) {
            if (interactRay.GetCollider() is InteractiveBase interactive) {
                interactive.Use();
            }
        }
        if (interactRay.IsColliding()) {
            if (interactRay.GetCollider() is InteractiveBase interactive) {
                if (this.interactive != interactive) {
                    this.interactive = interactive;
                    // Helper.GetHUDLog().Push( interactive.GetObjectName() ); 
                }
                Vector2 pos = camera.UnprojectPosition(this.interactive.GlobalTransform.origin);
                Vector2 size = CalculateInteractiveObjectSize(interactive.GetPoints(), this.interactive.GlobalTransform.origin, pos, interactive.Rotation);
                hudInteract.ShowTooltip(interactive.GetObjectName(), pos, size);
            } else {
                interactive = null;
                hudInteract.Visible = false;
            }
        } else {
            interactive = null;
            hudInteract.Visible = false;
        }

        camera.Fov = Mathf.Lerp(camera.Fov, fovWanted, delta * 1.5f);
	}

    public void UpdateFOV(float newFov) {
        fovWanted = newFov;
    }

    public override void _Input(InputEvent @event) {
        if (Input.GetMouseMode() != Input.MouseMode.Captured) return;
        if (@event is InputEventMouseMotion mouseMotion) {
            Vector2 mouseOffset = mouseMotion.Relative;

            Vector3 rot = this.Rotation;

            float xRotation = mouseOffset.y * sensitivity * (mouseYInversed ? -1 : 1);
            float yRotation = mouseOffset.x * sensitivity * (mouseXInversed ? -1 : 1);

            rot.x -= xRotation;
            rot.y -= yRotation;

            rot.x = Mathf.Clamp(rot.x, -Mathf.Pi / 2f, Mathf.Pi / 2f);
            rot.y = Mathf.Wrap(rot.y, 0, Mathf.Tau);

            this.Rotation = rot;

            Vector3 rotation = handsHolder.RotationDegrees;
            rotation.x -= xRotation * 3f;
            rotation.y += yRotation * 1.5f;
            rotation.x = Mathf.Clamp(rotation.x, handsRest.x - handsOffset.x, handsRest.x + handsOffset.x);
            rotation.y = Mathf.Clamp(rotation.y, handsRest.y - handsOffset.y, handsRest.y + handsOffset.y);
            handsHolder.RotationDegrees = rotation;
        }
    }

    private Vector2 CalculateInteractiveObjectSize(Position3D[] points, Vector3 pos3d, Vector2 pos, Vector3 rot) {
        Vector2 minPos = new Vector2(100000, 100000);
        Vector2 maxPos = new Vector2(-100000, -100000);

        foreach (Position3D point in points) {
            Vector2 unproj = camera.UnprojectPosition(point.GlobalTransform.origin);
            if (unproj.x > maxPos.x) maxPos.x = unproj.x;
            if (unproj.y > maxPos.y) maxPos.y = unproj.y;
            if (unproj.x < minPos.x) minPos.x = unproj.x;
            if (unproj.y < minPos.y) minPos.y = unproj.y;
        }

        return new Vector2(maxPos.x - minPos.x, maxPos.y - minPos.y);
    }

    public Camera GetCamera() {
        return camera;
    }

}