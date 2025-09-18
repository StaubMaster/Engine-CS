#version 430

uniform vec2[2] screenRatios = { vec2(640, 480), vec2(0.75, 1.00) };

uniform float[7] depthFactor;


//	PolyHedra
layout(location = 0) in vec3 VPos;
layout(location = 1) in vec3 VNorm;
layout(location = 2) in float VTex;

//	Grid Pos
layout(location = 4) in vec2 IPosNormal;
layout(location = 5) in vec2 IPosPixel;
layout(location = 6) in vec2 IPosOffset;

//	Grid Size
layout(location = 7) in vec2 ISize;
layout(location = 8) in float IPadding;

//	Body Scale
layout(location = 9) in float IScale;

//	Trans
layout(location = 10) in vec3 IPos;
layout(location = 11) in vec3 ISin;
layout(location = 12) in vec3 ICos;

out VertUI {
	vec3 Original;
	vec3 Absolute;
	vec3 Relative;

	vec3 Normal;
	float Tex;
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
	p_out.z = p_inn.z * 0.01;
	p_out.w = 1.0;

	p_out.x = p_out.x / screenRatios[0].x;
	p_out.y = p_out.y / screenRatios[0].y;

	p_out.x = p_out.x * ISize.x;
	p_out.y = p_out.y * ISize.y;

	vec2 normPos;
	normPos = IPosPixel + (IPosOffset * (ISize + IPadding));
	normPos = normPos / screenRatios[0];
	normPos = normPos * 2;
	normPos = normPos + IPosNormal;

	p_out.x += normPos.x;
	p_out.y += normPos.y;

	return p_out;
}

vec3 UIntColor(uint UCol)
{
	vec3 col3;
	col3.r = (UCol & 0xFF0000) >> 16;
	col3.g = (UCol & 0x00FF00) >> 8;
	col3.b = (UCol & 0x0000FF) >> 0;
	return col3 / 255.0;
}

void main()
{
	vs_out.Original = VPos * IScale;
	vs_out.Absolute = ASD(vs_out.Original + IPos, ISin, ICos);
	vs_out.Relative = vs_out.Absolute;
	gl_Position = proj(vs_out.Relative);

	vs_out.Normal = DSA(VNorm, ISin, ICos);
	vs_out.Tex = VTex;
}
