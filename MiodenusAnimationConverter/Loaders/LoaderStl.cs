using System;
using System.IO;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Loaders
{
    public class LoaderStl : IModelLoader
    {
        private const byte HeaderSizeInBytes = 80;
        private const byte TriangleRecordSizeInBytes = 50;
        private const byte TriangleRecordsStartPositionInBytes = 84;
        
        /* TODO: добавить поддержку ASCII STL. */
        public Model Load(in string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"File {filename} not found.");
            }

            byte[] modelData = File.ReadAllBytes(filename);

            if (modelData.Length <= 0)
            {
                throw new Exception($"File {filename} is empty.");    // TODO: Сделать свое исключение.
            }
            
            /* TODO: проверить, что файл корректного типа и содержимого. */

            int trianglesAmount = BitConverter.ToInt32(modelData, HeaderSizeInBytes);
            var triangles = new Triangle[trianglesAmount];

            float scale = 0.05f;    // Временно.

            for (int i = 0; i < trianglesAmount; i++)
            {
                int currentRecordPosition = TriangleRecordsStartPositionInBytes + (i * TriangleRecordSizeInBytes);
                
                var normal = new Vector3(BitConverter.ToSingle(modelData, currentRecordPosition), 
                        BitConverter.ToSingle(modelData, currentRecordPosition + sizeof(float)),
                        BitConverter.ToSingle(modelData, currentRecordPosition + 2 * sizeof(float)));

                var vertexes = new Vertex[Triangle.VertexesAmount];
                
                vertexes[0] = new Vertex(
                        new Vector4(
                                BitConverter.ToSingle(modelData, currentRecordPosition + 3 * sizeof(float)) * scale,
                                BitConverter.ToSingle(modelData, currentRecordPosition + 4 * sizeof(float)) * scale,
                                BitConverter.ToSingle(modelData, currentRecordPosition + 5 * sizeof(float)) * scale,
                                1.0f),
                        Color4.Green);
                
                vertexes[1] = new Vertex(
                        new Vector4(
                                BitConverter.ToSingle(modelData, currentRecordPosition + 6 * sizeof(float)) * scale,
                                BitConverter.ToSingle(modelData, currentRecordPosition + 7 * sizeof(float)) * scale,
                                BitConverter.ToSingle(modelData, currentRecordPosition + 8 * sizeof(float)) * scale,
                                1.0f),
                        Color4.Green);
                
                vertexes[2] = new Vertex(
                        new Vector4(
                                BitConverter.ToSingle(modelData, currentRecordPosition + 9 * sizeof(float)) * scale,
                                BitConverter.ToSingle(modelData, currentRecordPosition + 10 * sizeof(float)) * scale,
                                BitConverter.ToSingle(modelData, currentRecordPosition + 11 * sizeof(float)) * scale,
                                1.0f),
                        Color4.Green);
                
                triangles[i] = new Triangle(normal, vertexes);
            }

            return new Model(triangles);
        }
    }
}