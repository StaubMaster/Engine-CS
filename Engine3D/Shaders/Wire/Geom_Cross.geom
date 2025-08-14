#version 430

layout(triangles) in;
layout(line_strip, max_vertices = 18) out;

layout(std430, binding = 0) buffer BufferColor {
	uint color[];
};



in VertCross {
	vec4 Corners[6];
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
	# - - - - - #
	| \       / |
	|	# - #	|
	|	|	|	|
	|	# - #	|
	| /       \ |
	# - - - - - #
*/

void main()
{
	gs_out.Normal = vec3(0, 0, 0);
	gs_out.Absolut = vec3(0, 0, 0);

	EmitCorner(0);
	EmitCorner(1);
	EmitCorner(2);
}
