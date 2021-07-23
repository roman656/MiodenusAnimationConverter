using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Cameras;

namespace MiodenusAnimationConverter.Scene
{
    public class Scene
    {
        public List<ICamera> Cameras;
        public List<ModelGroup> ModelGroups = new ();

        public Scene()
        {
            
        }
    }
}