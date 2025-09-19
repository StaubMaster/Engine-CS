#version 430

layout(points) in;
layout(triangle_strip, max_vertices = 12) out;



in VertTile {
	vec3		Color;
	ivec3[5]	Absolute;
	vec3[5]		Relative;
	vec4[5]		Position;
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Normal;
	vec3 Original;
	vec3 Absolute;
	vec3 Relative;
} gs_out;



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

void EmitCorn(int i)
{
	gs_out.Original = vec3(0, 0, 0);
	gs_out.Absolute = gs_in[0].Absolute[i];
	gs_out.Relative = gs_in[0].Relative[i];
	gl_Position = gs_in[0].Position[i];
	EmitVertex();
}

void EmitTri(int i0, int i1, int i2)
{
	CalcNormal(gs_in[0].Absolute[i0], gs_in[0].Absolute[i1], gs_in[0].Absolute[i2]);
	EmitCorn(i0);
	EmitCorn(i1);
	EmitCorn(i2);
	EndPrimitive();
}

void main()
{
	gs_out.Color = gs_in[0].Color;

	EmitTri(4, 1, 0);
	EmitTri(4, 2, 1);
	EmitTri(4, 3, 2);
	EmitTri(4, 0, 3);
}
