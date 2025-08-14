#version 430

uniform vec2 pos;
uniform vec3[2] rot;
uniform float scale;



layout(location = 0) in vec3 VPos;

out Vert {
	vec3 Absolut;
} vs_out;



void rot2D(inout float pls, inout float mns, in float fsin, in float fcos)
{
	float tmp;
	tmp = fcos * pls - fsin * mns;
	mns = fcos * mns + fsin * pls;
	pls = tmp;
}

vec3 ASD(in vec3 p, in vec3 wSin, in vec3 wCos)
{
	vec3 n = p;
	rot2D(n.x, n.z, wSin.x, wCos.x);
	rot2D(n.y, n.z, wSin.y, wCos.y);
	rot2D(n.y, n.x, wSin.z, wCos.z);
	return n;
}

vec3 DSA(in vec3 p, in vec3 wSin, in vec3 wCos)
{
	vec3 n = p;
	rot2D(n.x, n.y, wSin.z, wCos.z);
	rot2D(n.z, n.y, wSin.y, wCos.y);
	rot2D(n.z, n.x, wSin.x, wCos.x);
	return n;
}

void main()
{
	vec3 p;
	p = ASD(VPos, rot[0], rot[1]);
	vs_out.Absolut = p;

	p = p * scale;
	p.x = p.x * 0.5 + pos.x;
	p.y = p.y * 1.0 + pos.y;
	gl_Position = vec4(p, 1.0);
}
