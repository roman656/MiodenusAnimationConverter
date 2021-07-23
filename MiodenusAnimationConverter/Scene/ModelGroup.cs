using System.Collections.Generic;
using MiodenusAnimationConverter.Scene.Models;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Scene
{
    public class ModelGroup : IMovable, IRotatable, IScalable
    {
        public List<Model> Models = new ();

        public ModelGroup(Model model)
        {
            Models.Add(model);
        }
        
        public void Move(float deltaX, float deltaY, float deltaZ)
        {
            foreach (var model in Models)
            {
                model.Move(deltaX, deltaY, deltaZ);
            }
        }

        public void Rotate(float angle, Vector3 vector)
        {
            foreach (var model in Models)
            {
                model.Rotate(angle, vector);
            }
        }

        public void Scale(float scaleX, float scaleY, float scaleZ)
        {
            foreach (var model in Models)
            {
                model.Scale(scaleX, scaleY, scaleZ);
            }
        }
    }
}