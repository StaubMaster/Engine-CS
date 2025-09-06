#version 430



uniform vec2[2] screenRatios;



layout(location = 0) in uint VChar;

layout(location = 1) in vec2 VAnchor;
layout(location = 2) in vec2 VPosition;
layout(location = 3) in vec2 VOffset;

layout(location = 4) in float VHeight;
layout(location = 5) in float VPadding;
layout(location = 6) in float VThick;

layout(location = 7) in uint VColor;

out Text_Char {
	uint Char;

	vec2 Anchor;
	vec2 Position;
	vec2 Offset;

	float Height;
	float Padding;
	float Thick;

	vec3 Color;
} vs_out;



void main()
{
	vs_out.Char = VChar;

	vs_out.Anchor = VAnchor;
	vs_out.Position = VPosition;
	vs_out.Offset = VOffset;

	vs_out.Height = VHeight;
	vs_out.Padding = VPadding;
	vs_out.Thick = VThick;

	vs_out.Color.r = ((VColor & 0xFF0000) >> 16) / 255.0;
	vs_out.Color.g = ((VColor & 0x00FF00) >>  8) / 255.0;
	vs_out.Color.b = ((VColor & 0x0000FF) >>  0) / 255.0;
}
