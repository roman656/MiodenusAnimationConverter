using System;
using System.IO;
using MiodenusAnimationConverter.Exceptions;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Loaders
{
    public class LoaderStl : IModelLoader
    {
        private enum StlFormat
        {
            Ascii,
            Binary
        }

        private const string FileExtension = "stl";
        private const byte HeaderSizeInBytes = 80;
        private const byte TriangleRecordSizeInBytes = 50;
        private const byte TriangleRecordsStartPositionInBytes = 84;
        private static readonly string[] StlAsciiKeywords = { "solid ",
                                                              "facet normal",
                                                              "outer loop",
                                                              "vertex",
                                                              "endloop",
                                                              "endfacet",
                                                              "endsolid " };
        
        public Model Load(in string filename)
        {
            Model model;
            
            CheckModelFile(filename);

            var fileData = File.ReadAllBytes(filename);
            
            if (RecogniseStlFormat(fileData) == StlFormat.Ascii)
            {
                model = LoadAsciiStl(fileData);
            }
            else
            {
                model = LoadBinaryStl(fileData);
            }

            return model;
        }

        private static void CheckModelFile(in string filename)
        {
            var modelFileInfo = new FileInfo(filename);
            
            if (!modelFileInfo.Exists)
            {
                throw new FileNotFoundException($"File {filename} not found.");
            }

            if (modelFileInfo.Length <= 0)
            {
                throw new EmptyFileException($"File {filename} is empty.");
            }
            
            if (modelFileInfo.Extension.Equals(FileExtension))
            {
                throw new InvalidExtensionException($"Extension of {filename} file must be {FileExtension}.");
            }
        }

        private static StlFormat RecogniseStlFormat(in byte[] fileData)
        {
            StlFormat result;
            
            if (fileData.ToString().StartsWith(StlAsciiKeywords[0]))
            {
                result = StlFormat.Ascii;
            }
            else
            {
                result = StlFormat.Binary;
            }

            return result;
        }
        
        private static Model LoadAsciiStl(in byte[] fileData)
        {
            // TODO
            return new Model(null);
        }
        
        private static Model LoadBinaryStl(in byte[] fileData)
        {
            int trianglesAmount = BitConverter.ToInt32(fileData, HeaderSizeInBytes);
            var triangles = new Triangle[trianglesAmount];

            float scale = 0.05f;    // Временно.

            for (int i = 0; i < trianglesAmount; i++)
            {
                int currentRecordPosition = TriangleRecordsStartPositionInBytes + (i * TriangleRecordSizeInBytes);
                
                var normal = new Vector3(BitConverter.ToSingle(fileData, currentRecordPosition), 
                        BitConverter.ToSingle(fileData, currentRecordPosition + sizeof(float)),
                        BitConverter.ToSingle(fileData, currentRecordPosition + 2 * sizeof(float)));

                var vertexes = new Vertex[Triangle.VertexesAmount];
                
                vertexes[0] = new Vertex(
                        new Vector4(
                                BitConverter.ToSingle(fileData, currentRecordPosition + 3 * sizeof(float)) * scale,
                                BitConverter.ToSingle(fileData, currentRecordPosition + 4 * sizeof(float)) * scale,
                                BitConverter.ToSingle(fileData, currentRecordPosition + 5 * sizeof(float)) * scale,
                                1.0f),
                        Color4.Green);
                
                vertexes[1] = new Vertex(
                        new Vector4(
                                BitConverter.ToSingle(fileData, currentRecordPosition + 6 * sizeof(float)) * scale,
                                BitConverter.ToSingle(fileData, currentRecordPosition + 7 * sizeof(float)) * scale,
                                BitConverter.ToSingle(fileData, currentRecordPosition + 8 * sizeof(float)) * scale,
                                1.0f),
                        Color4.Green);
                
                vertexes[2] = new Vertex(
                        new Vector4(
                                BitConverter.ToSingle(fileData, currentRecordPosition + 9 * sizeof(float)) * scale,
                                BitConverter.ToSingle(fileData, currentRecordPosition + 10 * sizeof(float)) * scale,
                                BitConverter.ToSingle(fileData, currentRecordPosition + 11 * sizeof(float)) * scale,
                                1.0f),
                        Color4.Green);
                
                triangles[i] = new Triangle(normal, vertexes);
            }

            return new Model(triangles);
        }
    }
}