#version 430

layout(triangles) in;
layout(line_strip, max_vertices = 4) out;

layout(std430, binding = 0) buffer BufferColor {
	uint color[];
};


in Vert {
	vec3 Absolut;
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} gs_out;



void EmitCorner(int i)
{
	gs_out.Absolut = gs_in[i].Absolut;
	gl_Position = gl_in[i].gl_Position;
	EmitVertex();
}

void main()
{
	gs_out.Normal = vec3(0, 0, 0);
	gs_out.Absolut = vec3(0, 0, 0);
	gs_out.Color = vec3(1, 1, 1);

	EmitCorner(0);
	EmitCorner(1);
	EmitCorner(2);
	EmitCorner(0);
	EndPrimitive();
}
