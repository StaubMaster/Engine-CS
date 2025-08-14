#version 430

layout(location = 0) in vec2 VPos;
layout(location = 1) in uint VOff;
layout(location = 2) in uint VCol;
layout(location = 3) in uint VChar;

layout(std430, binding = 0) buffer BufferChars {
	ivec2 none[1][8];
	ivec2 digit[10][8];

	ivec2 letterU[26][8];
	ivec2 letterL[26][8];
	ivec2 punctuation[6][8];

	ivec2 arithmetic[4][8];
	ivec2 compare[3][8];
	ivec2 brackets[6][8];
};



const float charNorm = 0.1;
const vec2 charRatio = vec2(0.5, 1.0);
const vec2 screenRatio = vec2(0.5, 1.0);
const float charScale = 0.0125 * 2;
const vec2 scale = charRatio * screenRatio * charNorm * charScale;



out Text_Char {
	vec2 Pos;
	vec3 Color;
	ivec2 pallet[8];
} vs_out;


int ushort_to_int(uint num)
{
	if ((num & 0x8000) != 0)
		num |= 0xFFFF0000;
	return int(num);
}

ivec2[8] select_pallet()
{
	uint c = VChar;

	if (c < 10)
		return digit[c];
	c -= 10;

	if (c < 26)
		return letterU[c];
	c -= 26;

	if (c < 26)
		return letterL[c];
	c -= 26;

	if (c < 6)
		return punctuation[c];
	c -= 6;

	if (c < 4)
		return arithmetic[c];
	c -= 4;

	if (c < 3)
		return compare[c];
	c -= 3;

	if (c < 6)
		return brackets[c];
	c -= 6;

	return none[0];
}

void main()
{
	ivec2 offset;
	offset.x = ushort_to_int((VOff >> 16));
	offset.y = ushort_to_int((VOff & 0xFFFF));

	vs_out.Pos = VPos + (offset * scale * 24);

	vs_out.Color.r = ((VCol & 0xFF0000) >> 16) / 255.0;
	vs_out.Color.g = ((VCol & 0x00FF00) >>  8) / 255.0;
	vs_out.Color.b = ((VCol & 0x0000FF) >>  0) / 255.0;

	vs_out.pallet = select_pallet();
}
