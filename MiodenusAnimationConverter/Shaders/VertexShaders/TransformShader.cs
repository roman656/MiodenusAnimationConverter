using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.VertexShaders
{
    public static class TransformShader
    {
        public const ShaderType Type = ShaderType.VertexShader;
        public const string Code = @"
                #version 330 core

                layout (location = 0) in vec3 position;
                layout (location = 1) in vec3 normal;
                layout (location = 2) in vec4 color;

                out vec4 vertex_color;
                out vec3 original_normal;
                out vec3 transformed_normal;

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;

                vec3 move(in vec3 vertex_position, in vec3 delta)
                {
                    vec3 result = vec3(vertex_position);

                    result.x += delta.x;
                    result.y += delta.y;
                    result.z += delta.z;

                    return result;
                }

                vec3 rotate(in vec3 vertex_position, in vec3 delta)
                {
                    vec3 result = vec3(vertex_position);
                    return result;
                }

                vec3 scale(in vec3 vertex_position, in vec3 delta)
                {
                    vec3 result = vec3(vertex_position);

                    result.x *= delta.x;
                    result.y *= delta.y;
                    result.z *= delta.z;

                    return result;
                }

                vec4 transform(in vec3 vertex_position)
                {
                    vec3 transformed_position;

                    transformed_position = move(vertex_position, vec3(0.0));
                    transformed_position = scale(transformed_position, vec3(0.01));
    
                    return vec4(transformed_position, 1.0);
                }

                void main(void)
                {
                    gl_Position = projection * view * model * transform(position);

                    vertex_color = color;
                    original_normal = vec3(color);
                    mat3 normal_matrix = transpose(inverse(mat3(view * model)));
                    transformed_normal = normal_matrix * original_normal;
                }
                ";
    }
}