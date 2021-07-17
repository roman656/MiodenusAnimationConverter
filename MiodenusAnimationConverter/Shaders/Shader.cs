using MiodenusAnimationConverter.Exceptions;
using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders
{
    public class Shader
    {
        public readonly ShaderType Type;
        public readonly int ShaderId;

        public Shader(in string shaderCode, ShaderType type)
        {
            Type = type;
            ShaderId = GL.CreateShader(Type);
            
            GL.ShaderSource(ShaderId, shaderCode);
            GL.CompileShader(ShaderId);
            
            var infoLog = GL.GetShaderInfoLog(ShaderId);
            
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new ShaderCompileException(
                        $"Errors occurred during the compilation of the {Type} shader: {infoLog}.");
            }
        }

        public void Delete()
        {
            GL.DeleteShader(ShaderId);
        }
    }
}