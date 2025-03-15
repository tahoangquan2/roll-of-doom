shader_type canvas_item;

uniform float speed = 0.0;
varying vec4 modulate;

void vertex() {
	modulate = COLOR;
}

void fragment(){
	vec2 move=vec2(1.0,0.0)*TIME*-speed;
	COLOR = texture(TEXTURE,UV+move)*modulate;
}
