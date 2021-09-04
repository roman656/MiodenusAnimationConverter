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

                for (var i = 0; i < ModelGroups.Count; i++)
                {
                    var modelGroup = ModelGroups[i];

                    for (var j = 0; j < modelGroup.Models.Count; j++)
                    {
                        var model = modelGroup.Models[j];

                        for (var k = 0; k < model.Mesh.Triangles.Length; k++)
                        {
                            var triangle = model.Mesh.Triangles[k];

                            for (var l = 0; l < triangle.Vertexes.Length; l++)
                            {
                                var vertex = triangle.Vertexes[l];
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