using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.FragmentShaders
{
    public static class LightingShader
    {
        public const ShaderType Type = ShaderType.FragmentShader;
        public const string Code = @"
                #version 330 core

                in vec4 vs_color;
                in vec3 original_normal;
                in vec3 transformed_normal;
                out vec4 color;

                void main(void)
                {
                    float lighting = abs(dot(transformed_normal, vec3(0,0,-1)));
                    color = vs_color * 1; //lighting;
                }
                ";
    }
}