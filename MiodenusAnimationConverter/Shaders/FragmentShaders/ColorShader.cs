using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.FragmentShaders
{
    public static class ColorShader
    {
        public const ShaderType Type = ShaderType.FragmentShader;
        public const string Code = @"
                #version 330 core

                in vec4 color;
                out vec4 result_color;
                 
                void main(void)
                {
                    result_color = color;
                } 
                ";
    }
}