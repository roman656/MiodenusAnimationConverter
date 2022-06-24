namespace MiodenusAnimationConverter.Shaders.FragmentShaders;

using OpenTK.Graphics.OpenGL;

public static class GridFragmentShader
{
    public const ShaderType Type = ShaderType.FragmentShader;
    public const string Code =
    @"
        #version 330 core

        in vec4 vertex_color;

        out vec4 color;

        void main(void)
        {
            color = vertex_color;
        }
    ";
}