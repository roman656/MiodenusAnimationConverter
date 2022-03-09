using System.Collections.Generic;
using MiodenusAnimationConverter.Animation;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Scene.Models;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class Scene
    {
        public readonly Grid Grid = new (xSizeInCells: 60, zSizeInCells: 60, cellSize: 0.1f);
        public readonly Grid MajorGrid = new (xSizeInCells: 6, zSizeInCells: 6, cellSize: 1.0f, lineWidth: 2.0f);
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
            CamerasController.InitializeVao();
            Grid.InitializeVao();
            MajorGrid.InitializeVao();
            
            for (var i = 0; i < ModelGroups.Count; i++)
            {
                ModelGroups[i].Initialize();
            }
        }
        
        public void Delete()
        {
            CamerasController.Delete();
            Grid.DeleteVao();
            MajorGrid.DeleteVao();
            
            for (var i = 0; i < ModelGroups.Count; i++)
            {
                ModelGroups[i].Delete();
            }
        }

        public void DrawGrid(in Camera camera)
        {
            Grid.Draw(camera);
            MajorGrid.Draw(camera);
        }
    }
}