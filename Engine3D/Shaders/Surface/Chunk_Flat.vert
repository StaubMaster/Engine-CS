#version 430

uniform vec3[3] view;
uniform float[7] depthFactor;
uniform float fov;

uniform ivec3 chunk_pos;
uniform int tiles_size;
uniform int tiles_per_side;



layout(location = 0) in uint VCol;
layout(location = 1) in int VHeight_Mid;
layout(location = 2) in ivec4 VHeight_Corn;

out VertTile {
	vec3		Color;
	ivec3[5]	Absolut;
	vec4[5]		Proj;
} vs_out;



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

vec3 DSA(in vec3 p, in vec3 wSin, in vec3 wCos)
{
	vec3 n = p;
	rot(n.x, n.y, wSin.z, wCos.z);
	rot(n.z, n.y, wSin.y, wCos.y);
	rot(n.z, n.x, wSin.x, wCos.x);
	return n;
}

vec4 proj(in vec3 p_inn)
{
	vec4 p_out;

	p_out.x = p_inn.x;
	p_out.y = p_inn.y;
	p_out.z = p_inn.z * depthFactor[5] - depthFactor[6];
	p_out.w = p_inn.z;

	p_out.x = p_out.x / 2;
	return p_out;
}



void main()
{
	vs_out.Color.r = ((VCol >> 16) & 0xFF) / 255.0;
	vs_out.Color.g = ((VCol >> 8) & 0xFF) / 255.0;
	vs_out.Color.b = ((VCol >> 0) & 0xFF) / 255.0;


	ivec3 p;
	p.x = gl_VertexID % tiles_per_side;
	p.z = gl_VertexID / tiles_per_side;

	p.x += chunk_pos.x * tiles_per_side;
	p.z += chunk_pos.z * tiles_per_side;

	p.x *= tiles_size;
	p.z *= tiles_size;


	p.y = VHeight_Corn[0];
	vs_out.Absolut[0] = p;
	p.x += tiles_size;

	p.y = VHeight_Corn[1];
	vs_out.Absolut[1] = p;
	p.z += tiles_size;

	p.y = VHeight_Corn[2];
	vs_out.Absolut[2] = p;
	p.x -= tiles_size;

	p.y = VHeight_Corn[3];
	vs_out.Absolut[3] = p;
	p.z -= tiles_size;

	p.y = VHeight_Mid;
	p.x += tiles_size / 2;
	p.z += tiles_size / 2;
	vs_out.Absolut[4] = p;

	vs_out.Proj[0] = proj(ASD(vs_out.Absolut[0] - view[0], view[1], view[2]));
	vs_out.Proj[1] = proj(ASD(vs_out.Absolut[1] - view[0], view[1], view[2]));
	vs_out.Proj[2] = proj(ASD(vs_out.Absolut[2] - view[0], view[1], view[2]));
	vs_out.Proj[3] = proj(ASD(vs_out.Absolut[3] - view[0], view[1], view[2]));
	vs_out.Proj[4] = proj(ASD(vs_out.Absolut[4] - view[0], view[1], view[2]));
}
