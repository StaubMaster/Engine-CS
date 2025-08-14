#version 430



struct pallet_point
{
	vec2 pos;
	vec2 dir;
	vec2 per;
};

struct pallet
{
	pallet_point	p0;
	pallet_point	p1;
	pallet_point	p2;
	pallet_point	p3;
	pallet_point	p4;
	pallet_point	p5;
	pallet_point	p6;
	pallet_point	p7;
	uint		skips;
	uint		padding;
};



uniform vec2[2] screenRatios;



layout(location = 0) in uint VChar;
layout(location = 1) in vec2 VPos;
layout(location = 2) in vec2 VScale;
layout(location = 3) in vec2 VThick;
layout(location = 4) in uint VColor;

layout(std430, binding = 0) buffer BufferChars {
	pallet none[1];
	pallet digit[10];

	pallet letterHi[26];
	pallet letterLo[26];
	pallet punctuation[6];

	pallet arithmetic[4];
	pallet compare[3];
	pallet brackets[6];
};

out Text_Char {
	pallet Pallet;
	vec2 Pos;
	vec2 Scale;
	vec2 Thick;
	vec3 Color;
} vs_out;



pallet select_pallet()
{
	uint c = VChar;

	if (c < 10) { return digit[c]; }		c -= 10;
	if (c < 26) { return letterHi[c]; }		c -= 26;
	if (c < 26) { return letterLo[c]; }		c -= 26;
	if (c < 6) { return punctuation[c]; }	c -= 6;
	if (c < 4) { return arithmetic[c]; }	c -= 4;
	if (c < 3) { return compare[c]; }		c -= 3;
	if (c < 6) { return brackets[c]; }		c -= 6;

	return none[0];
}

void main()
{
	vs_out.Pallet = select_pallet();

	vs_out.Pos = VPos;
	vs_out.Scale = VScale;
	vs_out.Thick = VThick;

	vs_out.Color.r = ((VColor & 0xFF0000) >> 16) / 255.0;
	vs_out.Color.g = ((VColor & 0x00FF00) >>  8) / 255.0;
	vs_out.Color.b = ((VColor & 0x0000FF) >>  0) / 255.0;
}
