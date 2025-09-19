#version 430

layout(triangles) in;
layout(line_strip, max_vertices = 3) out;

layout(std430, binding = 0) buffer BufferColor {
	uint color[];
};



in VertInst {
	vec3 Original;
	vec3 Absolute;
	vec3 Relative;

	vec3 Normal;
	float Tex;

	vec3 AltColor;
	float[2] AltColLInter;
	float[2] GrayLInter;
} gs_in[];

out GeomInst {
	vec3 Original;
	vec3 Absolute;
	vec3 Relative;

	vec3 Color;

	vec3 AltColor;
	float[2] AltColLInter;
	float[2] GrayLInter;
} gs_out;



void EmitCorner(int i)
{
	gs_out.Original = gs_in[i].Original;
	gs_out.Absolute = gs_in[i].Absolute;
	gs_out.Relative = gs_in[i].Relative;

	gs_out.AltColor = gs_in[i].AltColor;
	gs_out.AltColLInter[0] = gs_in[i].AltColLInter[0];
	gs_out.AltColLInter[1] = gs_in[i].AltColLInter[1];
	gs_out.GrayLInter[0] = gs_in[i].GrayLInter[0];
	gs_out.GrayLInter[1] = gs_in[i].GrayLInter[1];

	gl_Position = gl_in[i].gl_Position;
	EmitVertex();
}

void main()
{
	gs_out.Color = vec3(0, 0, 0);

	EmitCorner(0);
	EmitCorner(1);
	EmitCorner(2);
	EmitCorner(0);
	EndPrimitive();
}
