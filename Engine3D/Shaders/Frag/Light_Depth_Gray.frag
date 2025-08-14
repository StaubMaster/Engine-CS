#version 430

const vec3 solar = normalize(vec3(-1, +3, -2));
//uniform vec3 solar;
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
	//vec3 light_rel;
	//light_rel = vec3(-10, +15, -10) - fs_in.Absolut;

	float fak;
	fak = dot(solar, normalize(fs_in.Normal));
	fak = max(abs(fak), lightMin);
	//fak = dot(normalize(light_rel), normalize(fs_in.Normal));
	//fak = max(fak, lightMin);

	float dep;
	dep = gl_FragDepth;
	dep = depthFactor[4] / (depthFactor[3] - dep * depthFactor[2]);
	dep = (dep - depthFactor[0]) / depthFactor[1];
	gl_FragDepth = dep;

	dep = (dep - 0.8) / 0.2;
	dep = max(dep, 0);

	vec3 col = fs_in.Color * fak;
	col = col * (1.0 - dep) + (vec3(0.5, 0.5, 0.5) * dep);

	float sum = (col.r + col.g + col.b) / 3;
	Color = vec4(sum, sum, sum, 1);
}
