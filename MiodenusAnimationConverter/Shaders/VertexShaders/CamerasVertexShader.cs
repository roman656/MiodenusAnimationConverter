namespace MiodenusAnimationConverter.Shaders.VertexShaders;

using OpenTK.Graphics.OpenGL;

public static class CamerasVertexShader
{
    public const ShaderType Type = ShaderType.VertexShader;
    public const string Code =
    @"
        #version 330 core

        layout (location = 0) in float fov;
        layout (location = 1) in float distance_to_the_near_clip_plane;
        layout (location = 2) in float distance_to_the_far_clip_plane;
        layout (location = 3) in vec3 front;
        layout (location = 4) in vec3 right;
        layout (location = 5) in vec3 up;
        layout (location = 6) in vec3 position;

        out VertexShaderOutput
        {
            float fov;
            float distance_to_the_near_clip_plane;
            float distance_to_the_far_clip_plane;
            vec3 front;
            vec3 right;
            vec3 up;
            vec3 position;
        } camera;

        uniform mat4 view;
        uniform mat4 projection;

        void main(void)
        {
            camera.fov = fov;
            camera.distance_to_the_near_clip_plane = distance_to_the_near_clip_plane;
            camera.distance_to_the_far_clip_plane = distance_to_the_far_clip_plane;
            camera.front = front;
            camera.right = right;
            camera.up = up;
            camera.position = position;
                    
            gl_Position = projection * view * vec4(position, 1.0f);
        }              
    ";
}