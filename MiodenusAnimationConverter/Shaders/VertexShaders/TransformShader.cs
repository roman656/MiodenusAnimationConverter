using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.VertexShaders
{
    public static class TransformShader
    {
        public const ShaderType Type = ShaderType.VertexShader;
        public const string Code = @"
                #version 330 core

                layout (location = 0) in vec4 position;
                layout (location = 1) in vec4 color;

                out vec4 vs_color;
                out vec3 original_normal;
                out vec3 transformed_normal;

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;

                void main(void)
                {
                    gl_Position = projection * view * model * position;
                    vs_color = color;
                    original_normal = vec3(color);
                    mat3 normal_matrix = transpose(inverse(mat3(view * model)));
                    transformed_normal = normal_matrix * original_normal;
                }
                ";
    }
}