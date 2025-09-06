#version 430



struct pallet_point
{
	vec2 pos;
	vec2 dir;
	vec2 per;
	vec2 thickDir0;
	vec2 thickDir1;
	float thickScalar;
	uint padding;
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
//layout(triangle_strip, max_vertices = 64) out;
layout(line_strip, max_vertices = 64) out;

layout(std430, binding = 0) buffer BufferChars {
	pallet pallets[];
};



in Text_Char {
	uint Char;

	vec2 Pos;
	vec2 Off;
	vec2 Size;
	vec2 Thick;

	vec2 PixelSize;
	float PixelThick;
	float PixelPad;

	vec3 Color;
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} gs_out;

void EmitPalletPoint(vec2 relative, vec2 position)
{
	vec2 normRatio = gs_in[0].PixelSize;
	normRatio.x = normRatio.x / gs_in[0].PixelSize.y;
	normRatio.y = normRatio.y / gs_in[0].PixelSize.x;

	normRatio.x = min(1.0, normRatio.x);
	normRatio.y = min(1.0, normRatio.y);

	vec2 normSize = gs_in[0].PixelSize;
	normSize = normSize / screenRatios[0];

	//normRatio = normalize(normRatio);
	vec2 normThick = vec2(gs_in[0].PixelThick);
	normThick = normThick / screenRatios[0];
	//normThick = normThick * normRatio;

	//vec2 normStride;
	//normStride.x = (gs_in[0].PixelSize.x + gs_in[0].PixelPad) / screenRatios[0].x;
	//normStride.y = (gs_in[0].PixelSize.y + gs_in[0].PixelPad) / screenRatios[0].y;

	//relative = relative * normThick;
	//position = position * normSize;

	//relative = relative * normThick;
	//position = position * normSize;

	relative = relative * gs_in[0].Thick;
	position = position * gs_in[0].Size;

	vec2 absolute = relative + position;
	absolute = absolute + gs_in[0].Pos;

	gl_Position = vec4(absolute, 0, 1);
	EmitVertex();
}

void EmitPallet(pallet_point[8] pp_arr)
{
	vec2[8] centers;
	vec2[8] points0;
	vec2[8] points1;
	vec2[8] points2;
	vec2[8] points3;

	int i = 0;
	while (i < 8)
	{
		pallet_point pp = pp_arr[i];

		if (pp.pos.x == 255)
		{
			//EndPrimitive();
		}
		else
		{
			//vec2 normRatio = gs_in[0].PixelSize;
			//normRatio.x = normRatio.x / gs_in[0].PixelSize.x;
			//normRatio.y = normRatio.y / gs_in[0].PixelSize.y;
			//normRatio.x = min(1.0, normRatio.x);
			//normRatio.y = min(1.0, normRatio.y);

			//vec2 dir = pp.dir;
			//vec2 per = pp.per;
			//per = normalize(per * normRatio);

			//EmitPalletPoint(dir + per, pp[i].pos);
			//EmitPalletPoint(dir - per, pp[i].pos);

			vec2 relative;
			vec2 position;
			vec2 absolute;

			centers[i] = pp.pos * gs_in[0].Size + gs_in[0].Pos * vec2(2, 1);



			relative = pp.dir + pp.per;
			position = pp.pos;
			relative = relative * gs_in[0].Thick;
			position = position * gs_in[0].Size;
			absolute = relative + position;
			absolute = absolute + gs_in[0].Pos * vec2(2, 1);
			points0[i] = absolute;



			relative = pp.thickDir0;
			position = pp.pos;
			relative = relative * vec2(0.5, 1.0) * 0.05;
			position = position * gs_in[0].Size;
			absolute = relative + position;
			absolute = absolute + gs_in[0].Pos * vec2(2, 1);
			points2[i] = absolute;



			relative = pp.dir - pp.per;
			position = pp.pos;
			relative = relative * gs_in[0].Thick;
			position = position * gs_in[0].Size;
			absolute = relative + position;
			absolute = absolute + gs_in[0].Pos * vec2(2, 1);
			points1[i] = absolute;



			relative = pp.thickDir1;
			position = pp.pos;
			relative = relative * vec2(0.5, 1.0) * 0.05;
			position = position * gs_in[0].Size;
			absolute = relative + position;
			absolute = absolute + gs_in[0].Pos * vec2(2, 1);
			points3[i] = absolute;
		}
		i++;
	}
	//EndPrimitive();



	gs_out.Color = vec3(0, 0, 0);
	i = 0; while (i < 8) {
		if (pp_arr[i].pos.x == 255) { EndPrimitive(); }
		else { gl_Position = vec4(centers[i], 0, 1); EmitVertex(); }
		i++;
	} EndPrimitive();

	gs_out.Color = vec3(1, 0, 0);
	i = 0; while (i < 8) {
		if (pp_arr[i].pos.x == 255) { EndPrimitive(); }
		else { gl_Position = vec4(points0[i], 0, 1); EmitVertex(); }
		i++;
	} EndPrimitive();

	gs_out.Color = vec3(0, 0, 1);
	i = 0; while (i < 8) {
		if (pp_arr[i].pos.x == 255) { EndPrimitive(); }
		else { gl_Position = vec4(points1[i], 0, 1); EmitVertex(); }
		i++;
	} EndPrimitive();

	gs_out.Color = vec3(1, 0.25, 0.25);
	i = 0; while (i < 8) {
		if (pp_arr[i].pos.x == 255) { EndPrimitive(); }
		else { gl_Position = vec4(points2[i], 0, 1); EmitVertex(); }
		i++;
	} EndPrimitive();

	gs_out.Color = vec3(0.25, 0.25, 1);
	i = 0; while (i < 8) {
		if (pp_arr[i].pos.x == 255) { EndPrimitive(); }
		else { gl_Position = vec4(points3[i], 0, 1); EmitVertex(); }
		i++;
	} EndPrimitive();

	/*gs_out.Color = vec3(0, 1, 0);
	i = 0; while (i < 8) {
		if (pp_arr[i].pos.x == 255) { EndPrimitive(); }
		else {
			gl_Position = vec4(points0[i], 0, 1); EmitVertex();
			gl_Position = vec4(points1[i], 0, 1); EmitVertex();
		}
		i++;
	} EndPrimitive();*/
}

void main()
{
	gs_out.Color = gs_in[0].Color;
	gs_out.Absolut = vec3(0, 0, 0);
	gs_out.Normal = vec3(0, 0, 0);

	pallet Pallet = pallets[gs_in[0].Char];
	pallet_point[8] pp;
	pp[0] = Pallet.p0;
	pp[1] = Pallet.p1;
	pp[2] = Pallet.p2;
	pp[3] = Pallet.p3;
	pp[4] = Pallet.p4;
	pp[5] = Pallet.p5;
	pp[6] = Pallet.p6;
	pp[7] = Pallet.p7;

	EmitPallet(pp);
}
