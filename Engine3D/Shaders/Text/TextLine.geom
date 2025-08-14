#version 430

layout(points) in;
layout(line_strip, max_vertices = 64) out;



uniform vec2[2] screenRatios;
uniform vec2 scale;
uniform vec2 ratio;
uniform float thick;



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



in Text_Char {
	vec2 Pos;
	vec3 Color;
	pallet pallet;
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} gs_out;



vec2 Intersekt(vec2 p1, vec2 p2, vec2 p3, vec2 p4)
{
	vec2 Ldir12 = p1 - p2;
	vec2 Ldir34 = p3 - p4;
	float f12 = (p1.x * p2.y) - (p1.y * p2.x);
	float f34 = (p3.x * p4.y) - (p3.y * p4.x);
	float div = (Ldir12.x * Ldir34.y) - (Ldir12.y * Ldir34.x);

	return vec2(
		((f12 * Ldir34.x) - (Ldir12.x * f34)) / div,
		((f12 * Ldir34.y) - (Ldir12.y * f34)) / div
	);
}

void emit(vec2 p)
{
	gl_Position = vec4((gs_in[0].Pos) + (p * scale), 0, 1);
	EmitVertex();
}

vec2 perpendicular(vec2 p)
{
	return vec2(+p.y, -p.x);
}

void LineCenter(pallet_point[8] pp)
{
	for (int i = 0; i < 8; i++)
	{
		if (pp[i].pos.x == 255.0) { if (pp[i].pos.y == 255.0) { break; } EndPrimitive(); }
		else { emit(pp[i].pos); }
	} EndPrimitive();
}

void LineMns(pallet_point[8] pp)
{
	for (int i = 0; i < 8; i++)
	{
		if (pp[i].pos.x == 255.0) { if (pp[i].pos.y == 255.0) { break; } EndPrimitive(); }
		else { emit(pp[i].pos + ((pp[i].dir + pp[i].per) * thick)); }
	} EndPrimitive();
}
void LinePls(pallet_point[8] pp)
{
	for (int i = 0; i < 8; i++)
	{
		if (pp[i].pos.x == 255.0) { if (pp[i].pos.y == 255.0) { break; } EndPrimitive(); }
		else { emit(pp[i].pos + ((pp[i].dir - pp[i].per) * thick)); }
	} EndPrimitive();
}

/*void LineCalc(pallet_point[8] pp)
{
	for (int i = 0; i < 8; i++)
	{
		if (pp[i].pos.x == 255.0) { if (pp[i].pos.y == 255.0) { break; } EndPrimitive(); }
		else
		{
			vec2 p0 = vec2(0, 0);
			bool IsPrev = false;
			if (i > 0 && pp[i - 1].pos.x != 255.0)
			{
				p0 = pp[i - 1].pos;
				IsPrev = true;
			}

			vec2 p1 = vec2(0, 0);
			p1 = pp[i].pos;

			vec2 p2 = vec2(0, 0);
			bool IsNext = false;
			if (i < 7 && pp[i + 1].pos.x != 255.0)
			{
				p2 = pp[i + 1].pos;
				IsNext = true;
			}

			if (!IsPrev && !IsNext) { }
			else if (IsPrev && !IsNext)
			{
				vec2 dir01 = normalize(p1 - p0);
				vec2 per01 = perpendicular(dir01);
				emit(p1 - (per01 - dir01) * thick);
				emit(p1 + (per01 + dir01) * thick);
			}
			else if (!IsPrev && IsNext)
			{
				vec2 dir12 = normalize(p2 - p1);
				vec2 per12 = perpendicular(dir12);
				emit(p1 - (per12 + dir12) * thick);
				emit(p1 + (per12 - dir12) * thick);
			}
			else
			{
				vec2 per01 = perpendicular(normalize(p1 - p0)) * thick;
				vec2 per12 = perpendicular(normalize(p2 - p1)) * thick;

				vec2 inter0 = Intersekt(
					p0 - per01, p1 - per01,
					p1 - per12, p2 - per12);

				vec2 inter1 = Intersekt(
					p0 + per01, p1 + per01,
					p1 + per12, p2 + per12);

				emit(inter0);
				emit(inter1);
			}
		}
	} EndPrimitive();
}*/

/*	skips
2 bits per pallet_point
00 : normal
01 : end primitive before this
10 : 
11 : do nothing (filler for end)
*/

void main()
{
	gs_out.Color = gs_in[0].Color;
	gs_out.Absolut = vec3(0, 0, 0);
	gs_out.Normal = vec3(0, 0, 0);

	pallet_point[8] pp;
	pp[0] = gs_in[0].pallet.p0;
	pp[1] = gs_in[0].pallet.p1;
	pp[2] = gs_in[0].pallet.p2;
	pp[3] = gs_in[0].pallet.p3;
	pp[4] = gs_in[0].pallet.p4;
	pp[5] = gs_in[0].pallet.p5;
	pp[6] = gs_in[0].pallet.p6;
	pp[7] = gs_in[0].pallet.p7;

	//gs_out.Color = vec3(0, 1, 0); LineCalc(pp);
	gs_out.Color = vec3(0, 0, 0); LineCenter(pp);
	gs_out.Color = vec3(1, 0, 0); LineMns(pp);
	gs_out.Color = vec3(0, 0, 1); LinePls(pp);
}
