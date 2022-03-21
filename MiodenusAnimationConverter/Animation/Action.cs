using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MiodenusAnimationConverter.Animation
{
    public class Action
    {
        private static int _index;
        public string Name { get; set; }
        public List<ActionState> States { get; set; }

        public Action(in MafStructure.Action action)
        {
            Name = string.IsNullOrEmpty(action.Name.Trim())
                    ? DefaultAnimationParameters.Action.Name + _index++
                    : action.Name.Trim();
            States = new List<ActionState>();

            foreach (var actionState in action.States)
            {
                States.Add(new ActionState(actionState));
            }
        }
        
        public override string ToString()
        {
            var result = string.Format(CultureInfo.InvariantCulture, $"Action:\n\tName: {Name}\n\tStates:\n");

            result = States.Aggregate(result, (current, state) => current + $"\n\t{state}");

            return result + "\n";
        }
    }
}