using System.Collections.Generic;
using MiodenusAnimationConverter.Exceptions;
using OpenTK.Graphics.OpenGL;

namespace MiodenusAnimationConverter.Shaders
{
    public class ShaderProgram
    {
        public readonly int ProgramId;
        
        public ShaderProgram(in List<Shader> shaders) 
        {
            ProgramId = GL.CreateProgram();

            foreach (var shader in shaders)
            {
                GL.AttachShader(ProgramId, shader.ShaderId);
            }
            
            GL.LinkProgram(ProgramId);
            
            var infoLog = GL.GetProgramInfoLog(ProgramId);

            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new ProgramLinkingException($"Errors occurred during the program linking: {infoLog}.");
            }
            
            foreach (var shader in shaders)
            {
                GL.DetachShader(ProgramId, shader.ShaderId);
            }
        }

        public void Delete()
        {
            GL.DeleteProgram(ProgramId);
        }

        public void Use()
        {
            GL.UseProgram(ProgramId);
        }
    }
}