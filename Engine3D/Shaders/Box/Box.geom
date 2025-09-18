#version 430

layout(points) in;
layout(line_strip, max_vertices = 16) out;



in VertBox {
	vec3		Color;
	vec3[8]		Absolute;
	vec3[8]		Relative;
	vec4[8]		Position;
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} gs_out;



void EmitCorn(int i)
{
	gs_out.Absolut = gs_in[0].Absolute[i];
	gl_Position = gs_in[0].Position[i];
	EmitVertex();
}

void EmitQuad(int i0, int i1, int i2, int i3)
{
	EmitCorn(i0);
	EmitCorn(i1);
	EmitCorn(i2);
	EmitCorn(i3);
	EndPrimitive();
}

void main()
{
	gs_out.Color = gs_in[0].Color;

	EmitQuad(0, 1, 5, 4);
	EmitQuad(1, 2, 6, 5);
	EmitQuad(2, 3, 7, 6);
	EmitQuad(3, 0, 4, 7);
}
