using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.GeometryShaders
{
    public static class NormalsShader
    {
        public const ShaderType Type = ShaderType.GeometryShader;
        public const string Code = @"
                #version 330 core

                layout (triangles) in;
                layout (line_strip) out;
                layout (max_vertices = 6) out;

                in vertex_shader_output
                {
                    vec4 position;
                    vec4 normal;
                    vec4 color;
                } vertexes[];

                uniform mat4 view;
                uniform mat4 projection;
                 
                const float MAGNITUDE = 0.15f;
                                  
                void generate_normal_line(const in int vertex_index);
              
                void main(void)
                {
                    generate_normal_line(0);
                    generate_normal_line(1);
                    generate_normal_line(2);
                }

                void generate_normal_line(const in int vertex_index)
                {
                    gl_Position = projection * view * vertexes[vertex_index].position;
                    EmitVertex();

                    gl_Position = projection * view * (vertexes[vertex_index].position + vertexes[vertex_index].normal * MAGNITUDE);
                    EmitVertex();

                    EndPrimitive();
                }
                ";
    }
}