#version 430

in Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} fs_in;

out vec4 Color;



void main()
{
	Color = vec4(fs_in.Color, 1);
}
