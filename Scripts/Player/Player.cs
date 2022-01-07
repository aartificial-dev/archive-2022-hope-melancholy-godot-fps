using Godot;
using System;

public class Player : KinematicBody { 
    [Export]
    private NodePath modelHolderPath = new NodePath("ModelHolder");
    [Export]
    private NodePath cameraHolderPath = new NodePath("CameraHolder");
    [Export]
    private NodePath headHolderPath = new NodePath("ModelHolder/HeadHolder");
    [Export]
    private NodePath rayCastPath = new NodePath("ModelHolder/RayCast");
    [Export]
    private readonly float gravity = 9.8f;
    [Export]
    private readonly float maxSpeed = 3f;

    private Spatial modelHolder;
    private PlayerCamera cameraHolder;
    private RayCast rayCast;

    private Spatial headHolder;
	private readonly Vector3 headAngleMax = new Vector3(30f, 25f, 45f);
	private readonly Vector3 headAngleMin = new Vector3(-30f, -25f, -45f);
	private readonly Vector3 headAngleDef = new Vector3(15f, 0, 0); // Nod (look +down -up), Tilt (tilt +right -left), Shake (look -left +right)
	private Vector3 headAngleCur = Vector3.Zero;

	private float camRecoverTime = 1f;

    public override void _Ready() {
        modelHolder = GetNode<Spatial>(modelHolderPath);
        cameraHolder = GetNode<PlayerCamera>(cameraHolderPath);
        rayCast = GetNode<RayCast>(rayCastPath);
    }

	public override void _Process(float delta) {
        if (rayCast.GetCollider() is InteractiveBase) {
            InteractiveBase interactive = (InteractiveBase) rayCast.GetCollider();

            if (Input.IsActionJustPressed("key_use")) {
                interactive.Use();
            }
        }
	}

    public override void _PhysicsProcess(float delta) {
        ProcessInput(delta);
    }

    public override void _Input(InputEvent e) {
        if (e is InputEventMouseMotion eventMouseMotion) {
			GetNode<Timer>("Timer").Start(camRecoverTime);
        }
    }

    private void ProcessInput(float delta) {
        Vector3 movement = Vector3.Zero;

        if (Input.IsActionPressed("key_right")) {
            movement.x += 1f;
        }
        if (Input.IsActionPressed("key_left")) {
            movement.x -= 1f;
        }
        if (Input.IsActionPressed("key_forward")) {
            movement.z -= 1f;
        }
        if (Input.IsActionPressed("key_backward")) {
            movement.z += 1f;
        }

        movement = movement.Normalized();


        Vector3 velocity = movement * maxSpeed; // do rootmotion instead
        velocity = velocity.Rotated(Vector3.Up, cameraHolder.Rotation.y);

        float desiredModelRotation = Mathf.Wrap(Mathf.Atan2(velocity.x, velocity.z) + Mathf.Pi, 0, Mathf.Tau);

        if (movement != Vector3.Zero || Input.IsActionPressed("key_aim")) {
            // Vector3 rot = modelHolder.Rotation;
            // rot.y = Mathf.LerpAngle(rot.y, cameraHolder.Rotation.y, DeltifyValue(0.5f / camRecoverTime, delta));
            // modelHolder.Rotation = rot;
            Vector3 rot = modelHolder.Rotation;
            rot.y = Mathf.LerpAngle(rot.y, cameraHolder.Rotation.y, DeltifyValue(0.5f / camRecoverTime, delta));
            modelHolder.Rotation = rot;
        } 
        // else if (movement != Vector3.Zero) {
        //     Vector3 rot = modelHolder.Rotation;
        //     rot.y = Mathf.LerpAngle(rot.y, desiredModelRotation, DeltifyValue(0.5f / camRecoverTime, delta));
        //     modelHolder.Rotation = rot;
        // }


        if (movement != Vector3.Zero) {
            float fovLerpSpeed = DeltifyValue(0.1f, delta);
            if (Input.IsActionPressed("key_sprint")) {
                velocity *= 1.5f;
				// stateMachine.Travel("Run-loop");
				camRecoverTime = 0.4f;
				cameraHolder.InterpolateFOV(85f, fovLerpSpeed);
			} else if (Input.IsActionPressed("key_crouch")) {
                velocity /= 1.5f;
				// stateMachine.Travel("WalkSlow-loop");
				camRecoverTime = 2f;
				cameraHolder.InterpolateFOV(75f, fovLerpSpeed);
			} else {
				// stateMachine.Travel("Walk-loop");
				camRecoverTime = 1f;
				cameraHolder.InterpolateFOV(80f, fovLerpSpeed);
			}
			if (GetNode<Timer>("Timer").TimeLeft == 0f) {
				float normalLerpSpd = DeltifyValue(0.1f / camRecoverTime, delta);
				cameraHolder.InterpolateX(0f, normalLerpSpd);
			}
		} else {
			// stateMachine.Travel("Idle-loop");
		}

        if (!IsOnFloor()) {
            velocity.y = -gravity;
        }

        this.MoveAndSlide(velocity, Vector3.Up, true, 4, 0.785f, false);
    }

    public Vector3 GetModelRotation() {
        return modelHolder.Rotation;
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