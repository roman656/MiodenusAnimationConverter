using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

        private const string FileExtension = ".stl";
        private const byte HeaderSizeInBytes = 80;
        private const byte TriangleRecordSizeInBytes = 50;
        private const byte TriangleRecordsStartPositionInBytes = 84;
        private static readonly string[] StlAsciiKeywords = { "solid ",
                                                              "facet normal ",
                                                              "outer loop",
                                                              "vertex ",
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

            if (!modelFileInfo.Extension.ToLower().Equals(FileExtension))
            {
                throw new InvalidExtensionException($"Extension of {filename} file must be {FileExtension}.");
            }
        }

        private static StlFormat RecogniseStlFormat(in byte[] fileData)
        {
            var result = StlFormat.Binary;
            var dataLength = fileData.Length;

            for (var i = 0; i < dataLength; i++)
            {
                /* Если встретился первый символ ключевого слова. */
                if ((fileData[i] == StlAsciiKeywords[0][0]) || (fileData[i] == (StlAsciiKeywords[0].ToUpper())[0]))
                {
                    /* Пытаемся считать все слово и сравнить с ключевым. */
                    try
                    {
                        var keywordCandidate = new ArraySegment<byte>(fileData, i, StlAsciiKeywords[0].Length);

                        if (System.Text.Encoding.ASCII.GetString(keywordCandidate).ToLower().Equals(StlAsciiKeywords[0]))
                        {
                            result = StlFormat.Ascii;
                        }
                    }
                    catch (ArgumentException)
                    {
                        /* Если не удалось - значит это не ASCII. */
                        result = StlFormat.Binary;
                    }
                }
                else if (char.IsWhiteSpace((char) fileData[i]))
                {
                    /* Все пробельные символы с начала файла пропускаются. */
                    continue;
                }
                
                break;
            }
            
            return result;
        }
        
        private static Model LoadAsciiStl(in byte[] fileData)
        {
            var triangles = new List<Triangle>();
            var lines = System.Text.Encoding.ASCII.GetString(fileData).Split('\n');
            bool isTriangleReady = false;
            var vertexes = new Vertex[Triangle.VertexesAmount];
            int currentVertex = 0;
            for (var i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Trim();
                
                if (lines[i].StartsWith(StlAsciiKeywords[4]))
                {
                    isTriangleReady = true;
                }
                else if (lines[i].StartsWith(StlAsciiKeywords[3]))
                {
                    var values = lines[i].Split(' ');
                    vertexes[currentVertex] = new Vertex(
                        new Vector4(0.05f * float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat), 0.05f * float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat), 0.05f * float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat), 1.0f),
                            Color4.Green);
                    currentVertex++;
                }

                if (isTriangleReady)
                {
                    triangles.Add(new Triangle(vertexes));
                    isTriangleReady = false;
                    currentVertex = 0;
                }
            }
            
            return new Model(triangles.ToArray());
        }
        
        private static Model LoadBinaryStl(in byte[] fileData)
        {
            var trianglesAmount = BitConverter.ToUInt32(fileData, HeaderSizeInBytes);
            var triangles = new Triangle[trianglesAmount];

            for (uint i = 0; i < trianglesAmount; i++)
            {
                var currentRecordPosition = (int)(TriangleRecordsStartPositionInBytes + (i * TriangleRecordSizeInBytes));

                var vertexes = new Vertex[Triangle.VertexesAmount];
                
                vertexes[0] = new Vertex(
                        new Vector4(
                                BitConverter.ToSingle(fileData, currentRecordPosition + 3 * sizeof(float)),
                                BitConverter.ToSingle(fileData, currentRecordPosition + 4 * sizeof(float)),
                                BitConverter.ToSingle(fileData, currentRecordPosition + 5 * sizeof(float)),
                                1.0f),
                        Color4.Green);
                
                vertexes[1] = new Vertex(
                        new Vector4(
                                BitConverter.ToSingle(fileData, currentRecordPosition + 6 * sizeof(float)),
                                BitConverter.ToSingle(fileData, currentRecordPosition + 7 * sizeof(float)),
                                BitConverter.ToSingle(fileData, currentRecordPosition + 8 * sizeof(float)),
                                1.0f),
                        Color4.Green);
                
                vertexes[2] = new Vertex(
                        new Vector4(
                                BitConverter.ToSingle(fileData, currentRecordPosition + 9 * sizeof(float)),
                                BitConverter.ToSingle(fileData, currentRecordPosition + 10 * sizeof(float)),
                                BitConverter.ToSingle(fileData, currentRecordPosition + 11 * sizeof(float)),
                                1.0f),
                        Color4.Green);
                
                triangles[i] = new Triangle(vertexes);
            }

            return new Model(triangles);
        }
    }
}