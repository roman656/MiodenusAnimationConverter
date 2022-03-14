using System.Collections.Generic;
using System.Linq;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Scene.Models;
using MiodenusAnimationConverter.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class ModelGroup
    {
        public readonly Dictionary<string, Model> Models = new ();

        public void Draw(in ShaderProgram shaderProgram, in Camera camera, PrimitiveType mode = PrimitiveType.Triangles)
        {
            for (var i = 0; i < Models.Count; i++)
            {
                Models.Values.ElementAt(i).Draw(shaderProgram, camera, mode);
            }
        }

        public void InitializeVao()
        {
            for (var i = 0; i < Models.Count; i++)
            {
                Models.Values.ElementAt(i).InitializeVao();
            }
        }
        
        public void DeleteVao()
        {
            for (var i = 0; i < Models.Count; i++)
            {
                Models.Values.ElementAt(i).DeleteVao();
            }
        }

        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            for (var i = 0; i < Models.Count; i++)
            {
                Models.Values.ElementAt(i).Pivot.GlobalMove(deltaX, deltaY, deltaZ);
            }
        }

        public void Rotate(float angle, Vector3 vector)
        {
            for (var i = 0; i < Models.Count; i++)
            {
                Models.Values.ElementAt(i).Pivot.GlobalRotate(angle, vector);
            }
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            for (var i = 0; i < Models.Count; i++)
            {
                Models.Values.ElementAt(i).Scale = new Vector3(scaleX, scaleY, scaleZ);
            }
        }
    }
}