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
        public readonly Pivot Pivot;
        private Vector3 _scale = Vector3.One;
        public bool IsVisible = true;
        
        public Model(in Dictionary<string, Mesh> meshes) : this(meshes, new Pivot()) {}
        public Model(in Dictionary<string, Mesh> meshes, in Pivot pivot) : this(meshes, pivot, Vector3.One) {}

        public Model(in Dictionary<string, Mesh> meshes, in Pivot pivot, Vector3 scale)
        {
            Meshes = meshes.ToDictionary(entry => entry.Key,
                    entry => (Mesh)entry.Value.Clone());
            Pivot = (Pivot)pivot.Clone();
            Scale = scale;
        }
        
        public void ResetScale() => _scale = Vector3.One;

        public Vector3 Scale
        {
            get => _scale;
            set
            {
                if (value.X > 0.0f && value.Y > 0.0f && value.Z > 0.0f)
                {
                    _scale = value;
                }
                else
                {
                    Logger.Warn("Wrong value for Scale parameter. Expected: value"
                            + $" greater than 0 for X, Y and Z components. Got: {value}. Scale was not changed.");
                }
            }
        }

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