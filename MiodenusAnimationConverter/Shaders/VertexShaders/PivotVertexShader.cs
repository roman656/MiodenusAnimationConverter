using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.VertexShaders
{
    public static class PivotVertexShader
    {
        public const ShaderType Type = ShaderType.VertexShader;
        public const string Code = @"
                #version 330 core

                struct Pivot
                {
                    vec3 position;
                    vec4 rotation;
                };

                layout (location = 0) in vec3 position;
                layout (location = 1) in vec4 color;

                out vec4 vertex_color;

                uniform mat4 view;
                uniform mat4 projection;
                uniform Pivot pivot;

                vec3 rotate(const in vec3 vector, const in vec4 quaternion);    
                void transform(inout vec3 vertex_position);

                void main(void)
                {
                    vec3 t_position = vec3(position);

                    transform(t_position);                                    
                    vertex_color = color;
                    gl_Position = projection * view * vec4(t_position, 1.0f);
                }

                vec3 rotate(const in vec3 vector, const in vec4 quaternion)
                {
                    return (vector + 2.0f * cross(quaternion.xyz, cross(quaternion.xyz, vector) + quaternion.w * vector));
                }

                void transform(inout vec3 vertex_position)
                {
                    vertex_position = rotate(vertex_position, pivot.rotation);
                    vertex_position = vertex_position + pivot.position;
                }
                ";
    }
}