using System.Collections.Generic;
using MiodenusAnimationConverter.Exceptions;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace MiodenusAnimationConverter.Shaders
{
    public class ShaderProgram
    {
        public readonly int ProgramId;
        private readonly Dictionary<string, int> _uniformsLocations = new ();
        
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

            GetUniformsLocations();
        }

        private void GetUniformsLocations()
        {
            GL.GetProgram(ProgramId, GetProgramParameterName.ActiveUniforms, out var uniformsAmount);
            
            for (var i = 0; i < uniformsAmount; i++)
            {
                var key = GL.GetActiveUniform(ProgramId, i, out _, out _);
                var location = GL.GetUniformLocation(ProgramId, key);
                
                _uniformsLocations.Add(key, location);
            }
        }
        
        public int GetAttributeLocation(in string attributeName)
        {
            return GL.GetAttribLocation(ProgramId, attributeName);
        }

        public void Delete()
        {
            GL.DeleteProgram(ProgramId);
        }

        public void Use()
        {
            GL.UseProgram(ProgramId);
        }
        
        public void SetInt(string name, int data)
        {
            GL.UseProgram(ProgramId);
            GL.Uniform1(_uniformsLocations[name], data);
        }
        
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(ProgramId);
            GL.Uniform1(_uniformsLocations[name], data);
        }
        
        public void SetMatrix4(string name, Matrix4 data, bool transpose = true)
        {
            GL.UseProgram(ProgramId);
            GL.UniformMatrix4(_uniformsLocations[name], transpose, ref data);
        }
        
        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(ProgramId);
            GL.Uniform3(_uniformsLocations[name], data);
        }
        
        public void SetVector4(string name, Vector4 data)
        {
            GL.UseProgram(ProgramId);
            GL.Uniform4(_uniformsLocations[name], data);
        }
    }
}