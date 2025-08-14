#version 430

layout(triangles) in;
layout(line_strip, max_vertices = 4) out;

layout(std430, binding = 0) buffer BufferColor {
	uint color[];
};



in Vert {
	vec3 Original;
	vec3 Absolute;
	vec3 Relative;
} gs_in[];

out GeomLine {
	vec3 Color;
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

void main()
{
	//uint uCol = color[gl_PrimitiveIDIn];
	//gs_out.Color.r = ((uCol & 0xFF0000) >> 16) / 255.0;
	//gs_out.Color.g = ((uCol & 0x00FF00) >> 8) / 255.0;
	//gs_out.Color.b = ((uCol & 0x0000FF) >> 0) / 255.0;
	gs_out.Color = vec3(1, 1, 1);

	EmitCorner(0);
	EmitCorner(1);
	EmitCorner(2);
	EmitCorner(0);
}
