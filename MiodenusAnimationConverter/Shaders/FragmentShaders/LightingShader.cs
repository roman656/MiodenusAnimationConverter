using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.FragmentShaders
{
    public static class LightingShader
    {
        public const ShaderType Type = ShaderType.FragmentShader;
        public const string Code = @"
                #version 330 core

                #define LIGHT_POINTS_AMOUNT 8

                struct LightPoint
                {
                    bool is_active;
                    bool use_attenuation;
                    vec3 position;
                    vec4 color;                   
                    vec4 ambient_component;
                    float diffuse_strength;
                    float specular_strength;
                    float constant_factor;
                    float linear_factor;
                    float quadratic_factor;
                };

                in VertexShaderOutput
                {
                    vec3 position;
                    vec3 normal;
                    vec4 color;
                } vertex;

                out vec4 color;

                uniform vec3 view_position;
                uniform LightPoint light_points[LIGHT_POINTS_AMOUNT];

                const int MATERIAL_SHININESS = 32;

                vec4 diffuse_lighting(const in vec3 light_direction, const in vec3 light_position, const in vec4 light_color, const in float diffuse_strength);
                vec4 specular_lighting(const in vec3 light_direction, const in vec3 light_position, const in vec4 light_color, const in float specular_strength);
                vec4 calculate_light_point(const in LightPoint light);

                void main(void)
                {
                    vec4 result = vec4(0.0f);

                    for (int i = 0; i < LIGHT_POINTS_AMOUNT; i++)
                    {
                        result += calculate_light_point(light_points[i]);
                    }

                    color = result * vertex.color;
                }

                vec4 diffuse_lighting(const in vec3 light_direction, const in vec3 light_position, const in vec4 light_color, const in float diffuse_strength)
                {
                    return (diffuse_strength * light_color * max(dot(vertex.normal, light_direction), 0.0f));
                }

                vec4 specular_lighting(const in vec3 light_direction, const in vec3 light_position, const in vec4 light_color, const in float specular_strength)
                {
                    vec3 view_direction = normalize(view_position - vertex.position);
                    vec3 reflect_direction = reflect(-light_direction, vertex.normal);
                    return (specular_strength * light_color * pow(max(dot(view_direction, reflect_direction), 0.0f), MATERIAL_SHININESS));
                }

                vec4 calculate_light_point(const in LightPoint light)
                {
                    if (light.is_active)
                    {
                        vec3 light_direction = normalize(light.position - vertex.position);
                        vec4 ambient = light.ambient_component;
                        vec4 diffuse = diffuse_lighting(light_direction, light.position, light.color, light.diffuse_strength);
                        vec4 specular = diffuse_lighting(light_direction, light.position, light.color, light.specular_strength);
                        
                        if (light.use_attenuation)
                        {
                            float distance = length(light.position - vertex.position);
                            float attenuation = 1.0f / (light.constant_factor + light.linear_factor * distance + light.quadratic_factor * (distance * distance));    
                            
                            ambient *= attenuation;
                            diffuse *= attenuation;
                            specular *= attenuation;
                        }
                        
                        return (ambient + diffuse + specular);
                    }
                    else
                    {
                        return vec4(0.0f);
                    }              
                }
                ";
    }
}