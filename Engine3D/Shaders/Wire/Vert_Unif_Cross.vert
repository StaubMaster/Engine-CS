#version 430

uniform vec3[3] view;
uniform vec2 depthFactor;
uniform float fov;

uniform vec3[3] trans;

const float cross_size = 0.1;



layout(location = 0) in vec3 VPos;

out VertCross {
	vec4 Corners[6];
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
	p_inn = ASD(p_inn - view[0], view[1], view[2]);

	vec4 p_out;
	p_out.x = p_inn.x;
	p_out.y = p_inn.y;
	p_out.z = p_inn.z * depthFactor[0] - depthFactor[1];
	p_out.w = p_inn.z;

	p_out.x = p_out.x / 2;
	return p_out;
}

void main()
{
	vec3 p;
	p = DSA(VPos, trans[1], trans[2]) + trans[0];

	vs_out.Corners[0] = proj(p + vec3(+cross_size, 0, 0));
	vs_out.Corners[1] = proj(p + vec3(-cross_size, 0, 0));
	vs_out.Corners[2] = proj(p + vec3(0, +cross_size, 0));
	vs_out.Corners[3] = proj(p + vec3(0, -cross_size, 0));
	vs_out.Corners[4] = proj(p + vec3(0, 0, +cross_size));
	vs_out.Corners[5] = proj(p + vec3(0, 0, -cross_size));

	gl_Position = proj(p);
}
