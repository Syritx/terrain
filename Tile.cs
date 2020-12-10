using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Collections.Generic;

namespace _3d {
    class Tile {
        
        Shader shader;
        float[] vertices;
        uint[] indices;
        int ibo, vbo, vao;        
        Camera camera;

        public Tile(Camera camera, List<float> verts, List<uint> inds) {

            this.camera = camera;
            vertices = FloatListToFloatArray(verts);
            indices = UnsignedIntListToUintArray(inds);

            shader = new Shader("Shaders/vertex.glsl", "Shaders/fragment.glsl");
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ibo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);

            int positionAttribLocation = GL.GetAttribLocation(shader.program, "vertexPosition");
            int colorAttribLocation = GL.GetAttribLocation(shader.program, "vertexColor");

            GL.VertexAttribPointer(
                positionAttribLocation,
                3,
                VertexAttribPointerType.Float,
                false,
                6 * sizeof(float), 0
            );

            GL.VertexAttribPointer(
                colorAttribLocation,
                3,
                VertexAttribPointerType.Float,
                false,
                6 * sizeof(float),
                3 * sizeof(float)
            );
            GL.EnableVertexAttribArray(positionAttribLocation);
            GL.EnableVertexAttribArray(colorAttribLocation);
            shader.UseShader();

            int worldUniformLocation = GL.GetUniformLocation(shader.program, "mWorld"),
                viewUniformLocation = GL.GetUniformLocation(shader.program, "mView"),
                ProjectionUniformLocation = GL.GetUniformLocation(shader.program, "mProj");
            
            Matrix4 worldMatrix = new Matrix4(),
                    viewMatrix =  new Matrix4(),
                    projMatrix =  new Matrix4();

            worldMatrix = Matrix4.Identity;
            viewMatrix = Matrix4.LookAt(camera.position, camera.position+camera.lookEye, camera.up);
            projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(80), 1000/720, 0.01f, 2000f);

            GL.UniformMatrix4(worldUniformLocation, false, ref worldMatrix);
            GL.UniformMatrix4(viewUniformLocation, false, ref viewMatrix);
            GL.UniformMatrix4(ProjectionUniformLocation, false, ref projMatrix);

            camera.calculateMouse(projMatrix, this);
        }

        public void Render() {

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.05f,0.05f,0.05f,1.0f);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            
            try {
                shader.UseShader();

                int worldUniformLocation = GL.GetUniformLocation(shader.program, "mWorld"),
                viewUniformLocation = GL.GetUniformLocation(shader.program, "mView"),
                ProjectionUniformLocation = GL.GetUniformLocation(shader.program, "mProj");
            
                Matrix4 worldMatrix = new Matrix4(),
                        viewMatrix =  new Matrix4(),
                        projMatrix =  new Matrix4();

                worldMatrix = Matrix4.Identity;
                viewMatrix = Matrix4.LookAt(camera.position, camera.position+camera.lookEye, camera.up);
                projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(95), 1000/720, 0.01f, 2000f);

                GL.UniformMatrix4(worldUniformLocation, false, ref worldMatrix);
                GL.UniformMatrix4(viewUniformLocation, false, ref viewMatrix);
                GL.UniformMatrix4(ProjectionUniformLocation, false, ref projMatrix);
                GL.BindVertexArray(vao);
                GL.DrawElements(PrimitiveType.Lines, indices.Length, DrawElementsType.UnsignedInt, 0);
            }
            catch(System.Exception e) {System.Console.WriteLine(e.Message);}
        }

        public void a(List<float> vertices, List<uint> indices) {
            this.vertices = FloatListToFloatArray(vertices);
            this.indices = UnsignedIntListToUintArray(indices);

            GL.BufferData(BufferTarget.ArrayBuffer, this.vertices.Length * sizeof(float), this.vertices, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer, this.indices.Length * sizeof(uint), this.indices, BufferUsageHint.StaticDraw);
        }

        float[] FloatListToFloatArray(List<float> list) {
            float[] floatArray = new float[list.Count];

            for (int i = 0; i < floatArray.Length; i++) {
                floatArray[i] = list[i];
            }
            return floatArray;
        }

        uint[] UnsignedIntListToUintArray(List<uint> list) {
            uint[] uintArray = new uint[list.Count];

            for (int i = 0; i < uintArray.Length; i++) {
                uintArray[i] = list[i];
            }
            return uintArray;
        }
    }
}