#version 430

uniform vec3[3] view;
uniform vec2 depthFactor;
uniform float fov;

uniform uint tiles_per_side;
uniform uvec2[2] tiles_cut;



layout (lines_adjacency) in;
layout (triangle_strip, max_vertices = 12) out;

//layout(std430, binding = 0) buffer BufferColor {
//	uint color[];
//};



in VertSurf {
	vec3  Color;
	ivec3 Absolut;
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} gs_out;



vec3 middle;
vec4 middle_proj;


void rot(inout float pls, inout float mns, in float fsin, in float fcos)
{
	float tmp;
	tmp = fcos * pls - fsin * mns;
	mns = fcos * mns + fsin * pls;
	pls = tmp;
}

vec3 ASD(in vec3 p, in vec3 wSin, in vec3 wCos)
{
	vec3 n = p;
	rot(n.x, n.z, wSin.x, wCos.x);
	rot(n.y, n.z, wSin.y, wCos.y);
	rot(n.y, n.x, wSin.z, wCos.z);
	return n;
}

vec4 proj(in vec3 p_inn)
{
	vec4 p_out;
	p_out.x = p_inn.x;
	p_out.y = p_inn.y;
	p_out.z = p_inn.z * depthFactor[0] - depthFactor[1];
	p_out.w = p_inn.z;

	p_out.x = p_out.x / 2;
	return p_out;
}



void EmitMiddle()
{
	gs_out.Absolut = middle;
//	gs_out.Color = vec3(1, 1, 1);
	gl_Position = middle_proj;
	EmitVertex();
}

void EmitCorner(int i)
{
	gs_out.Absolut = gs_in[i].Absolut;
//	gs_out.Color = gs_in[i].Color;
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

void EmitTri(int i1, int i2)
{
	CalcNormal(
		gs_in[i1].Absolut,
		gs_in[i2].Absolut,
		middle
		);
	EmitCorner(i1);
	EmitCorner(i2);
	EmitMiddle();
	EndPrimitive();
}

void main()
{
	uvec2 idx;
	idx.x = gl_PrimitiveIDIn % tiles_per_side;
	idx.y = gl_PrimitiveIDIn / tiles_per_side;
	if (tiles_cut[0].x <= idx.x &&
		tiles_cut[0].y <= idx.y &&
		idx.x < tiles_cut[1].x &&
		idx.y < tiles_cut[1].y)
	{
		return;
	}

	gs_out.Color = gs_in[0].Color;

	if ((idx.x & 1) == (idx.y & 1))
		gs_out.Color += vec3(0.125, 0.125, 0.125);
	else
		gs_out.Color -= vec3(0.125, 0.125, 0.125);

	middle = (
		gs_in[0].Absolut +
		gs_in[1].Absolut +
		gs_in[2].Absolut +
		gs_in[3].Absolut) * 0.25;
	middle_proj = proj(ASD(middle - view[0], view[1], view[2]));

	EmitTri(2, 0);
	EmitTri(3, 2);
	EmitTri(1, 3);
	EmitTri(0, 1);
}
