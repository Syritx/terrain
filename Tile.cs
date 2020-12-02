using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using System.Collections.Generic;

namespace _3d {
    class Tile {
        
        Shader shader;
        static float size = 100.5f;

        static float[] ys = {
            new System.Random().Next(-1,1),
            new System.Random().Next(-1,1),
            new System.Random().Next(-1,1),
            new System.Random().Next(-1,1),
        };

        float[] vertices = {
//          X           Y   Z           R  G  B
           -1.5f*size, ys[0],  1.5f*size,  1, 0, 0,
            1.5f*size, ys[1],  1.5f*size,  1, 1, 0,
            1.5f*size, ys[2], -1.5f*size,  1, 1, 1,
           -1.5f*size, ys[3], -1.5f*size,  1, 0, 1,
        };
        uint[] indices = {
            0, 1, 2,
            0, 2, 3
        };
        int ibo, vbo, vao;
        Camera camera;

        public Tile(Camera camera, List<float> verts, List<uint> inds) {
            
            vertices = FloatListToFloatArray(verts);
            indices = UnsignedIntListToUintArray(inds);
            this.camera = camera;

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
            projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), 1000/720, 0.01f, 2000f);

            GL.UniformMatrix4(worldUniformLocation, false, ref worldMatrix);
            GL.UniformMatrix4(viewUniformLocation, false, ref viewMatrix);
            GL.UniformMatrix4(ProjectionUniformLocation, false, ref projMatrix);
        }

        public void Render() {

            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.0f,0.0f,0.0f,1.0f);
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
                projMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), 1000/720, 0.01f, 2000f);

                GL.UniformMatrix4(worldUniformLocation, false, ref worldMatrix);
                GL.UniformMatrix4(viewUniformLocation, false, ref viewMatrix);
                GL.UniformMatrix4(ProjectionUniformLocation, false, ref projMatrix);
                GL.BindVertexArray(vao);
                GL.DrawElements(PrimitiveType.Lines, indices.Length, DrawElementsType.UnsignedInt, 0);
            }
            catch(System.Exception e) {System.Console.WriteLine(e.Message);}
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