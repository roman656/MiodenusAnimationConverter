using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.GeometryShaders
{
    public static class NormalsShader
    {
        public const ShaderType Type = ShaderType.GeometryShader;
        public const string Code = @"
                #version 330 core

                layout (triangles) in;
                layout (line_strip, max_vertices = 6) out;

                in VertexShaderOutput
                {
                    vec3 position;
                    vec3 normal;
                    vec4 color;
                } vertexes[];

                uniform mat4 view;
                uniform mat4 projection;
                 
                const float MAGNITUDE = 0.05f;
                                  
                void generate_normal_line(const in int vertex_index);
              
                void main(void)
                {
                    generate_normal_line(0);
                    generate_normal_line(1);
                    generate_normal_line(2);
                }

                void generate_normal_line(const in int vertex_index)
                {
                    gl_Position = projection * view * vec4(vertexes[vertex_index].position, 1.0f);
                    EmitVertex();

                    gl_Position = projection * view * vec4((vertexes[vertex_index].position + vertexes[vertex_index].normal * MAGNITUDE), 1.0f);
                    EmitVertex();

                    EndPrimitive();
                }
                ";
    }
}