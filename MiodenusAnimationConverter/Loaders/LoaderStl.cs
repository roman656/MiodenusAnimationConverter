using System;
using System.Collections.Generic;
using System.Globalization;
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

        public Model Load(in string filename, Color4 modelColor, bool useCalculatedNormals)
        {
            Model model;
            
            CheckModelFile(filename);

            var fileData = File.ReadAllBytes(filename);
            
            if (RecogniseStlFormat(fileData) == StlFormat.Ascii)
            {
                model = LoadAsciiStl(fileData, useCalculatedNormals, modelColor);
            }
            else
            {
                model = LoadBinaryStl(fileData, useCalculatedNormals, modelColor);
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
                /* Если встретился первый символ ключевого слова (до него непробельных символов не было). */
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
        
        private static Model LoadAsciiStl(in byte[] fileData, bool useCalculatedNormals, Color4 modelColor)
        {
            var triangles = new List<Triangle>();
            var fileLines = System.Text.Encoding.ASCII.GetString(fileData).ToLower().Split('\n');
            var fileLinesAmount = fileLines.Length;
            var isNormalReady = false;
            var isVertexesReady = false;
            var normal = new Vector4();
            var vertexes = new Vertex[Triangle.VertexesAmount];
            byte currentVertexId = 0;

            for (var i = 0; i < fileLinesAmount; i++)
            {
                fileLines[i] = fileLines[i].Trim();

                if (fileLines[i].StartsWith(StlAsciiKeywords[1]) && !useCalculatedNormals)    // Считывание нормали.
                {
                    if (isNormalReady)
                    {
                        throw new WrongModelFileContentException("There is not enough data in the model file." 
                                + $" {Triangle.VertexesAmount - currentVertexId} more vertexes are needed."
                                + $" Current facet normal: {normal.X} {normal.Y} {normal.Z}.");
                    }
                    
                    var values = fileLines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (values.Length != 5)
                    {
                        throw new WrongModelFileContentException("Normal`s parameters amount is incorrect "
                                + $"in the model file. facet normal must have 3 parameters. Got {values.Length - 2}.");
                    }

                    try
                    {
                        normal = new Vector4(float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                                             float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat),
                                             float.Parse(values[4], CultureInfo.InvariantCulture.NumberFormat),
                                             1.0f);
                    }
                    catch (FormatException)
                    {
                        throw new WrongModelFileContentException("In the model file: one or more parameters"
                                + " of facet normal are not of the float type.");
                    }

                    isNormalReady = true;
                }
                else if (fileLines[i].StartsWith(StlAsciiKeywords[3]))    // Считывание текущей вершины.
                {
                    if (!isNormalReady && !useCalculatedNormals)
                    {
                        throw new WrongModelFileContentException("Incorrect data in the model file."
                                + " Expected: facet normal. Got: vertex.");
                    }

                    var values = fileLines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (values.Length != 4)
                    {
                        throw new WrongModelFileContentException("Vertex`s parameters amount is incorrect " 
                                + $"in the model file. vertex must have 3 parameters. Got {values.Length - 1}.");
                    }
                    
                    try
                    {
                        vertexes[currentVertexId] = new Vertex(
                                new Vector4(float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat),
                                            float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                                            float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat),
                                            1.0f), 
                                modelColor);
                    }
                    catch (FormatException)
                    {
                        throw new WrongModelFileContentException("In the model file: one or more parameters" 
                                + " of vertex are not of the float type.");
                    }

                    currentVertexId++;

                    if (currentVertexId >= Triangle.VertexesAmount)
                    {
                        isVertexesReady = true;
                    }
                }

                if (isVertexesReady)
                {
                    if (useCalculatedNormals)
                    {
                        triangles.Add(new Triangle(vertexes, Triangle.CalculateNormal(vertexes)));
                    }
                    else
                    {
                        triangles.Add(new Triangle(vertexes, normal));
                    }
                    
                    isNormalReady = false;
                    isVertexesReady = false;
                    currentVertexId = 0;
                }
            }
            
            return new Model(triangles.ToArray());
        }
        
        private static Model LoadBinaryStl(in byte[] fileData, bool useCalculatedNormals, Color4 modelColor)
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
                
                triangles[i] = new Triangle(vertexes, Triangle.CalculateNormal(vertexes));
            }

            return new Model(triangles);
        }
    }
}