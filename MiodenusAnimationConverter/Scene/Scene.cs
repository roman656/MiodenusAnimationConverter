using System.Collections.Generic;
using MiodenusAnimationConverter.Animation;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Scene.Models;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class Scene
    {
        public readonly LightPointsController LightPointsController = new ();
        public readonly CamerasController CamerasController;
        public readonly List<ModelGroup> ModelGroups = new ();

        public Scene(in AnimationInfo animationInfo, in List<Model> models)
        {
            var cameras = new List<Camera> { new (new Vector3(0.0f, 0.5f, 3.0f), animationInfo.FrameWidth, animationInfo.FrameHeight) };
            var debugCameras = new List<DebugCamera> { new (new Vector3(0.0f, 0.5f, 3.0f), animationInfo.FrameWidth, animationInfo.FrameHeight) };

            CamerasController = new CamerasController(cameras, debugCameras);
            
            for (var i = 0; i < models.Count; i++)
            {
                var tempGroup = new ModelGroup();
                tempGroup.Models.Add(models[i]);
                ModelGroups.Add(tempGroup);
            }
        }

        public void Initialize()
        {
            CamerasController.Initialize();
            
            for (var i = 0; i < ModelGroups.Count; i++)
            {
                ModelGroups[i].Initialize();
            }
        }
        
        public void Delete()
        {
            CamerasController.Delete();
            
            for (var i = 0; i < ModelGroups.Count; i++)
            {
                ModelGroups[i].Delete();
            }
        }
    }
}