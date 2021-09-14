using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.FragmentShaders
{
    public static class ColorShader
    {
        public const ShaderType Type = ShaderType.FragmentShader;
        public const string Code = @"
                #version 330 core

                out vec4 color;
                 
                void main(void)
                {
                    color = vec4(1.0f, 1.0f, 0.0f, 1.0f);
                } 
                ";
    }
}