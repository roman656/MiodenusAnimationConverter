using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Scene.Models.Meshes;

namespace MiodenusAnimationConverter.Scene
{
    public class Scene
    {
        public List<Camera> Cameras = new ();
        public List<ModelGroup> ModelGroups = new ();

        public Scene() {}
        
        public Vertex[] Vertexes
        {
            get
            {
                var vertexes = new List<Vertex>();

                foreach (var modelGroup in ModelGroups)
                {
                    foreach (var model in modelGroup.Models)
                    {
                        foreach (var triangle in model.Mesh.Triangles)
                        {
                            foreach (var vertex in triangle.Vertexes)
                            {
                                vertexes.Add(vertex);
                            }
                        }
                    }
                }

                return vertexes.ToArray();
            }
        }
    }
}