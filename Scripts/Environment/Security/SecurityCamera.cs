using Godot;
using System;

public class SecurityCamera : IAlarmSetter { 
    [Export]
    public bool isRotating = true;
    [Export]
    public bool enabled = true;
    
    [Export(PropertyHint.Range, "-360,360,0.1")]
    public float horStartAngle = 0f;
    [Export(PropertyHint.Range, "-360,360,0.1")]
    public float verStartAngle = 0f;

    [Export]
    public float horTurnSpeed = 1f;
    [Export]
    public float turnWaitTime = 1f;

    [Export(PropertyHint.Range, "-360,360,0.1")]
    public float horAngleMin = -360f;
    [Export(PropertyHint.Range, "-360,360,0.1")]
    public float horAngleMax = 360f;

    [Export(PropertyHint.Range, "-360,360,0.1")]
    public float verAngleMin = -45f;
    [Export(PropertyHint.Range, "-360,360,0.1")]
    public float verAngleMax = 0f;

    [Export]
    public float calmDownTime = 5f;
    [Export]
    public float alertTime = 3f;
    [Export]
    public float alertCalmDownTime = 20f;

    private bool isBreaked = false;
    private bool isClockwise = true;
    private CameraState state = CameraState.idle;
    private bool isSeeingPlayer = false;

    public enum CameraState {
        idle, notice, alerted
    }

    private Color colorRed = Color.ColorN("red");
    private Color colorGreen = Color.ColorN("green");
    private Color colorOrange = Color.ColorN("orange");

    private Spatial headHorizontal;
    private Spatial headVertical;

    private Timer idleTimer;
    private Timer alarmTimer;
    private Timer calmDownTimer;

    private AudioStreamPlayer3D audioMotor;
    private AudioStreamPlayer3D audioDestroy;
    private AudioStreamPlayer3D audioAlarm;
    private AudioStreamPlayer3D audioNoContact;
    private AudioStreamPlayer3D audioNotice;
    private AudioStreamPlayer3D audioSetAlarm;

    private SpatialMaterial lightMat;

    private Player player;
    private RayCast rayCast;

    public override void _Ready() {
        GetNode<SecurityCameraCollision>("HeadHorizontal/HeadVertical/StaticBody").Connect(nameof(SecurityCameraCollision.SignalHit), this, nameof(CameraHit));
        horAngleMax = Mathf.Clamp(horAngleMax, horAngleMin, 360f);
        horAngleMin = Mathf.Clamp(horAngleMin, -360f, horAngleMax);
        
        verAngleMax = Mathf.Clamp(verAngleMax, verAngleMin, 360f);
        verAngleMin = Mathf.Clamp(verAngleMin, -360f, verAngleMax);
        
        horStartAngle = Mathf.Clamp(horStartAngle, horAngleMin, horAngleMax);
        verStartAngle = Mathf.Clamp(verStartAngle, verAngleMin, verAngleMax);

        headHorizontal = GetNode<Spatial>("HeadHorizontal");
        headVertical = GetNode<Spatial>("HeadHorizontal/HeadVertical");

        idleTimer = GetNode<Timer>("Timers/TimerWait");
        alarmTimer = GetNode<Timer>("Timers/TimerAlarm");
        calmDownTimer = GetNode<Timer>("Timers/TimerCalmDown");

        alarmTimer.Connect("timeout", this, nameof(SetOffAlarm));
        calmDownTimer.Connect("timeout", this, nameof(CalmDown));

        audioMotor = GetNode<AudioStreamPlayer3D>("Sounds/AudioMotorLoop");
        audioDestroy = GetNode<AudioStreamPlayer3D>("Sounds/AudioDestroy");
        audioAlarm = GetNode<AudioStreamPlayer3D>("Sounds/AudioAlarm");
        audioNoContact = GetNode<AudioStreamPlayer3D>("Sounds/AudioNoContact");
        audioNotice = GetNode<AudioStreamPlayer3D>("Sounds/AudioNotice");
        audioSetAlarm = GetNode<AudioStreamPlayer3D>("Sounds/AudioSetAlarm");

        headHorizontal.Rotation = new Vector3(0f, Mathf.Deg2Rad(horStartAngle), 0f);
        headVertical.Rotation = new Vector3(Mathf.Deg2Rad(verStartAngle), 0f, 0f);

        lightMat = (SpatialMaterial) GetNode<CSGCylinder>("HeadHorizontal/HeadVertical/CSGCylinder3").Material;

        player = Helper.GetPlayer();
        rayCast = GetNode<RayCast>("RayCast");
    }

