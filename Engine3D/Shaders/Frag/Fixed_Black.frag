#version 430

uniform float[7] depthFactor;



in Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} fs_in;

out vec4 Color;



void main()
{
	vec3 col;
	col = vec3(0.0, 0.0, 0.0);
	Color = vec4(col, 1);
}
