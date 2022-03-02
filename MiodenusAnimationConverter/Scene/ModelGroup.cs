using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Scene.Models;
using MiodenusAnimationConverter.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class ModelGroup
    {
        public readonly List<Model> Models = new ();

        public void Draw(in ShaderProgram shaderProgram, in Camera camera, PrimitiveType mode = PrimitiveType.Triangles)
        {
            for (var i = 0; i < Models.Count; i++)
            {
                Models[i].Draw(shaderProgram, camera, mode);
            }
        }

        public void Initialize()
        {
            for (var i = 0; i < Models.Count; i++)
            {
                Models[i].InitializeVao();
            }
        }
        
        public void Delete()
        {
            for (var i = 0; i < Models.Count; i++)
            {
                Models[i].DeleteVao();
            }
        }

        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            for (var i = 0; i < Models.Count; i++)
            {
                Models[i].Move(deltaX, deltaY, deltaZ);
            }
        }

        public void Rotate(float angle, Vector3 vector)
        {
            for (var i = 0; i < Models.Count; i++)
            {
                Models[i].Rotate(angle, vector);
            }
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            for (var i = 0; i < Models.Count; i++)
            {
                Models[i].Scale(scaleX, scaleY, scaleZ);
            }
        }
    }
}