	public override void _Process(float delta) {
        switch (state) {
            case CameraState.idle:
                lightMat.AlbedoColor = colorGreen;
                lightMat.Emission = colorGreen;
                if (audioAlarm.Playing) {
                    audioAlarm.Stop();
                }
                SecurityAlarmStatus.SetAlarmTime(0f);
            break;
            case CameraState.notice:
                lightMat.AlbedoColor = colorOrange;
                lightMat.Emission = colorOrange;
                SecurityAlarmStatus.SetAlarmTime(0f);
            break;
            case CameraState.alerted:
                lightMat.AlbedoColor = colorRed;
                lightMat.Emission = colorRed;
                SecurityAlarmStatus.SetAlarmTime(isSeeingPlayer ? 99.99f : calmDownTimer.TimeLeft);
            break;
        }
        LookForPlayer();

        if (isSeeingPlayer) {
            LookAtPlayer(delta);
            if (!audioAlarm.Playing) {
                // audioAlarm.Play();
            }
        } else {
            if (audioAlarm.Playing) {
                // audioAlarm.Stop();
            }
        }

        if (idleTimer.TimeLeft > 0 || isBreaked || !isRotating || horTurnSpeed == 0f || state != CameraState.idle) {
            if (! (state != CameraState.idle && isSeeingPlayer) ) {
                audioMotor.Stop();
            }
            return;
        }
        if (!audioMotor.Playing) {
            audioMotor.Play();
        }
        if (isClockwise) {
            headHorizontal.Rotation = new Vector3(0f, headHorizontal.Rotation.y + horTurnSpeed * delta, 0f);
            if (headHorizontal.Rotation.y > Mathf.Deg2Rad(horAngleMax)) {
                isClockwise = false;
                idleTimer.Start(turnWaitTime);
            }
        } else {
            headHorizontal.Rotation = new Vector3(0f, headHorizontal.Rotation.y - horTurnSpeed * delta, 0f);
            if (headHorizontal.Rotation.y < Mathf.Deg2Rad(horAngleMin)) {
                isClockwise = true;
                idleTimer.Start(turnWaitTime);
            }
        }
	}

    private void LookForPlayer() {
        if (isBreaked) return;
        if (player is null) return;
        if (this.GlobalTransform.origin.DistanceTo(player.GlobalTransform.origin) > 100f) return;
        Vector3 rayDirection = (player.GlobalTransform.origin + new Vector3(0f, 1f, 0f)) - this.GlobalTransform.origin;
        rayCast.CastTo = rayDirection;
        rayCast.ForceRaycastUpdate();

        bool doSeePlayer = false;
        if (rayCast.IsColliding()) {
            if (rayCast.GetCollider() is Player) {
                Vector3 cameraAngle = -headVertical.GlobalTransform.basis.z;
                Vector3 playerAngle = headVertical.GlobalTransform.origin.DirectionTo(player.GlobalTransform.origin);
                if (cameraAngle.Dot(playerAngle) > 0.75) {
                    doSeePlayer = true;
                    if (state == CameraState.idle) {
                        state = CameraState.notice;
                        audioNotice.Play();
                    }
                }
            }
        }
        if (state == CameraState.alerted && doSeePlayer == true && isSeeingPlayer == false) {
            this.EmitSignal(nameof(AlarmSignal), new object[]{});
        }
        isSeeingPlayer = doSeePlayer;
        if (isSeeingPlayer && state == CameraState.notice && alarmTimer.TimeLeft == 0f) {
            alarmTimer.Start(alertTime);
        }
        if (isSeeingPlayer && state == CameraState.notice) {
            calmDownTimer.Stop();
            calmDownTimer.Start(calmDownTime);
        }
        if (isSeeingPlayer && alarmTimer.Paused == true) {
            alarmTimer.Paused = false;
        }
        if (!isSeeingPlayer && alarmTimer.TimeLeft != 0f) {
            alarmTimer.Paused = true;
        }
        if (!isSeeingPlayer && state == CameraState.alerted && calmDownTimer.TimeLeft == 0f) {
            calmDownTimer.Stop();
            calmDownTimer.Start(alertCalmDownTime);
        }
        if (isSeeingPlayer && state == CameraState.alerted) {
            calmDownTimer.Stop();
        }
    }

