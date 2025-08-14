#version 430

layout(points) in;
layout(line_strip, max_vertices = 16) out;



in VertBox {
	vec3 Color;
	vec4 Corners[8];
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} gs_out;



void EmitCorner(int i)
{
	gs_out.Color = vec3(1, 0, 0);
	gl_Position = gs_in[i].Corners[0];	EmitVertex();
	gl_Position = gs_in[i].Corners[1];	EmitVertex();
	EndPrimitive();

	gs_out.Color = vec3(0, 1, 0);
	gl_Position = gs_in[i].Corners[2];	EmitVertex();
	gl_Position = gs_in[i].Corners[3];	EmitVertex();
	EndPrimitive();

	gs_out.Color = vec3(0, 0, 1);
	gl_Position = gs_in[i].Corners[4];	EmitVertex();
	gl_Position = gs_in[i].Corners[5];	EmitVertex();
	EndPrimitive();
}

/*
	2 - - - - - 3
	| \       / |
	|	6 - 7	|
	|	|	|	|
	|	4 - 5	|
	| /       \ |
	0 - - - - - 1
*/

void main()
{
	gs_out.Normal = vec3(0, 0, 0);
	gs_out.Absolut = vec3(0, 0, 0);
	gs_out.Color = gs_in[0].Color;

	vec4 corners[8] = gs_in[0].Corners;

	gl_Position = corners[0];	EmitVertex();
	gl_Position = corners[1];	EmitVertex();
	gl_Position = corners[3];	EmitVertex();
	gl_Position = corners[2];	EmitVertex();
	EndPrimitive();

	gl_Position = corners[4];	EmitVertex();
	gl_Position = corners[0];	EmitVertex();
	gl_Position = corners[2];	EmitVertex();
	gl_Position = corners[6];	EmitVertex();
	EndPrimitive();

	gl_Position = corners[5];	EmitVertex();
	gl_Position = corners[4];	EmitVertex();
	gl_Position = corners[6];	EmitVertex();
	gl_Position = corners[7];	EmitVertex();
	EndPrimitive();

	gl_Position = corners[1];	EmitVertex();
	gl_Position = corners[5];	EmitVertex();
	gl_Position = corners[7];	EmitVertex();
	gl_Position = corners[3];	EmitVertex();
	EndPrimitive();
}
