#version 430

layout(points) in;
layout(line_strip, max_vertices = 2) out;



in VertTP {
	vec3		Color;
	vec3[2]		Absolut;
	vec4[2]		Proj;
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} gs_out;



void main()
{
	gs_out.Color = gs_in[0].Color;
	gs_out.Normal = vec3(0);

	gs_out.Absolut = gs_in[0].Absolut[0];
	gl_Position = gs_in[0].Proj[0];
	EmitVertex();

	gs_out.Absolut = gs_in[0].Absolut[1];
	gl_Position = gs_in[0].Proj[1];
	EmitVertex();

	EndPrimitive();
}
