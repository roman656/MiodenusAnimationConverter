using System.Collections.Generic;

namespace MiodenusAnimationConverter.Animation.MAFStructure
{
    public class Action
    {
        public string Name { get; set; } = string.Empty;
        public List<ActionState> States { get; set; } = new ();
    }
}