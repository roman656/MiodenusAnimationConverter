using System;
using System.Collections.Generic;

namespace MiodenusAnimationConverter.AnimationFile
{
    public class Action
    {
        public int ID {get;set;} = 0;
        public float StartTime {get;set;} = 0;
        public float TimeLength {get;set;} = 0;
        public bool UseInterpolation {get;set;} = false;

        public List<ActionValue> Values = new List<ActionValue>();
    }
}