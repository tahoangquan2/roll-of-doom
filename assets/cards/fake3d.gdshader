// Hey this is Hei! This shader "fakes" a 3D-camera perspective on CanvasItems.
// License: MIT

shader_type canvas_item;

// Camera FOV
uniform float fov : hint_range(1, 179) = 90;
uniform bool cull_back = true;
uniform float y_rot : hint_range(-180, 180) = 0.0;
uniform float x_rot : hint_range(-180, 180) = 0.0;
// At 0, the image retains its size when unrotated.
// At 1, the image is resized so that it can do a full
// rotation without clipping inside its rect.
uniform float inset : hint_range(0, 1) = 0.0;
// Consider changing this to a uniform and changing it fr

// add option for super sampling (2x2, 3x3, none)

uniform int super_sampling : hint_range(1, 3) = 2;

varying flat vec2 o;
varying vec3 p;

// Creates rotation matrix
void vertex(){
	float sin_b = sin(y_rot / 180.0 * PI);
	float cos_b = cos(y_rot / 180.0 * PI);
	float sin_c = sin(x_rot / 180.0 * PI);
	float cos_c = cos(x_rot / 180.0 * PI);
	
	mat3 inv_rot_mat;
	inv_rot_mat[0][0] = cos_b;
	inv_rot_mat[0][1] = 0.0;
	inv_rot_mat[0][2] = -sin_b;
	
	inv_rot_mat[1][0] = sin_b * sin_c;
	inv_rot_mat[1][1] = cos_c;
	inv_rot_mat[1][2] = cos_b * sin_c;
	
	inv_rot_mat[2][0] = sin_b * cos_c;
	inv_rot_mat[2][1] = -sin_c;
	inv_rot_mat[2][2] = cos_b * cos_c;
	
	
	float t = tan(fov / 360.0 * PI);
	p = inv_rot_mat * vec3((UV - 0.5), 0.5 / t);
	float v = (0.5 / t) + 0.5;
	p.xy *= v * inv_rot_mat[2].z;
	o = v * inv_rot_mat[2].xy;

	VERTEX += (UV - 0.5) / TEXTURE_PIXEL_SIZE * t * (1.0 - inset);
}

void fragment() {
    if (cull_back && p.z <= 0.0) discard;
    
    vec2 uv = (p.xy / p.z).xy - o;

    // Define sampling offsets 
	vec4 color = vec4(0.0);
	if (super_sampling == 1) {//none
		color = texture(TEXTURE, uv + 0.5);
	} else if (super_sampling == 2) {//2x2
		// Define sampling offsets (a 2x2 grid)
		vec2 offsets[4] = vec2[](
			vec2(-0.5, -0.5), 
			vec2( 0.5, -0.5), 
			vec2(-0.5,  0.5), 
			vec2( 0.5,  0.5)
		);

		float weight = 0.25; // Each sample contributes equally

		for (int i = 0; i < 4; i++) {
			vec2 sample_uv = uv + offsets[i] * TEXTURE_PIXEL_SIZE * 0.5; // Adjust spread factor
			color += texture(TEXTURE, sample_uv + 0.5) * weight;
		}
	} else if (super_sampling == 3) {//3x3
		vec2 offsets[9] = vec2[](
			vec2(-1.0, -1.0), vec2( 0.0, -1.0), vec2( 1.0, -1.0),
			vec2(-1.0,  0.0), vec2( 0.0,  0.0), vec2( 1.0,  0.0),
			vec2(-1.0,  1.0), vec2( 0.0,  1.0), vec2( 1.0,  1.0)
		);

		
		float weight = 1.0 / 9.0; // Proper weight for 9 samples

		for (int i = 0; i < 9; i++) {
			vec2 sample_uv = uv + offsets[i] * TEXTURE_PIXEL_SIZE * 0.33; // Adjust spread factor
			color += texture(TEXTURE, sample_uv + 0.5) * weight;
		}
	}

    COLOR = color;

    // Apply smooth alpha blending to avoid hard clipping
    COLOR.a *= step(max(abs(uv.x), abs(uv.y)), 0.5);
}
