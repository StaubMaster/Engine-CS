#version 430

layout(points) in;
layout(line_strip, max_vertices = 2) out;



in VertTP {
	vec3		Color;
	vec3[2]		Original;
	vec3[2]		Absolute;
	vec3[2]		Relative;
	vec4[2]		Proj;
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Original;
	vec3 Absolute;
	vec3 Relative;
} gs_out;



void main()
{
	gs_out.Color = gs_in[0].Color;

	gs_out.Original = gs_in[0].Original[0];
	gs_out.Absolute = gs_in[0].Absolute[0];
	gs_out.Relative = gs_in[0].Relative[0];
	gl_Position = gs_in[0].Proj[0];
	EmitVertex();

	gs_out.Original = gs_in[0].Original[1];
	gs_out.Absolute = gs_in[0].Absolute[1];
	gs_out.Relative = gs_in[0].Relative[1];
	gl_Position = gs_in[0].Proj[1];
	EmitVertex();

	EndPrimitive();
}
