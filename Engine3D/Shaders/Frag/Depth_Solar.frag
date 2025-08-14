#version 430

uniform float[7] depthFactor;

uniform float[3] depthFadeRange = { 0.8, 0.2, 1.0 };
uniform vec3 depthFadeColor = vec3(0.5, 0.5, 0.5);

uniform vec3 solar = normalize(vec3(-1, +3, -2));
uniform float[3] lightRange = { 0.1, 0.9, 1.0 };

uniform float[2] colorInterPol = { 1.0, 0.0 };
uniform vec3 colorOther = vec3(1, 1, 1);



in Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} fs_in;

out vec4 Color;

/*
	get relative in here
	calculate langth of relative to get distance ?
	sqrt is inefficient, so slightly slower
	bit do it for fun ?
*/

void main()
{
	float depth_factor;
	depth_factor = gl_FragDepth;
	depth_factor = depthFactor[4] / (depthFactor[3] - depth_factor * depthFactor[2]);
	depth_factor = (depth_factor - depthFactor[0]) / depthFactor[1];

	depth_factor = (depth_factor - depthFadeRange[0]) / depthFadeRange[1];
	gl_FragDepth = depth_factor;
	depth_factor = max(depth_factor, 0);

	float solar_factor;
	solar_factor = dot(solar, normalize(fs_in.Normal));

	float light_factor = solar_factor;
	//light_factor = abs(light_factor);	//	lights from both sides
	light_factor = min(max(light_factor, lightRange[0]), lightRange[2]);



	vec3 col;
	col = (colorInterPol[0] * fs_in.Color) + (colorInterPol[1] * colorOther);
	col = col * light_factor;
	col = (col * (1.0 - depth_factor)) + (depthFadeColor * depth_factor);

	Color = vec4(col, 1);
}
