using System;
using System.Collections.Generic;

namespace MiodenusAnimationConverter.AnimationFile
{
    public class ActionValue
    {
        public float Time {get;set;} = 0;
        public Transform Transform { get; set; } = new Transform();
    }
}