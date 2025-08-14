#version 430

const vec3 solar = normalize(vec3(-1, +3, -2));
//uniform vec3 solar;

const float lightMin = 0.1;



in Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} fs_in;

out vec4 Color;



void main()
{
	float solar_factor;
	solar_factor = dot(solar, normalize(fs_in.Normal));

	float light_factor = solar_factor;
	light_factor = max(abs(light_factor), lightMin);	//	lights from both sides
	//light_factor = max(light_factor, lightMin);

	vec3 col = fs_in.Color;
	col = col * light_factor;

	Color = vec4(col, 1);
}
