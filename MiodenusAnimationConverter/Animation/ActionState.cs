using System.Globalization;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Animation
{
    public class ActionState
    {
        public int Time { get; set; }
        public bool IsModelVisible { get; set; }
        public Color4 Color { get; set; }
        public Transformation Transformation { get; set; }
        public bool WasColorChanged { get; set; }

        public ActionState(in MafStructure.ActionState actionState)
        {
            Time = actionState.Time < 0
                    ? DefaultAnimationParameters.ActionState.Time
                    : actionState.Time;
            IsModelVisible = actionState.IsModelVisible;
            
            if (CheckColor(actionState.Color))
            {
                Color = new Color4(actionState.Color[0], actionState.Color[1], actionState.Color[2], 1.0f);
                WasColorChanged = true;
            }
            else
            {
                Color = DefaultAnimationParameters.ActionState.Color;
            }

            Transformation = new Transformation(actionState.Transformation);
        }

        private static bool CheckColor(in float[] color)
        {
            var result = true;

            for (var i = 0; i < 3; i++)
            {
                if (color[i] < 0.0f || color[i] > 1.0f)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public override string ToString()
        {
            var result = string.Format(CultureInfo.InvariantCulture, 
                    $"Action state:\n\tTime: {Time}\n\tIs model visible: {IsModelVisible}\n\t");
            
            if (WasColorChanged)
            {
                result += $"Color: ({Color.R}; {Color.G}; {Color.B}; {Color.A})\n\t";
            }
            else
            {
                result += "Color was not changed.\n\t";
            }

            return result + $"{Transformation}";
        }
    }
}