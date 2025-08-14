#version 430

layout(triangles) in;
layout(triangle_strip, max_vertices = 3) out;

layout(std430, binding = 0) buffer BufferColor {
	uint color[];
};



in Vert {
	vec3 Original;
	vec3 Absolute;
	vec3 Relative;
} gs_in[];

out Geom {
	vec3 Color;
	vec3 Normal;
	vec3 Original;
	vec3 Absolute;
	vec3 Relative;
} gs_out;



void EmitCorner(int i)
{
	gs_out.Original = gs_in[i].Original;
	gs_out.Absolute = gs_in[i].Absolute;
	gs_out.Relative = gs_in[i].Relative;
	gl_Position = gl_in[i].gl_Position;
	EmitVertex();
}

void CalcNormal(vec3 a, vec3 b, vec3 c)
{
	vec3 p1, p2;
	p1 = b - a;
	p2 = c - a;

	vec3 n;
	n.x = p1.y * p2.z - p1.z * p2.y;
	n.y = p1.z * p2.x - p1.x * p2.z;
	n.z = p1.x * p2.y - p1.y * p2.x;
	gs_out.Normal = normalize(n);
}

void main()
{
	uint uCol = color[gl_PrimitiveIDIn];
	gs_out.Color.r = ((uCol & 0xFF0000) >> 16) / 255.0;
	gs_out.Color.g = ((uCol & 0x00FF00) >> 8) / 255.0;
	gs_out.Color.b = ((uCol & 0x0000FF) >> 0) / 255.0;

	CalcNormal(gs_in[0].Absolute, gs_in[1].Absolute, gs_in[2].Absolute);

	EmitCorner(0);
	EmitCorner(1);
	EmitCorner(2);
	EndPrimitive();
}
