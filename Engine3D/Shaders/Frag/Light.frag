#version 430

const vec3 solar = normalize(vec3(-1, +3, -2));
const float lightMin = 0.25;



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

	Color = vec4(fs_in.Color * fak, 1);
}
