#version 430

layout(triangles) in;
layout(triangle_strip, max_vertices = 3) out;

layout(std430, binding = 0) buffer BufferColor {
	uint color[];
};



in VertInst {
	vec3 Original;
	vec3 Absolute;
	vec3 Relative;

	vec3 AltColor;
	float[2] AltColLInter;
	float[2] GrayLInter;
} gs_in[];

out GeomInst {
	vec3 Color;
	vec3 Normal;

	vec3 Original;
	vec3 Absolute;
	vec3 Relative;

	vec3 AltColor;
	float[2] AltColLInter;
	float[2] GrayLInter;
} gs_out;



void EmitCorner(int i)
{
	gs_out.Original = gs_in[i].Original;
	gs_out.Absolute = gs_in[i].Absolute;
	gs_out.Relative = gs_in[i].Relative;
	gl_Position = gl_in[i].gl_Position;
	EmitVertex();
}

void CalcNormal(vec3 a, vec3 b, vec3 c)
{
	vec3 p1, p2;
	p1 = b - a;
	p2 = c - a;

	vec3 n;
	n.x = p1.y * p2.z - p1.z * p2.y;
	n.y = p1.z * p2.x - p1.x * p2.z;
	n.z = p1.x * p2.y - p1.y * p2.x;
	gs_out.Normal = normalize(n);
}

vec3 UIntColor(uint UCol)
{
	vec3 col3;
	col3.r = (UCol & 0xFF0000) >> 16;
	col3.g = (UCol & 0x00FF00) >> 8;
	col3.b = (UCol & 0x0000FF) >> 0;
	return col3 / 255.0;
}

void main()
{
	gs_out.Color = UIntColor(color[gl_PrimitiveIDIn]);

	gs_out.AltColor = gs_in[0].AltColor;
	gs_out.AltColLInter[0] = gs_in[0].AltColLInter[0];
	gs_out.AltColLInter[1] = gs_in[0].AltColLInter[1];
	gs_out.GrayLInter[0] = gs_in[0].GrayLInter[0];
	gs_out.GrayLInter[1] = gs_in[0].GrayLInter[1];

	CalcNormal(gs_in[0].Absolute, gs_in[1].Absolute, gs_in[2].Absolute);

	EmitCorner(0);
	EmitCorner(1);
	EmitCorner(2);
	EndPrimitive();
}
