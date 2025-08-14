#version 430

layout(location = 0) in vec3 VPos;

uniform vec3[3] view;
uniform vec2 depthFactor;
uniform float fov;

uniform vec3[3] trans;

uniform float fade;
uniform mat4 projMat;


out Vert {
	vec3 NonProj;
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
	vec3 rel;
	rel = ASD(p_inn - view[0], view[1], view[2]);

	vec4 _old;
	_old = vec4(+rel.x, +rel.y, -rel.z, 1.0) * projMat;

	vec4 _new;
	_new.x = rel.x;
	_new.y = rel.y;
	_new.z = rel.z * depthFactor[0] - depthFactor[1];
	_new.w = rel.z;

	vec4 p_out = (1.0 - fade) * _old + (0.0 + fade) * _new;

	p_out.x = p_out.x / 2;
	return p_out;
}

void main()
{
	vec3 p;
	p = DSA(VPos, trans[1], trans[2]) + trans[0];
	vs_out.NonProj = p;
	gl_Position = proj(p);
}
