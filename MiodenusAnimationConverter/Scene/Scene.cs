using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Scene.Models.Meshes;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class Scene
    {
        public readonly LightPointsController LightPointsController = new ();
        public CamerasController CamerasController;
        public List<ModelGroup> ModelGroups = new ();

        public Scene(int windowWidth, int windowHeight)
        {
            var cameras = new List<Camera> { new (new Vector3(0.0f, 0.5f, 3.0f), windowWidth, windowHeight) };
            var debugCameras = new List<DebugCamera> { new (new Vector3(0.0f, 0.5f, 3.0f), windowWidth, windowHeight) };

            CamerasController = new CamerasController(cameras, debugCameras);
        }
        
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