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
	float dep;
	dep = gl_FragDepth;
	dep = depthFactor[4] / (depthFactor[3] - dep * depthFactor[2]);
	dep = (dep - depthFactor[0]) / depthFactor[1];
	dep = 1.0 - dep;

	vec3 col;
	col = vec3(dep, dep, dep);
	Color = vec4(col, 1);
}
