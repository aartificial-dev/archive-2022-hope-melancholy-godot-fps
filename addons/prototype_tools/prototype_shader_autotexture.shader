shader_type spatial;

render_mode world_vertex_coords;

uniform sampler2D top_texture;
uniform sampler2D side_texture;
uniform vec3 world_direction = vec3(0.0, 1.0, 0.0);
uniform float max_angle: hint_range(0.,3.14) = 0.795;
uniform float scale = 1.;

varying vec3 world_pos;
varying vec3 world_nor;

varying vec3 triplanar_pos;
varying vec3 power_normal;
varying vec3 world_normal;
varying vec3 object_normal;

void vertex() {
	world_pos = VERTEX;
	world_nor = NORMAL;
	
	world_normal = vec3(0, 1.0, 0);
	object_normal = NORMAL;
	
	power_normal = pow(abs(NORMAL), vec3(10.0));
	power_normal = normalize(power_normal);
	
	triplanar_pos = VERTEX.xyz * vec3(1.0, -1.0, 1.0);
}

vec4 triplanar_texture(sampler2D p_sampler, vec3 p_weights, vec3 p_triplanar_pos) {
	vec4 samp = vec4(0.0);
	samp += texture(p_sampler, p_triplanar_pos.xy / scale) * p_weights.z;
	samp += texture(p_sampler, p_triplanar_pos.xz / scale) * p_weights.y;
	samp += texture(p_sampler, p_triplanar_pos.zy / scale * vec2(-1.0,1.0)) * p_weights.x;
	return samp;
}

void fragment() {
	vec3 pos = world_pos;
	float mix_factor = 0.;
	bool is_side = false;
	vec3 world_norm = NORMAL * mat3(INV_CAMERA_MATRIX);
	is_side = acos(abs(dot(world_norm, world_direction))) > max_angle;
	if (is_side) {
		ALBEDO = triplanar_texture(side_texture, power_normal, triplanar_pos).rgb;
		//ALBEDO = texture(side_texture, UV).rgb;
	} else {
		ALBEDO = triplanar_texture(top_texture, power_normal, triplanar_pos).rgb;
		//ALBEDO = texture(top_texture, UV).rgb;
	}
}
