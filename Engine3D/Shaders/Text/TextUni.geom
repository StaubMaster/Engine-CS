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



layout(points) in;
layout(triangle_strip, max_vertices = 64) out;
//layout(line_strip, max_vertices = 64) out;



in Text_Char {
	pallet Pallet;
	vec2 Pos;
	vec2 Scale;
	vec2 Thick;
	vec3 Color;
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} gs_out;



void EmitPalletPoint(vec2 relative, vec2 position)
{
	relative = relative * gs_in[0].Thick;
	position = position * gs_in[0].Scale;

	vec2 absolute = relative + position;
	absolute = absolute + gs_in[0].Pos;

	gl_Position = vec4(absolute, 0, 1);
	EmitVertex();
}

void EmitPallet(pallet_point[8] pp)
{
	int i = 0;
	while (i < 8)
	{
		if (pp[i].pos.x == 255)
		{
			EndPrimitive();
		}
		else
		{
			EmitPalletPoint(pp[i].dir + pp[i].per, pp[i].pos);
			EmitPalletPoint(pp[i].dir - pp[i].per, pp[i].pos);
		}
		i++;
	}
	EndPrimitive();
}

void main()
{
	gs_out.Color = gs_in[0].Color;
	gs_out.Absolut = vec3(0, 0, 0);
	gs_out.Normal = vec3(0, 0, 0);

	pallet_point[8] pp;
	pp[0] = gs_in[0].Pallet.p0;
	pp[1] = gs_in[0].Pallet.p1;
	pp[2] = gs_in[0].Pallet.p2;
	pp[3] = gs_in[0].Pallet.p3;
	pp[4] = gs_in[0].Pallet.p4;
	pp[5] = gs_in[0].Pallet.p5;
	pp[6] = gs_in[0].Pallet.p6;
	pp[7] = gs_in[0].Pallet.p7;

	EmitPallet(pp);
}
