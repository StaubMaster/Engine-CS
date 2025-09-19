#version 430

uniform float[7]	depthFactor;



in Geom {
	vec3 Color;
	vec3 Original;
	vec3 Absolute;
	vec3 Relative;
} fs_in;

out vec4 Color;



void main()
{
	float depth_factor;
	depth_factor = length(fs_in.Relative);

	depth_factor = (depth_factor - depthFactor[0]) / depthFactor[1];
	gl_FragDepth = depth_factor;

	Color = vec4(fs_in.Color, 1);
}
