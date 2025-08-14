#version 430

layout(points) in;
layout(line_strip, max_vertices = 24) out;



in VertBox {
	vec3		Color;
	vec3[8]		Absolut;
	vec4[8]		Proj;
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} gs_out;



void EmitCorn(int i)
{
	gs_out.Absolut = gs_in[0].Absolut[i];
	gl_Position = gs_in[0].Proj[i];
	EmitVertex();
}

void EmitLine(int i0, int i1, vec3 col)
{
	gs_out.Color = col;
	EmitCorn(i0);
	EmitCorn(i1);
	EndPrimitive();
}

void main()
{
	gs_out.Color = gs_in[0].Color;

	EmitLine(0, 1, vec3(1, 0, 0));
	EmitLine(2, 3, vec3(1, 0, 0));
	EmitLine(4, 5, vec3(1, 0, 0));
	EmitLine(6, 7, vec3(1, 0, 0));

	EmitLine(0, 3, vec3(0, 1, 0));
	EmitLine(1, 2, vec3(0, 1, 0));
	EmitLine(4, 7, vec3(0, 1, 0));
	EmitLine(5, 6, vec3(0, 1, 0));

	EmitLine(0, 4, vec3(0, 0, 1));
	EmitLine(1, 5, vec3(0, 0, 1));
	EmitLine(2, 6, vec3(0, 0, 1));
	EmitLine(3, 7, vec3(0, 0, 1));
}
