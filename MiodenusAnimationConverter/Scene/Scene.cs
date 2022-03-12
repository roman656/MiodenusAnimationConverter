using System.Collections.Generic;
using MiodenusAnimationConverter.Animation;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Scene.Models;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class Scene
    {
        private const int GridSize = 6;
        public readonly Grid Grid = new (xSizeInCells: GridSize * 10, zSizeInCells: GridSize * 10, cellSize: 0.1f);
        public readonly Grid MajorGrid = new (xSizeInCells: GridSize, zSizeInCells: GridSize, lineWidth: 2.0f);
        public readonly LightPointsController LightPointsController = new ();
        public readonly CamerasController CamerasController;
        public readonly List<ModelGroup> ModelGroups = new ();

        public Scene(in AnimationInfo animationInfo, in List<Model> models)
        {
            var cameras = new List<Camera> { new (new Vector3(0.0f, 0.5f, 3.0f), animationInfo.FrameWidth, animationInfo.FrameHeight) };
            var debugCameras = new List<DebugCamera> { new (new Vector3(0.0f, 1.5f, 3.0f), animationInfo.FrameWidth, animationInfo.FrameHeight) };
            debugCameras[0].LookAt(Vector3.Zero);
            
            CamerasController = new CamerasController(cameras, debugCameras);
            
            for (var i = 0; i < models.Count; i++)
            {
                var tempGroup = new ModelGroup();
                tempGroup.Models.Add(models[i]);
                ModelGroups.Add(tempGroup);
            }

            Grid.Pivot.IsVisible = false;
            MajorGrid.Pivot.YAxisSize = GridSize * MajorGrid.CellSize / 2.0f;
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
    }
}