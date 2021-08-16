using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter
{
    public class VertexArrayObject
    {
        public readonly int VertexArrayObjectIndex;
        private List<int> _vertexBufferObjectIndexes = new();

        public VertexArrayObject()
        {
            VertexArrayObjectIndex = GL.GenVertexArray();
        }

        public void AddVertexBufferObject(in float[] data, int elementsPerVertex,
                VertexAttribPointerType elementType = VertexAttribPointerType.Float,
                BufferUsageHint bufferUsageHint = BufferUsageHint.StaticDraw)
        {
            var vertexBufferObjectIndex = GL.GenBuffer();

            GL.BindVertexArray(VertexArrayObjectIndex);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObjectIndex);
            
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, bufferUsageHint);
            GL.EnableVertexAttribArray(_vertexBufferObjectIndexes.Count);
            GL.VertexAttribPointer(_vertexBufferObjectIndexes.Count,
                    elementsPerVertex,
                    elementType,
                    false,
                    0,
                    0);
            _vertexBufferObjectIndexes.Add(vertexBufferObjectIndex);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer,0);
            GL.BindVertexArray(0);
        }

        public void Draw(int vertexesAmount, PrimitiveType mode = PrimitiveType.Triangles)
        {
            GL.BindVertexArray(VertexArrayObjectIndex);
            GL.DrawArrays(mode, 0, vertexesAmount);
            GL.BindVertexArray(0);
        }

        private void DeleteAllVertexBufferObjects()
        {
            if (_vertexBufferObjectIndexes.Count > 0)
            {
                GL.DeleteBuffers(_vertexBufferObjectIndexes.Count, _vertexBufferObjectIndexes.ToArray());
            }
        }

        public void Delete()
        {
            DeleteAllVertexBufferObjects();
            GL.DeleteVertexArray(VertexArrayObjectIndex);
        }
    }
}