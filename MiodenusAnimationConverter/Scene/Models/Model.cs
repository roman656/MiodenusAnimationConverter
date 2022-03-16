using System.Collections.Generic;
using System.Linq;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Scene.Models.Meshes;
using MiodenusAnimationConverter.Shaders;
using NLog;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene.Models
{
    public class Model
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public readonly Dictionary<string, Mesh> Meshes;
        public readonly Pivot Pivot = new ();
        private Vector3 _scale = Vector3.One;
        public bool IsVisible = true;

        /*
         * На каждую модель должен быть отдельный набор мешей (входной словарь не использовать больше 1 раза для
         * создания моделей).
         */
        public Model(in Dictionary<string, Mesh> meshes)
        {
            Meshes = meshes.ToDictionary(entry => entry.Key, entry => entry.Value);
        }
        
        public void Scale(float scaleX = 1.0f, float scaleY = 1.0f, float scaleZ = 1.0f)
        {
            if (scaleX > 0.0f && scaleY > 0.0f && scaleZ > 0.0f)
            {
                _scale.X *= scaleX;
                _scale.Y *= scaleY;
                _scale.Z *= scaleZ;
            }
            else
            {
                Logger.Warn("Wrong scale parameters. Expected: values greater than 0 for X, Y and Z"
                        + $" components. Got: ({scaleX}; {scaleY}; {scaleZ}). Scale was not changed.");
            }
        }

        public void ResetScale() => _scale = Vector3.One;
        public Vector3 GetScale() => _scale;

        public void InitializeVao()
        {
            for (var i = 0; i < Meshes.Count; i++)
            {
                Meshes.Values.ElementAt(i).InitializeVao();
            }
        }

        public void DeleteVao()
        {
            for (var i = 0; i < Meshes.Count; i++)
            {
                Meshes.Values.ElementAt(i).DeleteVao();
            }
        }

        public void Draw(in ShaderProgram shaderProgram, in Camera camera, PrimitiveType mode = PrimitiveType.Triangles)
        {
            if (IsVisible)
            {
                shaderProgram.SetMatrix4("view", camera.ViewMatrix, false);
                shaderProgram.SetMatrix4("projection", camera.ProjectionMatrix, false);
                shaderProgram.SetVector3("model_pivot.position", Pivot.Position);
                shaderProgram.SetVector4("model_pivot.rotation", new Vector4(Pivot.Rotation.Xyz, Pivot.Rotation.W));
                shaderProgram.SetVector3("model_scale", _scale);
                
                for (var i = 0; i < Meshes.Count; i++)
                {
                    Meshes.Values.ElementAt(i).Draw(shaderProgram, mode);
                }
            }
        }
    }
}