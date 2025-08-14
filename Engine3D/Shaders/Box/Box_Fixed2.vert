#version 430

uniform vec2[2] screenRatios;

uniform vec3[3] view;

uniform float[7] depthFactor;



layout(location = 0) in uint VCol;
layout(location = 1) in vec3 VMin;
layout(location = 2) in vec3 VMax;

out VertBox {
	vec3		Color;
	vec3[8]		Absolute;
	vec3[8]		Relative;
	vec4[8]		Position;
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

	p_out.x = p_out.x * screenRatios[1].x;
	p_out.y = p_out.y * screenRatios[1].y;

	return p_out;
}

void main()
{
	vs_out.Color.r = ((VCol >> 16) & 0xFF) / 255.0;
	vs_out.Color.g = ((VCol >> 8) & 0xFF) / 255.0;
	vs_out.Color.b = ((VCol >> 0) & 0xFF) / 255.0;

	vs_out.Absolute[0] = vec3(VMin.x, VMin.y, VMin.z);
	vs_out.Absolute[1] = vec3(VMax.x, VMin.y, VMin.z);
	vs_out.Absolute[2] = vec3(VMax.x, VMax.y, VMin.z);
	vs_out.Absolute[3] = vec3(VMin.x, VMax.y, VMin.z);
	vs_out.Absolute[4] = vec3(VMin.x, VMin.y, VMax.z);
	vs_out.Absolute[5] = vec3(VMax.x, VMin.y, VMax.z);
	vs_out.Absolute[6] = vec3(VMax.x, VMax.y, VMax.z);
	vs_out.Absolute[7] = vec3(VMin.x, VMax.y, VMax.z);

	vs_out.Relative[0] = ASD(vs_out.Absolute[0] - view[0], view[1], view[2]);
	vs_out.Relative[1] = ASD(vs_out.Absolute[1] - view[0], view[1], view[2]);
	vs_out.Relative[2] = ASD(vs_out.Absolute[2] - view[0], view[1], view[2]);
	vs_out.Relative[3] = ASD(vs_out.Absolute[3] - view[0], view[1], view[2]);
	vs_out.Relative[4] = ASD(vs_out.Absolute[4] - view[0], view[1], view[2]);
	vs_out.Relative[5] = ASD(vs_out.Absolute[5] - view[0], view[1], view[2]);
	vs_out.Relative[6] = ASD(vs_out.Absolute[6] - view[0], view[1], view[2]);
	vs_out.Relative[7] = ASD(vs_out.Absolute[7] - view[0], view[1], view[2]);

	vs_out.Position[0] = proj(vs_out.Relative[0]);
	vs_out.Position[1] = proj(vs_out.Relative[1]);
	vs_out.Position[2] = proj(vs_out.Relative[2]);
	vs_out.Position[3] = proj(vs_out.Relative[3]);
	vs_out.Position[4] = proj(vs_out.Relative[4]);
	vs_out.Position[5] = proj(vs_out.Relative[5]);
	vs_out.Position[6] = proj(vs_out.Relative[6]);
	vs_out.Position[7] = proj(vs_out.Relative[7]);
}
