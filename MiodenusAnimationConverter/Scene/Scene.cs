using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Cameras;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class Scene
    {
        public readonly LightPointsController LightPointsController = new ();
        public readonly CamerasController CamerasController;
        public readonly List<ModelGroup> ModelGroups = new ();

        public Scene(int windowWidth, int windowHeight)
        {
            var cameras = new List<Camera> { new (new Vector3(0.0f, 0.5f, 3.0f), windowWidth, windowHeight) };
            var debugCameras = new List<DebugCamera> { new (new Vector3(0.0f, 0.5f, 3.0f), windowWidth, windowHeight) };

            CamerasController = new CamerasController(cameras, debugCameras);
        }

        public void Initialize()
        {
            CamerasController.Initialize();
            
            for (var i = 0; i < ModelGroups.Count; i++)
            {
                ModelGroups[i].Initialize();
            }
        }
    }
}