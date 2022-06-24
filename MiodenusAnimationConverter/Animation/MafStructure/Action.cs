namespace MiodenusAnimationConverter.Animation.MafStructure;

using System.Collections.Generic;

public class Action
{
    public string Name { get; set; } = DefaultMafParameters.Action.Name;
    public List<ActionState> States { get; set; } = new ();
}