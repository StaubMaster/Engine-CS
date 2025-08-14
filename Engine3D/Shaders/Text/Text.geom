#version 430

layout(points) in;
layout(line_strip, max_vertices = 8) out;



const float charNorm = 0.1;
const vec2 charRatio = vec2(0.5, 1.0);
const vec2 screenRatio = vec2(0.5, 1.0);
const float charScale = 0.0125 * 2;
const vec2 scale = charRatio * screenRatio * charNorm * charScale;



in Text_Char {
	vec2 Pos;
	vec3 Color;
	ivec2 pallet[8];
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} gs_out;



void emit_vec(ivec2 v)
{
	vec2 p;
	p = gs_in[0].Pos + (v * scale);
	gl_Position = vec4(p, 0, 1);
	EmitVertex();
}

void emit_char(ivec2[8] corners)
{
	int i = 0;
	while (i < 8)
	{
		ivec2 c = corners[i];
		if (c.x == 255)
		{
			EndPrimitive();
			if (c.y == 255)
				return;
		}
		else
		{
			emit_vec(c);
		}
		i++;
	}
	EndPrimitive();
}

void main()
{
	gs_out.Color = gs_in[0].Color;
	gs_out.Absolut = vec3(0, 0, 0);
	gs_out.Normal = vec3(0, 0, 0);

	emit_char(gs_in[0].pallet);
}
