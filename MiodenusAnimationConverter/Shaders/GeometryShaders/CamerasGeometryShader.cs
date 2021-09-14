using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.GeometryShaders
{
    public static class CamerasGeometryShader
    {
        public const ShaderType Type = ShaderType.GeometryShader;
        public const string Code = @"
                #version 330 core

                layout (points) in;
                layout (line_strip, max_vertices = 6) out;

                in VertexShaderOutput
                {
                    float fov;
                    float distance_to_the_near_clip_plane;
                    float distance_to_the_far_clip_plane;
                    vec3 front;
                    vec3 right;
                    vec3 up;
                    vec3 position;
                } cameras[];

                out vec4 vertex_color;

                uniform mat4 view;
                uniform mat4 projection;
                 
                const float MAGNITUDE = 0.03f;
                                  
                void create_local_coordinate_system(const in vec3 front, const in vec3 right, const in vec3 up, const in vec3 position);
              
                void main(void)
                {
                    create_local_coordinate_system(cameras[0].front, cameras[0].right, cameras[0].up, cameras[0].position);
                }

                void create_local_coordinate_system(const in vec3 front, const in vec3 right, const in vec3 up, const in vec3 position)
                {
                    vertex_color = vec4(1.0f, 0.0f, 0.0f, 1.0f);
                    gl_Position = projection * view * vec4(position, 1.0f);
                    EmitVertex();

                    vertex_color = vec4(0.0f, 1.0f, 0.0f, 1.0f);
                    gl_Position = projection * view * vec4((position + front * MAGNITUDE), 1.0f);
                    EmitVertex();

                    vertex_color = vec4(1.0f, 0.0f, 0.0f, 1.0f);
                    gl_Position = projection * view * vec4(position, 1.0f);
                    EmitVertex();

                    vertex_color = vec4(0.0f, 1.0f, 0.0f, 1.0f);
                    gl_Position = projection * view * vec4((position + right * MAGNITUDE), 1.0f);
                    EmitVertex();

                    vertex_color = vec4(1.0f, 0.0f, 0.0f, 1.0f);
                    gl_Position = projection * view * vec4(position, 1.0f);
                    EmitVertex();

                    vertex_color = vec4(0.0f, 1.0f, 0.0f, 1.0f);
                    gl_Position = projection * view * vec4((position + up * MAGNITUDE), 1.0f);
                    EmitVertex();

                    EndPrimitive();
                }
                ";
    }
}