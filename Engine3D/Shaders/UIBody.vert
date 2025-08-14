#version 430

uniform vec2[2] screenRatios;

uniform vec3[3] bodyTrans;
uniform float bodyScale;

uniform vec2[3] UIRectangle;



layout(location = 0) in vec3 VPos;

out Vert {
	vec3 Absolut;
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
	p_out.z = p_inn.z * 0.5;
	p_out.w = 1.0;

	return p_out;
}

void main()
{
	vec3 p = VPos;

	p = ASD(p, bodyTrans[1], bodyTrans[2]);

	p = p / bodyScale;

	p.x = p.x * (UIRectangle[0].x / screenRatios[0].x);
	p.y = p.y * (UIRectangle[0].y / screenRatios[0].y);

	p.x += UIRectangle[2].x + ((UIRectangle[1].x * 2) / screenRatios[0].x);
	p.y += UIRectangle[2].y + ((UIRectangle[1].y * 2) / screenRatios[0].y);

	vs_out.Absolut = p;
	gl_Position = proj(p);
}
