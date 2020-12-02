using OpenTK.Graphics.OpenGL;
using System.IO;
using System.Text;

namespace _3d {
    class Shader {

        public int program, 
                   vertexShader, 
                   fragmentShader;

        public Shader(string vertexFilePath, string fragmentFilePath) {
            string vertexSource = LoadShaderSource(vertexFilePath);
            string fragmentSource = LoadShaderSource(fragmentFilePath);

            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShader, vertexSource);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(vertexShader);
            GL.CompileShader(fragmentShader);

            program = GL.CreateProgram();
            GL.AttachShader(program,vertexShader);
            GL.AttachShader(program,fragmentShader);
            GL.LinkProgram(program);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void UseShader() {
            GL.UseProgram(program);
        }

        public static string LoadShaderSource(string filePath) {
            string source = null;

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8)) {
                source = reader.ReadToEnd();
            }
            return source;
        }
    }
}