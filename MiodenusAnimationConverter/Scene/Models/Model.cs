using System.Collections.Generic;
using System.Linq;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Scene.Models.Meshes;
using MiodenusAnimationConverter.Shaders;
using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Scene.Models
{
    public class Model
    {
        private const int ColorChannelsAmount = 4;
        public readonly Dictionary<string, Mesh> Meshes = new ();
        public readonly Pivot Pivot = new ();
        public bool IsVisible = true;

        public Model(in Mesh mesh)
        {
            Meshes.Add("name", new Mesh(mesh));
            _vertexesAmount = Meshes["name"].VertexesAmount;
            _vertexesColorsBuffer = Meshes["name"].VertexColorsBuffer;
        }

        public void InitializeVao()
        {
            
        }
        
        private void UpdateVertexesColorsVbo()
        {
            if (_wasColorChanged)
            {
                UpdateVertexesColorsBuffer();
                _vao.UpdateVertexBufferObject(_colorsVboIndex, _vertexesColorsBuffer);
                _wasColorChanged = false;
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
                for (var i = 0; i < Meshes.Count; i++)
                {
                    Meshes.Values.ElementAt(i).Draw(shaderProgram, camera, mode);
                }
            }
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            Meshes.Scale(scaleX, scaleY, scaleZ);
        }
    }
}