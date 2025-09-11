#version 430



struct pallet_point
{
	vec2 pos;
	vec2 dir0;
	vec2 per0;
	vec2 off0;
	vec2 dir1;
	vec2 per1;
	vec2 off1;
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

const vec2 SizeRatio = vec2(0.5, 1.0);



layout(points) in;
layout(triangle_strip, max_vertices = 64) out;
//layout(line_strip, max_vertices = 64) out;

layout(std430, binding = 0) buffer BufferChars {
	pallet pallets[];
};



in Text_Char {
	uint Char;

	vec2 Anchor;
	vec2 Position;
	vec2 Offset;

	float Height;
	float Padding;
	float Thick;

	vec3 Color;
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Absolut;
	vec3 Normal;
} gs_out;



vec2 IntersectLine(vec2 pos0, vec2 dir0, vec2 pos1, vec2 dir1)
{
	vec2 dst0 = pos0 + dir0;
	vec2 dst1 = pos1 + dir1;

	float f_0 = (pos0.x * dst0.y) - (pos0.y * dst0.x);
	float f_1 = (pos1.x * dst1.y) - (pos1.y * dst1.x);
	float div = (dir0.x * dir1.y) - (dir0.y * dir1.x);

	return vec2(
		((f_0 * dir1.x) - (f_1 * dir0.x)) / div,
		((f_0 * dir1.y) - (f_1 * dir0.y)) / div
	);
}

void EmitPallet(pallet_point[8] pp_arr, uint skips)
{
	vec2[8] centers;
	vec2[8] points0;
	vec2[8] points1;

	vec2 normPos;
	normPos = gs_in[0].Position + (gs_in[0].Offset * (gs_in[0].Height * SizeRatio + gs_in[0].Padding));
	normPos = normPos / screenRatios[0];
	normPos = normPos * 2;
	//normPos = normPos + (gs_in[0].Anchor * 2) - 1;
	normPos = normPos + gs_in[0].Anchor;

	vec2 normSize = vec2(gs_in[0].Height);
	normSize = normSize / screenRatios[0];

	vec2 normThick = vec2(gs_in[0].Thick);
	normThick = normThick / screenRatios[0];

	for (int i = 0; i < 8; i++)
	{
		pallet_point pp = pp_arr[i];

		vec2 hit0 = IntersectLine(pp.off0 - pp.per0, -pp.dir0, pp.off1 - pp.per1, -pp.dir1);
		vec2 hit1 = IntersectLine(pp.off0 + pp.per0, +pp.dir0, pp.off1 + pp.per1, +pp.dir1);
		vec2 center = normPos + (pp.pos * normSize);

		centers[i] = center;
		points0[i] = center + (hit0 * normThick);
		points1[i] = center + (hit1 * normThick);
	}

//	gs_out.Color = vec3(0, 0, 0);
//	for (int i = 0; i < 8; i++) {
//		gl_Position = vec4(centers[i], 0, 1); EmitVertex();
//		if ((skips & (1 << i)) != 0) { EndPrimitive(); }
//	} EndPrimitive();
//
//	gs_out.Color = vec3(1, 0, 0);
//	for (int i = 0; i < 8; i++) {
//		gl_Position = vec4(points0[i], 0, 1); EmitVertex();
//		if ((skips & (1 << i)) != 0) { EndPrimitive(); }
//	} EndPrimitive();
//
//	gs_out.Color = vec3(0, 0, 1);
//	for (int i = 0; i < 8; i++) {
//		gl_Position = vec4(points1[i], 0, 1); EmitVertex();
//		if ((skips & (1 << i)) != 0) { EndPrimitive(); }
//	} EndPrimitive();

	for (int i = 0; i < 8; i++) {
		gl_Position = vec4(points0[i], 0, 1); EmitVertex();
		gl_Position = vec4(points1[i], 0, 1); EmitVertex();
		if ((skips & (1 << i)) != 0) { EndPrimitive(); }
	} EndPrimitive();
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

	EmitPallet(pp, Pallet.skips);
}
