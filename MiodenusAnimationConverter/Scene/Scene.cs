using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Cameras;
using MiodenusAnimationConverter.Scene.Models;

namespace MiodenusAnimationConverter.Scene
{
    public class Scene : IMiodenusObject
    {
        public List<ICamera> Cameras;
        public List<IModel> Models;

        public Scene()
        {
            
        }
    }
}