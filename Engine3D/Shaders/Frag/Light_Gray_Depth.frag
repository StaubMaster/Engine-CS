#version 430

//const vec3 solar = normalize(vec3(-1, +3, -2));
uniform vec3 solar;
const float lightMin = 0.25;

uniform float[7] depthFactor;



in Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} fs_in;

out vec4 Color;



void main()
{
	float fak;
	fak = dot(solar, normalize(fs_in.Normal));
	fak = max(abs(fak), lightMin);

	float dep;
	dep = gl_FragDepth;
	dep = depthFactor[4] / (depthFactor[3] - dep * depthFactor[2]);
	dep = (dep - depthFactor[0]) / depthFactor[1];

	dep = (dep - 0.8) / 0.2;
	dep = max(dep, 0);

	vec3 col = fs_in.Color * fak;
	float sum = (col.r + col.g + col.b) / 3;
	col = vec3(sum, sum, sum);

	//col = col * (1.0 - dep) + (vec3(1, 1, 1) * dep);

	Color = vec4(col, 1);
}
