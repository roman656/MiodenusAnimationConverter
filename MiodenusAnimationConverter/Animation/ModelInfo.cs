using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Animation
{
    public class ModelInfo
    {
        private static int _index;
        public string Name { get; set; }
        public string Type { get; set; }
        public string Filename { get; set; }
        public Color4 Color { get; set; }
        public bool UseCalculatedNormals { get; set; }
        public List<ActionBinding> ActionBindings { get; set; }

        public ModelInfo(in MafStructure.ModelInfo modelInfo, in List<ActionBinding> actionBindings)
        {
            Filename = string.IsNullOrEmpty(modelInfo.Filename.Trim())
                    ? DefaultAnimationParameters.ModelInfo.Filename
                    : modelInfo.Filename.Trim();
            Name = string.IsNullOrEmpty(modelInfo.Name.Trim())
                    ? Filename == DefaultAnimationParameters.ModelInfo.Filename
                            ? DefaultAnimationParameters.ModelInfo.Name + _index++
                            : Path.GetFileName(Filename)
                    : modelInfo.Name.Trim();
            Type = string.IsNullOrEmpty(modelInfo.Type.Trim())
                    ? DefaultAnimationParameters.ModelInfo.Type
                    : modelInfo.Type.Trim().ToLower();
            UseCalculatedNormals = modelInfo.UseCalculatedNormals;
            Color = CheckColor(modelInfo.Color)
                    ? new Color4(modelInfo.Color[0],
                                 modelInfo.Color[1],
                                 modelInfo.Color[2],
                                 1.0f)
                    : DefaultAnimationParameters.ModelInfo.Color;
            ActionBindings = actionBindings;
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
                    $"Model info:\n\tName: {Name}\n\tType: {Type}\n\tFilename: {Filename}\n\t"
                    + $"Use calculated normals: {UseCalculatedNormals}\n\tColor: ({Color.R}; {Color.G}; {Color.B};"
                    + $" {Color.A})\n\tAction bindings:\n");
            
            result = ActionBindings.Aggregate(result, (current, binding) => current + $"\n\t{binding}");

            return result + "\n";
        }
    }
}