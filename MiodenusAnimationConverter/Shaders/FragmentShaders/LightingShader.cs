using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.FragmentShaders
{
    public static class LightingShader
    {
        public const ShaderType Type = ShaderType.FragmentShader;
        public const string Code = @"
                #version 330 core

                in vec4 vertex_color;
                in vec3 vertex_normal;
                in vec3 vertex_position;

                out vec4 color;

                uniform vec3 light_position;

                vec4 ambient_lighting(const in vec4 light_color, const in float ambient_strength);
                vec4 point_lighting(const in vec4 light_color);

                void main(void)
                {
                    vec4 light_color = vec4(1.0f);
                    float ambient_strength = 0.2f;

                    color = (ambient_lighting(light_color, ambient_strength) + point_lighting(light_color)) * vertex_color;
                }

                vec4 ambient_lighting(const in vec4 light_color, const in float ambient_strength)
                {
                    return (ambient_strength * light_color);
                }

                vec4 point_lighting(const in vec4 light_color)
                {
                    vec3 light_direction = normalize(light_position - vertex_position);
                    return (max(dot(vertex_normal, light_direction), 0.0f) * light_color);
                }
                ";
    }
}