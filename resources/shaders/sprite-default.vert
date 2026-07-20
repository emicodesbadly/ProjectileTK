#version 460
layout(location = 0) in vec2 a_pos;
layout(location = 1) in vec2 a_uv;
layout(location = 2) in mat4 a_trans;

layout(location = 0) uniform mat4 transform;

out vec2 v_uv;

void main()
{
	gl_Position = vec4(a_pos, 0.0f, 1.0f) * a_trans;
	v_uv = vec2(a_uv.x, a_uv.y);
}
