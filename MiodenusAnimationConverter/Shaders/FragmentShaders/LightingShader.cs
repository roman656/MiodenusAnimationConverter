using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.FragmentShaders
{
    public static class LightingShader
    {
        public const ShaderType Type = ShaderType.FragmentShader;
        public const string Code = @"
                #version 330 core

                in vertex_shader_output
                {
                    vec4 position;
                    vec4 normal;
                    vec4 color;
                } vertex;

                out vec4 color;

                uniform vec3 light_position_1;
                uniform vec4 light_color_1;
                uniform vec3 light_position_2;
                uniform vec4 light_color_2;
                uniform vec3 view_position;

                const float AMBIENT_STRENGTH = 0.3f;
                const float SPECULAR_STRENGTH = 0.8f;
                const int MATERIAL_SHININESS = 32;

                vec4 ambient_lighting(const in vec4 light_color);
                vec4 diffuse_lighting(const in vec3 light_position, const in vec4 light_color);
                vec4 specular_lighting(const in vec3 light_position, const in vec4 light_color);

                void main(void)
                {
                    color = (ambient_lighting(light_color_1) + diffuse_lighting(light_position_1, light_color_1) + specular_lighting(light_position_1, light_color_1) + ambient_lighting(light_color_2) + diffuse_lighting(light_position_2, light_color_2) + specular_lighting(light_position_2, light_color_2)) * vertex.color;
                }

                vec4 ambient_lighting(const in vec4 light_color)
                {
                    return (AMBIENT_STRENGTH * light_color);
                }

                vec4 diffuse_lighting(const in vec3 light_position, const in vec4 light_color)
                {
                    vec4 light_direction = normalize(vec4(light_position, 0.0f) - vertex.position);
                    return (max(dot(vertex.normal, light_direction), 0.0f) * light_color);
                }

                vec4 specular_lighting(const in vec3 light_position, const in vec4 light_color)
                {
                    vec4 light_direction = normalize(vec4(light_position, 0.0f) - vertex.position);
                    vec4 view_direction = normalize(vec4(view_position, 0.0f) - vertex.position);
                    vec4 reflect_direction = reflect(-light_direction, vertex.normal);
                    return (SPECULAR_STRENGTH * light_color * pow(max(dot(view_direction, reflect_direction), 0.0f), MATERIAL_SHININESS));
                }
                ";
    }
}