using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.FragmentShaders
{
    public static class GridFragmentShader
    {
        public const ShaderType Type = ShaderType.FragmentShader;
        public const string Code = @"
                #version 330 core

                #define TWO_PI 6.28318530718f

                in vec4 vertex_color;

                out vec4 color;

                uniform float time;
                uniform float color_rotation_speed;
                uniform bool use_vertex_color;
                uniform vec2 resolution;

                vec3 hsb2rgb(const in vec3 color);

                void main(void)
                {
                    if (use_vertex_color)
                    {
                        color = vertex_color;
                    }
                    else
                    {
                        vec2 coordinates = gl_FragCoord.xy / resolution.xy;

                        // Use polar coordinates.
                        vec2 to_center = vec2(0.5f).xy - coordinates.xy;
                        float angle = atan(to_center.y, to_center.x) + time * color_rotation_speed;
                        float radius = length(to_center) * 2.0f;

                        // Map the angle (-PI to PI) to the Hue (from 0 to 1) and the Saturation to the radius.
                        color = vec4(hsb2rgb(vec3(angle / TWO_PI + 0.5f, radius, 1.0f)), 1.0f);
                    }
                }

                vec3 hsb2rgb(const in vec3 color)
                {
                    vec3 rgb = clamp(abs(mod(color.r * 6.0f + vec3(0.0f, 4.0f, 2.0f), 6.0f) - 3.0f) - 1.0f, 0.0f, 1.0f);
                    rgb = rgb * rgb * (3.0f - 2.0f * rgb);
                    return color.b * mix(vec3(0.8f), rgb, color.g);
                }
                ";
    }
}