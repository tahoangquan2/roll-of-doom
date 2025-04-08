shader_type canvas_item;
render_mode blend_premul_alpha;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;

uniform float Line_Smoothness : hint_range(0, 0.1) = 0.045;
uniform float Line_Width : hint_range(0, 0.2) = 0.09;
uniform float Brightness = 3.0;
uniform float Rotation_deg : hint_range(-90, 90) = 30;
uniform float Distortion : hint_range(1, 2) = 1.8;
uniform float Speed = 0.7;
uniform float Position : hint_range(0, 1) = 0;
uniform float Position_Min = 0.25;
uniform float Position_Max = 0.5;
uniform float Alpha : hint_range(0, 1) = 1;
