using Godot;
using System;

public class Player : KinematicBody { 
    private PlayerCamera camera;
    private PlayerStats playerStats = new PlayerStats();
    
    private float gravity = 9.8f;
    private Vector3 velocity = Vector3.Zero;
    private Vector3 velocityMax = new Vector3(3f, 60f, 3.5f);
    private float velocityFalloff = 10f;
    private float velocityLerp = 6f;
    private float rotationY = 0f;
    private float jumpForce = 4f;

    private float headBobTimer = 0f;

    private const float MAX_SLOPE_ANGLE = 0.785398f;
    private const float CAMERA_Y_STANDING = 1.7f;
    private const float CAMERA_Y_CROUCHING = 1.033f;

    private Vector3 prevPos;

    private RayCast floorRayCast;

    private CollisionShape collisionStanding;
    private CollisionShape collisionCrouching;

    private AudioPlayerRandom3D audioFootStep;
    private AudioStreamPlayer3D audioJump;
    private AudioStreamPlayer3D audioFall;

    private Timer timerFootStep;

    private AnimationPlayer cameraAnimator;

    public override void _Ready() {
        camera = GetNode<PlayerCamera>("PlayerCamera");
        floorRayCast = GetNode<RayCast>("FloorRayCast");

        collisionStanding = GetNode<CollisionShape>("CollisionStanding");
        collisionCrouching = GetNode<CollisionShape>("CollisionCrouching");

        audioFootStep = GetNode<AudioPlayerRandom3D>("Audio/AudioFootStep");
        audioJump = GetNode<AudioStreamPlayer3D>("Audio/AudioJump");
        audioFall = GetNode<AudioStreamPlayer3D>("Audio/AudioFall");
        
        timerFootStep = GetNode<Timer>("Audio/TimerFootStep");
        timerFootStep.Connect("timeout", this, nameof(PlayFootStepSound));

        cameraAnimator = GetNode<AnimationPlayer>("PlayerCamera/CameraAnimator");

        prevPos = this.Translation;
    }

	public override void _PhysicsProcess(float delta) {
        Vector3 moveVec = Vector3.Zero;
        if (this.IsOnFloor()) {
            rotationY = camera.Rotation.y + this.Rotation.y;
        }

        bool isJumping = false;
        bool isSprinting = false;
        bool isCrouching = false;

        if (Input.IsActionPressed("key_forward")) moveVec.z -= 1f;
        if (Input.IsActionPressed("key_backward")) moveVec.z += 1f;
        if (Input.IsActionPressed("key_left")) moveVec.x -= 1f;
        if (Input.IsActionPressed("key_right")) moveVec.x += 1f;
        if (Input.IsActionJustPressed("key_jump")) isJumping = true;
        if (Input.IsActionPressed("key_sprint")) isSprinting = true;
        if (Input.IsActionPressed("key_crouch")) isCrouching = true;

        float crouchSpeed = 3.5f;
        if (isCrouching) {
            collisionStanding.Disabled = true;
            collisionCrouching.Disabled = false;
            camera.Translation = camera.Translation.LinearInterpolate(new Vector3(0f, CAMERA_Y_CROUCHING, 0f), delta * crouchSpeed);
        } else {
            collisionStanding.Disabled = false;
            collisionCrouching.Disabled = true;
            camera.Translation = camera.Translation.LinearInterpolate(new Vector3(0f, CAMERA_Y_STANDING, 0f), delta * crouchSpeed);
        }

        CalculateVelocity(moveVec, isSprinting, isCrouching, isJumping, delta);

        if (floorRayCast.GetCollider() is MovingPlatform platform) {
            this.Translate(-platform.Velocity);
            camera.RotateY(platform.Torque);
        }
        float yVelocity = velocity.y;
        velocity = velocity.Rotated(Vector3.Up, rotationY);

        velocity = this.MoveAndSlide(velocity, Vector3.Up, true, 4, MAX_SLOPE_ANGLE, false);

        velocity = velocity.Rotated(Vector3.Up, -rotationY);

        if (yVelocity < -9f && velocity.y > -2f) {
            audioFall.Play();
            cameraAnimator.Play("fall");
        }

        if (moveVec != Vector3.Zero && Mathf.Abs(velocity.y) < 1f) {
            // 6 is max velocity magnitude (sprinting forward)
            float bobStrength = 0.004f;
            float bobSpeedModifier = (velocity.Length() / 5.5f) * 6f;
            headBobTimer += delta * bobSpeedModifier;
            headBobTimer = Mathf.Wrap(headBobTimer, 0f, Mathf.Tau);
            Vector3 rot = camera.Rotation;
            rot.z = Mathf.Sin(headBobTimer) * bobStrength;
            camera.Rotation = rot;
            ProcessStepSound();
        } else {
            headBobTimer = 0f;
            Vector3 rot = camera.Rotation;
            rot.z = Mathf.Lerp(rot.z, 0f, delta);
            camera.Rotation = rot;
            if (timerFootStep.TimeLeft != 0) {
                timerFootStep.Stop();
            }
        }

        // TODO ladder climbing

        // Helper.GetHUDLog().UpdateOutput(GD.Str("Speed: ", Mathf.FloorToInt((this.Translation * new Vector3(1f, 0f, 1f)).DistanceTo(prevPos * new Vector3(1f, 0f, 1f)) * 10000f)));
        prevPos = this.Translation;
	}

