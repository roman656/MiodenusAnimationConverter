using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.VertexShaders
{
    public static class TransformShader
    {
        public const ShaderType Type = ShaderType.VertexShader;
        public const string Code = @"
                #version 330 core

                layout (location = 0) in vec4 position;
                layout (location = 1) in vec4 color;

                out vec4 vs_color;
                out vec3 original_normal;
                out vec3 transformed_normal;

                uniform mat4 model;
                uniform mat4 view;
                uniform mat4 projection;

                vec4 move(in vec4 vertex_position, in vec3 delta)
                {
                    vec4 result = vec4(vertex_position);

                    result.x += delta.x;
                    result.y += delta.y;
                    result.z += delta.z;

                    return result;
                }

                vec4 rotate(in vec4 vertex_position, in vec3 delta)
                {
                    vec4 result = vec4(vertex_position);

                    

                    return result;
                }

                vec4 scale(in vec4 vertex_position, in vec3 scale)
                {
                    vec4 result = vec4(vertex_position);

                    result.x *= scale.x;
                    result.y *= scale.y;
                    result.z *= scale.z;

                    return result;
                }

                vec4 transform(in vec4 vertex_position)
                {
                    vec4 result;

                    result = move(vertex_position, vec3(0.0));
                    //result = rotate(result, vec3(0.0));
                    result = scale(result, vec3(0.01));
    
                    return result;
                }

                void main(void)
                {
                    gl_Position = projection * view * model * transform(position);

                    vs_color = color;
                    original_normal = vec3(color);
                    mat3 normal_matrix = transpose(inverse(mat3(view * model)));
                    transformed_normal = normal_matrix * original_normal;
                }
                ";
    }
}