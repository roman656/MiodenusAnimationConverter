using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders.VertexShaders
{
    public static class CamerasVertexShader
    {
        public const ShaderType Type = ShaderType.VertexShader;
        public const string Code = @"
                #version 330 core

                layout (location = 0) in vec3 position;
                layout (location = 1) in vec3 normal;
                layout (location = 2) in vec4 color;

                layout (location = 3) in vec3 t_location;
                layout (location = 4) in vec4 t_rotation;
                layout (location = 5) in vec3 t_scale;

                out VertexShaderOutput
                {
                    vec3 position;
                    vec3 normal;
                    vec4 color;
                } transformed_vertex;

                uniform mat4 view;
                uniform mat4 projection;

                vec3 move(const in vec3 vector, const in vec3 delta);
                vec3 rotate(const in vec3 vector, const in vec4 quaternion);
                vec3 scale(const in vec3 vector, const in vec3 new_scale);
                void transform(inout vec3 vertex_position, inout vec3 vertex_normal);

                void main(void)
                {
                    vec3 t_position = vec3(position);
                    vec3 t_normal = vec3(normal);

                    transform(t_position, t_normal);                 
                    
                    transformed_vertex.position = t_position;
                    transformed_vertex.normal = t_normal;
                    transformed_vertex.color = color;

                    gl_Position = projection * view * vec4(t_position, 1.0f);
                }

                vec3 move(const in vec3 vector, const in vec3 delta)
                {
                    vec3 result = vec3(vector);

                    result.x += delta.x;
                    result.y += delta.y;
                    result.z += delta.z;

                    return result;
                }

                vec3 rotate(const in vec3 vector, const in vec4 quaternion)
                {
                    return (vector + 2.0f * cross(quaternion.xyz, cross(quaternion.xyz, vector) + quaternion.w * vector));
                }

                vec3 scale(const in vec3 vector, const in vec3 new_scale)
                {
                    vec3 result = vec3(vector);

                    result.x *= new_scale.x;
                    result.y *= new_scale.y;
                    result.z *= new_scale.z;

                    return result;
                }

                void transform(inout vec3 vertex_position, inout vec3 vertex_normal)
                {
                    vertex_position = move(vertex_position, t_location);
                    vertex_position = rotate(vertex_position, t_rotation);
                    vertex_position = scale(vertex_position, t_scale);

                    //vertex_normal = move(vertex_normal, t_location);
                    vertex_normal = rotate(vertex_normal, t_rotation);
                    vertex_normal = scale(vertex_normal, t_scale);
                    vertex_normal = normalize(vertex_normal);
                }
                ";
    }
}