shader_type canvas_item; 

uniform float scale: hint_range(0.01, 10.) = 1.;
uniform float strength: hint_range(0.01, 1.) = 0.05;

void fragment() {
	float ps = UV.x / (TEXTURE_PIXEL_SIZE.x * scale);
	vec4 color = texture(TEXTURE, UV);
	
	int pp = int(mod(ps, 3.));
	vec4 outcolor = vec4(0, 0, 0, 1);
	if (pp == 1) outcolor.r = color.r;
	    else if (pp == 2) outcolor.g = color.g;
	        else outcolor.b = color.b;
	// return outcolor;

	
	COLOR = mix(color, outcolor, strength);
}