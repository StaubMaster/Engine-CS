#version 430

uniform vec3[3] view;
uniform float[7] depthFactor;
uniform float fov;



layout(location = 0) in vec3 VInn;
layout(location = 1) in vec3 VOut;

out VertTP {
	vec3		Color;
	vec3[2]		Absolut;
	vec4[2]		Proj;
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
	vs_out.Color = vec3(1, 1, 1);

	vs_out.Absolut[0] = VInn;
	vs_out.Absolut[1] = VOut;

	vs_out.Proj[0] = proj(ASD(vs_out.Absolut[0] - view[0], view[1], view[2]));
	vs_out.Proj[1] = proj(ASD(vs_out.Absolut[1] - view[0], view[1], view[2]));
}
