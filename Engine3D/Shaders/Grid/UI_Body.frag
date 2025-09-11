#version 430

//uniform float[7]	depthFactor;
//uniform float[3]	depthFadeRange = { 0.8, 0.2, 1.0 };
//uniform vec3		depthFadeColor = vec3(0.5, 0.5, 0.5);

uniform vec3		solar = normalize(vec3(-1, +3, -2));
uniform float[3]	lightRange = { 0.1, 0.9, 1.0 };

uniform float[2]	colorInterPol = { 1.0, 0.0 };
uniform vec3		colorOther = vec3(1, 1, 1);

uniform float[2]	GrayInter = { 1.0, 0.0 };



in GeomUI {
	vec3 Color;
	vec3 Normal;

	vec3 Original;
	vec3 Absolute;
	vec3 Relative;
} fs_in;

out vec4 Color;



void main()
{
	float depth_factor;

	//	Normal Depth
	//depth_factor = depthFactor[4] / (depthFactor[3] - gl_FragDepth * depthFactor[2]);

	//	Distance Depth
	depth_factor = length(fs_in.Relative);

	//depth_factor = (depth_factor - depthFactor[0]) / depthFactor[1];
	//gl_FragDepth = depth_factor;

	//depth_factor = (depth_factor - depthFadeRange[0]) / depthFadeRange[1];
	//depth_factor = min(max(depth_factor, 0), 1);



	float solar_factor;
	solar_factor = dot(solar, normalize(fs_in.Normal));

	float light_factor = solar_factor;
	//light_factor = abs(light_factor);	//	lights from both sides
	light_factor = min(max(light_factor, lightRange[0]), lightRange[2]);



	//	other color
	vec3 col = fs_in.Color;

	//col = (col * colorInterPol[0]) + (colorInterPol[1] * colorOther);
	//col = (col * fs_in.AltColLInter[0]) + (fs_in.AltColLInter[1] * fs_in.AltColor);

	//	gray
	//col = (col * GrayInter[0]) + (GrayInter[1] * vec3((col.r + col.g + col.b) / 3));
	//col = (col * fs_in.GrayLInter[0]) + (fs_in.GrayLInter[1] * vec3((col.r + col.g + col.b) / 3));

	//	light
	//col = col * light_factor;

	//	depth
	//col = (col * (1.0 - depth_factor)) + (depth_factor * depthFadeColor);



	Color = vec4(col, 1);
}
