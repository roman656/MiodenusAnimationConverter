using System.Collections.Generic;
using System.Globalization;

namespace MiodenusAnimationConverter.Animation
{
    public class Action
    {
        public string Name { get; set; }
        public List<ActionState> States { get; set; }

        public Action(MAFStructure.Action action)
        {
            Name = (action.Name == string.Empty) ? "UnnamedAction" : action.Name;
            States = new List<ActionState>();

            foreach (var actionState in action.States)
            {
                States.Add(new ActionState(actionState));
            }
        }
        
        public override string ToString()
        {
            var result = $"Action:\n\tName: {Name}\n\tStates:\n\n";
            
            foreach (var state in States)
            {
                result += $"\t{state}\n";
            }
            
            return string.Format(CultureInfo.InvariantCulture, result);
        }
    }
}