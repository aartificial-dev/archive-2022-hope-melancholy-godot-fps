using Godot;
using System;

[Tool]
public class ProceduralSkyEnvFull : WorldEnvironment { 
    [Export]
    private NodePath DirectionalLightPath {set {directionalLightPath = value; UpdatePath();} get {return directionalLightPath;}}
    [Export]
    public GradientTexture sunGradient;
    [Export]
    public GradientTexture skyTopGradient;
    [Export]
    public GradientTexture skyHorizonGradient;
    [Export]
    public GradientTexture skyLightGradient;
    [Export]
    public GradientTexture skyShadowGradient;

    private NodePath directionalLightPath = "";

    private DirectionalLight light = null;
    private Vector3 lightRot = Vector3.Zero;

    private ProceduralSky sky = null;

    public override void _Ready() {
        Godot.Environment env = this.Environment;
        if (env.BackgroundMode != Godot.Environment.BGMode.Sky) return;
        if (env.BackgroundSky is null) return;
        if (!(env.BackgroundSky is ProceduralSky)) return;
        sky = (ProceduralSky) env.BackgroundSky;

        UpdatePath();
    }

	public override void _Process(float delta) {
        if (light is null || sky is null) return;

        Vector3 rot = light.RotationDegrees;

        if (lightRot != rot) {
            UpdateSky(rot);
            lightRot = rot;
        }
	}

    private void UpdateSky(Vector3 rot) {
        Godot.Environment env = this.Environment;
        
        float longitude = 0f; // y rotation, hor

        // 0 to 180 and -180 to 0; -180 to 180, full sky cycle.
        // + 180 = 0 to 360; div 360 = 0 to 1; gradient mapping where 0 sunset .5 sunrise 1 sunset
        float latitude = 0f;  // x rotation, ver 
        float x = rot.x; // Mathf.Rad2Deg(rot.x);
        float y = rot.y; // Mathf.Rad2Deg(rot.y);
        longitude = (y >= 0f) ? (180 - y) : (-180 - y);
        latitude = -x;

        sky.SunLongitude = longitude;
        sky.SunLatitude = latitude;
        
        // 0 - sunset .5 sunrize 1 sunset
        float map = Mathf.Wrap(x - 90, 0f, 360f) / 360f;  

        sky.SunColor = sunGradient.Gradient.Interpolate(map);
        sky.SkyTopColor = skyTopGradient.Gradient.Interpolate(map);
        sky.SkyHorizonColor = skyHorizonGradient.Gradient.Interpolate(map);

        sky.GroundHorizonColor = sky.SkyHorizonColor;
        // sky.GroundBottomColor = sky.SkyHorizonColor;

        env.FogColor = sky.SkyHorizonColor;
        env.FogSunColor = sky.SunColor;
        
        light.LightColor = skyLightGradient.Gradient.Interpolate(map);
        light.ShadowColor = skyShadowGradient.Gradient.Interpolate(map);
        
        float lightEnergy = 1f - Mathf.Abs(map - 0.5f) / 0.5f;
        lightEnergy = Mathf.Clamp((lightEnergy - 0.5f) * 2f, 0f, 1f);
        // GD.Print(lightEnergy);
        light.LightEnergy = lightEnergy;

        float maxSunSize = 0.05f;
        float minSunSize = 0.002f;
        float maxSunAngle = 0.2f;
        float curveOffset = maxSunAngle / 10f;
        float curveNorm = maxSunSize / (maxSunSize - curveOffset);

        float mapOffset = Mathf.Clamp(map, maxSunAngle, 1f - maxSunAngle);
        float sunMap = 1f - (Mathf.Abs(0.5f - mapOffset) / 0.5f);
        
        float sunSize = (maxSunSize * sunMap - curveOffset) * curveNorm;

        sky.SunCurve = Mathf.Max( sunSize, minSunSize );
    }

    private void UpdatePath() {
        if (directionalLightPath.IsEmpty() || !(GetNode(directionalLightPath) is DirectionalLight)) {
            return;
        }

        DirectionalLight light = GetNode<DirectionalLight>(directionalLightPath);
        this.light = light;
    }
}