    private void CalculateVelocity(Vector3 moveVec, bool isSprinting, bool isCrouching, bool isJumping, float delta) {
        moveVec = Vector3Spheritize(moveVec);
        float sprintModifier = (moveVec.z < 0f && isSprinting && !isCrouching) ? 1.5f : 1f;
        float backpedal = (moveVec.z > 0f) ? 0.75f : 1f;
        float crouchModifier = isCrouching ? 0.3f : 1f;
        if (moveVec.z != 0f) {
            velocity.z = Mathf.Lerp(velocity.z, velocityMax.z * moveVec.z * sprintModifier * backpedal * crouchModifier, delta * velocityLerp);
        } else {
            velocity.z = Mathf.Lerp(velocity.z, 0, delta * velocityFalloff);
        }
        if (moveVec.x != 0f) {
            velocity.x = Mathf.Lerp(velocity.x, velocityMax.x * moveVec.x * crouchModifier, delta * velocityLerp);
        } else {
            velocity.x = Mathf.Lerp(velocity.x, 0, delta * velocityFalloff);
        }
        if (!this.IsOnFloor()) {
            velocity.y -= gravity * delta;
            velocity.y = Mathf.Clamp(velocity.y, -velocityMax.y, velocityMax.y);
        }
        if (isJumping && floorRayCast.IsColliding() && !isCrouching) {
            velocity.y = jumpForce;
            audioJump.Play();
            timerFootStep.Stop();
            cameraAnimator.Play("jump");
        }
    }
    
    private void ProcessStepSound() {
        if (timerFootStep.TimeLeft == 0) {
            float baseTime = 0.65f;
            if (Input.IsActionPressed("key_crouch")) {
                timerFootStep.Start(baseTime * 1.8f);
            } else
            if (Input.IsActionPressed("key_sprint")) {
                timerFootStep.Start(baseTime * 0.7f);
            } else {
                timerFootStep.Start(baseTime);
            }
        }
    }

    private void PlayFootStepSound() {
        audioFootStep.PlaySound();
    }

    private Vector3 Vector3Spheritize(Vector3 vec) {
        // For readability
        float x = Mathf.Abs(vec.x);
        float y = Mathf.Abs(vec.y);
        float z = Mathf.Abs(vec.z);
        
        // Gives the maximum of the absolute value of the three coordinates
        float mag = Mathf.Max(x, Mathf.Max(y,z));
        
        // Returns the vector with the set magnitude
        return vec.Normalized() * mag;
    }

    private bool IsSlopeClimbable(Vector3 normal) {
        float slopeAng = GetSlopeAngle(normal);
        float slopeMaxAng = MAX_SLOPE_ANGLE + 0.1f;// max slope ang + threshold

        return (slopeAng > 0f && slopeAng < slopeMaxAng);
    }

    private float GetSlopeAngle(Vector3 normal) {
        return Mathf.Acos(normal.Dot(Vector3.Up));
    }
}