using System;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter
{
    public static class ModelLoader
    {
        public static Model Load(in string filename)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(filename);
            
            Vertex[] vertexes = new Vertex[] { };
            
            if (fi.Exists)
            {
                byte[] stlbinbytes = System.IO.File.ReadAllBytes(filename);
                if (stlbinbytes.Length > 0)
                {
                    int tri_count = BitConverter.ToInt32(stlbinbytes, 80);

                    int oneRecordInBytes = 50;
                    int byteStart = 84;
                    
                    vertexes = new Vertex[tri_count * 3];

                    for (int i = 0; i < tri_count; i++)
                    {
                        int sByte = byteStart + (i * oneRecordInBytes);

                        float[,] tr = new float[3, 3];
                        
                        tr[0, 0] = BitConverter.ToSingle(stlbinbytes, sByte + 12);
                        tr[0, 1] = BitConverter.ToSingle(stlbinbytes, sByte + 16);
                        tr[0, 2] = BitConverter.ToSingle(stlbinbytes, sByte + 20);

                        tr[1, 0] = BitConverter.ToSingle(stlbinbytes, sByte + 24);
                        tr[1, 1] = BitConverter.ToSingle(stlbinbytes, sByte + 28);
                        tr[1, 2] = BitConverter.ToSingle(stlbinbytes, sByte + 32);

                        tr[2, 0] = BitConverter.ToSingle(stlbinbytes, sByte + 36);
                        tr[2, 1] = BitConverter.ToSingle(stlbinbytes, sByte + 40);
                        tr[2, 2] = BitConverter.ToSingle(stlbinbytes, sByte + 44);

                        vertexes[i] =     new Vertex(new Vector4(tr[0, 0] * 0.01f, tr[0, 1] * 0.01f, tr[0, 2] * 0.01f, 1.0f), Color4.Green);
                        vertexes[i + 1] = new Vertex(new Vector4(tr[1, 0] * 0.01f, tr[1, 1] * 0.01f, tr[1, 2] * 0.01f, 1.0f), Color4.Green);
                        vertexes[i + 2] = new Vertex(new Vector4(tr[2, 0] * 0.01f, tr[2, 1] * 0.01f, tr[2, 2] * 0.01f, 1.0f), Color4.Green);
                    }
                }
            }
            
            return new Model(vertexes);
        }
    }
}