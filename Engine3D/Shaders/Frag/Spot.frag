#version 430

uniform int full;
uniform vec3 light_pos;
uniform vec3 light_rch;



in Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} fs_in;

out vec4 Pixel;



void main()
{
	if (full != 0)
	{
		Pixel = vec4(fs_in.Color, 1);
		//float h = fs_in.NonProj.y;
		//h = h / 100.0;
		//if (h > 0.3)
		//	Pixel = vec4(0, h, 0, 1);
		//else
		//	Pixel = vec4(h, 0, 0, 1);
		//Pixel = vec4(h, h, h, 1);
		return;
	}


	vec3 norm, rel_light;
	norm = normalize(fs_in.Normal);
	rel_light = normalize(light_pos - fs_in.Absolut);

	float fak_cone, fak_norm;
	fak_cone = dot(rel_light, -light_rch);
	if (fak_cone < 0.85)
		fak_cone = 0.0;
	else if (fak_cone > 0.95)
		fak_cone = 1.0;
	else
	{
		fak_cone -= 0.85;
		fak_cone /= 0.10;
	}
	//fak_norm = max(dot(light_rch, norm), 0.25);
	fak_norm = max(dot(-rel_light, norm), 0.1);

	Pixel = vec4(fs_in.Color * fak_cone * fak_norm, 1);
}
