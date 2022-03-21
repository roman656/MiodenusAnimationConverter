namespace MiodenusAnimationConverter.Animation.MafStructure
{
    public class ModelInfo
    {
        public string Name { get; set; } = DefaultMafParameters.ModelInfo.Name;
        public string Type { get; set; } = DefaultMafParameters.ModelInfo.Type;
        public string Filename { get; set; } = DefaultMafParameters.ModelInfo.Filename;
        public float[] Color { get; set; } = DefaultMafParameters.ModelInfo.Color;
        public bool UseCalculatedNormals { get; set; } = DefaultMafParameters.ModelInfo.UseCalculatedNormals;
    }
}