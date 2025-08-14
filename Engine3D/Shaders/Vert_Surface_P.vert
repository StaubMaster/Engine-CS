#version 430

uniform vec3[3] view;
uniform vec2 depthFactor;
uniform float fov;

uniform uint corns_per_side;
uniform uint tiles_width;
uniform ivec3 tiles_middle_offset;



layout(location = 0) in uint VHeight;

out VertSurf {
	vec3  Color;
	ivec3 Absolut;
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
	p_out.z = p_inn.z * depthFactor[0] - depthFactor[1];
	p_out.w = p_inn.z;

	p_out.x = p_out.x / 2;
	return p_out;
}

void main()
{
	uint idx = gl_VertexID;

	ivec3 p;
	p.x = int((idx % corns_per_side) * tiles_width);
	p.y = +int(VHeight);
	p.z = int((idx / corns_per_side) * tiles_width);

	p += tiles_middle_offset;
	vs_out.Absolut = p;

	gl_Position = proj(ASD(p - view[0], view[1], view[2]));

	switch (tiles_width)
	{
		default: vs_out.Color = vec3(1, 1, 1); break;
		case 16: vs_out.Color = vec3(0, 1, 1); break;
		case 32: vs_out.Color = vec3(1, 0, 1); break;
		case 64: vs_out.Color = vec3(1, 1, 0); break;
		case 128: vs_out.Color = vec3(0, 1, 1); break;
		case 256: vs_out.Color = vec3(1, 0, 1); break;
		case 512: vs_out.Color = vec3(1, 1, 0); break;
		case 1024: vs_out.Color = vec3(0, 1, 1); break;
		case 2048: vs_out.Color = vec3(1, 0, 1); break;
	}
}
