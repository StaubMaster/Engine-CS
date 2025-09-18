#version 430

layout(triangles) in;
layout(triangle_strip, max_vertices = 3) out;



in VertUI {
	vec3 Original;
	vec3 Absolute;
	vec3 Relative;

	vec3 Normal;
	float Tex;
} gs_in[];

out GeomUI {
	vec3 Normal;
	vec2 Tex;

	vec3 Original;
	vec3 Absolute;
	vec3 Relative;
} gs_out;



void EmitCorner(int i)
{
	gs_out.Original = gs_in[i].Original;
	gs_out.Absolute = gs_in[i].Absolute;
	gs_out.Relative = gs_in[i].Relative;

	gs_out.Normal = gs_in[i].Normal;
	gs_out.Tex = vec2(gs_in[i].Tex, 0);

	gl_Position = gl_in[i].gl_Position;
	EmitVertex();
}

void main()
{
	EmitCorner(0);
	EmitCorner(1);
	EmitCorner(2);
	EndPrimitive();
}
