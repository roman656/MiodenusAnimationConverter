namespace MiodenusAnimationConverter.Animation.MAFStructure
{
    public class ModelInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public float[] Color { get; set; } = { -1.0f, -1.0f, -1.0f };
        public Transformation BaseTransformation { get; set; } = new();
    }
}