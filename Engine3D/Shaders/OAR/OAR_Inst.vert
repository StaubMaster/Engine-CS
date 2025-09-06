#version 430

uniform vec2[2] screenRatios = { vec2(640, 480), vec2(0.75, 1.00) };

uniform vec3[3] view = { vec3(0, 0, 0), vec3(0, 0, 0), vec3(1, 1, 1) };

uniform float[7] depthFactor;



layout(location = 0) in vec3 VPos;

layout(location = 1) in vec3 IPos;
layout(location = 2) in vec3 ISin;
layout(location = 3) in vec3 ICos;

out Vert {
	vec3 Original;
	vec3 Absolute;
	vec3 Relative;
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
	vs_out.Original = VPos;
	vs_out.Absolute = DSA(vs_out.Original, ISin, ICos) + IPos;
	vs_out.Relative = ASD(vs_out.Absolute - view[0], view[1], view[2]);
	gl_Position = proj(vs_out.Relative);
}
