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
                    vec3 position;
                    vec3 normal;
                    vec4 color;
                } vertex;

                out vec4 color;

                uniform vec3 light_position;
                uniform vec4 light_color;
                uniform vec3 view_position;

                const float AMBIENT_STRENGTH = 0.3f;
                const float SPECULAR_STRENGTH = 0.8f;
                const int MATERIAL_SHININESS = 32;

                vec4 ambient_lighting();
                vec4 diffuse_lighting();
                vec4 specular_lighting();

                void main(void)
                {
                    color = (ambient_lighting() + diffuse_lighting() + specular_lighting()) * vertex.color;
                }

                vec4 ambient_lighting()
                {
                    return (AMBIENT_STRENGTH * light_color);
                }

                vec4 diffuse_lighting()
                {
                    vec3 light_direction = normalize(light_position - vertex.position);
                    return (max(dot(vertex.normal, light_direction), 0.0f) * light_color);
                }

                vec4 specular_lighting()
                {
                    vec3 light_direction = normalize(light_position - vertex.position);
                    vec3 view_direction = normalize(view_position - vertex.position);
                    vec3 reflect_direction = reflect(-light_direction, vertex.normal);
                    return (SPECULAR_STRENGTH * light_color * pow(max(dot(view_direction, reflect_direction), 0.0f), MATERIAL_SHININESS));
                }
                ";
    }
}