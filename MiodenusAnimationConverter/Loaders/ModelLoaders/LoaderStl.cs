using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MiodenusAnimationConverter.Exceptions;
using MiodenusAnimationConverter.Scene.Models;
using MiodenusAnimationConverter.Scene.Models.Meshes;
using NLog;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Loaders.ModelLoaders
{
    public class LoaderStl : IModelLoader
    {
        private enum StlFormat
        {
            Ascii,
            Binary
        }
        
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const byte NormalParametersAmount = 3;
        private const byte VertexParametersAmount = 3;
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

        public Model Load(string filename, in Color4 modelColor, bool useCalculatedNormals)
        {
            Model model;
            
            Logger.Trace($"Loading model from {filename} started.");

            CheckModelFile(filename);

            var fileData = File.ReadAllBytes(filename);
            
            if (RecogniseStlFormat(fileData) == StlFormat.Ascii)
            {
                model = LoadAsciiStl(fileData, modelColor, useCalculatedNormals);
            }
            else
            {
                model = LoadBinaryStl(fileData, modelColor, useCalculatedNormals);
            }
            
            Logger.Trace($"Loading model from {filename} finished.");
            
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
        }

        private static StlFormat RecogniseStlFormat(in byte[] fileData)
        {
            Logger.Trace("Recognising STL format...");
            
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
            
            Logger.Trace("Recognised STL format: {0}", result);
            
            return result;
        }
        
        private static Model LoadAsciiStl(in byte[] fileData, Color4 modelColor, bool useCalculatedNormals)
        {
            var triangles = new List<Triangle>();
            var fileLines = System.Text.Encoding.ASCII.GetString(fileData).ToLower().Split('\n');
            var fileLinesAmount = fileLines.Length;
            var isNormalReady = false;
            var isVertexesReady = false;
            var normal = new Vector3();
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

                    if (values.Length != 2 + NormalParametersAmount)
                    {
                        throw new WrongModelFileContentException("Normal`s parameters amount is incorrect "
                                + $"in the model file. facet normal must have {NormalParametersAmount} parameters."
                                + $" Got {values.Length - 2}.");
                    }

                    try
                    {
                        normal = new Vector3(float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                                             float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat),
                                             float.Parse(values[4], CultureInfo.InvariantCulture.NumberFormat));
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

                    if (values.Length != 1 + VertexParametersAmount)
                    {
                        throw new WrongModelFileContentException("Vertex`s parameters amount is incorrect " 
                                + $"in the model file. vertex must have {VertexParametersAmount} parameters."
                                + $" Got {values.Length - 1}.");
                    }
                    
                    try
                    {
                        vertexes[currentVertexId] = new Vertex(
                                new Vector3(float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat),
                                            float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat),
                                            float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat)), 
                                normal,
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

            if (triangles.Count <= 0)
            {
                Logger.Warn("Warning: there are no triangles in the model file.");
            }
            
            return new Model(new Mesh(triangles.ToArray()));
        }
        
        private static Model LoadBinaryStl(in byte[] fileData, Color4 modelColor, bool useCalculatedNormals)
        {
            CheckBinaryStlFileContent(fileData);
            
            var trianglesAmount = BitConverter.ToInt32(fileData, HeaderSizeInBytes);
            var triangles = new Triangle[trianglesAmount];

            for (var i = 0; i < trianglesAmount; i++)
            {
                var currentRecordPosition = TriangleRecordsStartPositionInBytes + (i * TriangleRecordSizeInBytes);
                var vertexes = new Vertex[Triangle.VertexesAmount];

                for (var j = 1; j <= Triangle.VertexesAmount; j++)
                {
                    var offsetX = (VertexParametersAmount * j) * sizeof(float);
                    var offsetY = (VertexParametersAmount * j + 1) * sizeof(float);
                    var offsetZ = (VertexParametersAmount * j + 2) * sizeof(float);
                    
                    vertexes[j - 1] = new Vertex(
                            new Vector3(
                                    BitConverter.ToSingle(fileData, currentRecordPosition + offsetX),
                                    BitConverter.ToSingle(fileData, currentRecordPosition + offsetY),
                                    BitConverter.ToSingle(fileData, currentRecordPosition + offsetZ)),
                            new Vector3(),
                            modelColor);
                }
                
                if (useCalculatedNormals)
                {
                    triangles[i] = new Triangle(vertexes, Triangle.CalculateNormal(vertexes));
                }
                else
                {
                    var normal = new Vector3(
                            BitConverter.ToSingle(fileData, currentRecordPosition), 
                            BitConverter.ToSingle(fileData, currentRecordPosition + sizeof(float)),
                            BitConverter.ToSingle(fileData, currentRecordPosition + sizeof(float) * 2));
                    
                    triangles[i] = new Triangle(vertexes, normal);
                }
            }
            
            if (triangles.Length <= 0)
            {
                Logger.Warn("Warning: there are no triangles in the model file.");
            }

            return new Model(new Mesh(triangles));
        }

        private static void CheckBinaryStlFileContent(in byte[] fileData)
        {
            Logger.Trace("Checking binary STL model file content started.");
            
            if (fileData.Length < TriangleRecordsStartPositionInBytes)
            {
                throw new WrongModelFileContentException("Incorrect content of the model file.");
            }
            
            var trianglesAmount = BitConverter.ToUInt32(fileData, HeaderSizeInBytes);
            var realTrianglesAmount =
                    (fileData.Length - TriangleRecordsStartPositionInBytes) / TriangleRecordSizeInBytes;

            if (trianglesAmount != realTrianglesAmount)
            {
                throw new WrongModelFileContentException("Triangles amount specified in the model file does" 
                        + " not match to the actual contents of the file.");
            }
            
            Logger.Trace("Checking binary STL model file content finished.");
        }
    }
}