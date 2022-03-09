using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.VertexShaders
{
    public static class CoordinateSystemVertexShader
    {
        public const ShaderType Type = ShaderType.VertexShader;
        public const string Code = @"
                #version 330 core

                layout (location = 0) in vec3 position;
                layout (location = 1) in vec4 color;

                out vec4 vertex_color;

                uniform mat4 view;
                uniform mat4 projection;

                void main(void)
                {
                    vertex_color = color;
                    gl_Position = projection * view * vec4(position, 1.0f);
                }
                ";
    }
}