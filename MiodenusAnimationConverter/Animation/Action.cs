namespace MiodenusAnimationConverter.Animation
{
    public class Action
    {
        public string Name;
        public ActionState[] States;

        public Action(ActionState[] states, string name = "UnnamedAction")
        {
            Name = name;
            States = states;
        }
    }
}