    private void LookAtPlayer(float delta) {
        if (isBreaked) return;
        Vector3 cameraAngle = -headHorizontal.GlobalTransform.basis.z;
        Vector3 playerAngle = (headHorizontal.GlobalTransform.origin * new Vector3(1f, 0f, 1f)).DirectionTo(player.GlobalTransform.origin * new Vector3(1f, 0f, 1f));

        float angle = SignedAngleTo(cameraAngle, playerAngle, Vector3.Up);

        if (Mathf.Abs(angle) > horTurnSpeed * delta * 1.5f) {
            if (angle > 0f) {
                if (headHorizontal.Rotation.y < Mathf.Deg2Rad(horAngleMax)) {
                    headHorizontal.Rotation = new Vector3(0f, headHorizontal.Rotation.y + horTurnSpeed * delta, 0f);
                    if (!audioMotor.Playing) { audioMotor.Play(); }
                } else {
                    if (audioMotor.Playing) { audioMotor.Stop(); }
                }
            } else {
                if (headHorizontal.Rotation.y > Mathf.Deg2Rad(horAngleMin)) {
                    headHorizontal.Rotation = new Vector3(0f, headHorizontal.Rotation.y - horTurnSpeed * delta, 0f);
                    if (!audioMotor.Playing) { audioMotor.Play(); }
                } else {
                    if (audioMotor.Playing) { audioMotor.Stop(); }
                }
            }
        } else {
            if (audioMotor.Playing) {
                audioMotor.Stop();
            }
        }
    }

    private float SignedAngleTo(Vector3 u, Vector3 v, Vector3 axis) {
        float dir = u.Cross(v).Dot(axis);
        float dot = u.Dot(v);
        float angle = Mathf.Acos(dot);
        if (dir > 0f) {
            return angle;
        } else {
            return -angle;
        }
    }

    private void SetOffAlarm() {
        if (state == CameraState.notice && isSeeingPlayer == true) {
            state = CameraState.alerted;
            calmDownTimer.Stop();
            this.EmitSignal(nameof(AlarmSignal), new object[]{});
            audioSetAlarm.Play();
        }
    }

    private void CalmDown() {
        if (isSeeingPlayer == false) {
            state = CameraState.idle;
            audioNoContact.Play();
            alarmTimer.Paused = false;
            alarmTimer.Stop();
        }
    }

    public void CameraHit() {
        if (isBreaked) return;
        isBreaked = true;
        GetNode<CollisionShape>("HeadHorizontal/HeadVertical/StaticBody/CollisionShape").Disabled = true;
        GetNode<Spatial>("HeadHorizontal").Visible = false;
        GetNode<Particles>("Particles").Emitting = true;
        audioDestroy.Play();
        audioMotor.Stop();
        audioNoContact.Stop();
        audioAlarm.Stop();
        audioSetAlarm.Stop();
        audioNotice.Stop();

        alarmTimer.Stop();
        idleTimer.Stop();
        calmDownTimer.Stop();
        state = CameraState.idle;
        
        SecurityAlarmStatus.SetAlarmTime(0f);
    }
}