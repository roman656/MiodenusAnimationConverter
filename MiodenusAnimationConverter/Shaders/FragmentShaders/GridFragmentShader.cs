using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.FragmentShaders
{
    public static class GridFragmentShader
    {
        public const ShaderType Type = ShaderType.FragmentShader;
        public const string Code = @"
                #version 330 core

                in VertexShaderOutput
                {
                    bool is_visible;
                    vec4 color;
                } vertex;

                out vec4 color;

                void main(void)
                {
                    if (vertex.is_visible)
                    {
                        color = vertex.color;
                    }
                    else
                    {
                        discard;
                    }
                }
                ";
    }
}