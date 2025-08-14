#version 430

uniform float[7] depthFactor;
const float depthFadeBoarder = 0.8;
const float depthFadeSize = 0.2;	// 1.0 - depthFadeBoarder
const vec3 depthFadeColor = vec3(0.5, 0.5, 0.5);



in Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} fs_in;

out vec4 Color;



void main()
{
	float depth_factor;
	depth_factor = gl_FragDepth;
	depth_factor = depthFactor[4] / (depthFactor[3] - depth_factor * depthFactor[2]);
	depth_factor = (depth_factor - depthFactor[0]) / depthFactor[1];

	depth_factor = (depth_factor - depthFadeBoarder) / depthFadeSize;
	depth_factor = max(depth_factor, 0);

	vec3 col = fs_in.Color;
	col = col * (1.0 - depth_factor) + (depthFadeColor * depth_factor);

	Color = vec4(col, 1);
}
