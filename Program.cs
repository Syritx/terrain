using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace _3d
{
    class Program
    {
        static void Main(string[] args)
        {
            NativeWindowSettings nws = new NativeWindowSettings() {
                Title = "Hello",
                Size = new Vector2i(1000,720),
                StartFocused = true,
                StartVisible = true,
                APIVersion = new Version(3,2),
                Flags = ContextFlags.ForwardCompatible,
                Profile = ContextProfile.Core,
            };
            GameWindowSettings gws = new GameWindowSettings();
            new Game(gws,nws);
        }
    }

    class Game : GameWindow {

        Tile tile;
        Camera camera;

        List<float> vertices = new List<float>();
        List<uint> indices = new List<uint>();
        int mapResolution = 400;
        float[] colors = {1,1,1};
        double seed = new Random().Next(1,100000000);

        int id = 0;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings,nativeWindowSettings) {
            
            for (int x = -(mapResolution/2); x < mapResolution/2; x++) {
                for (int y = -(mapResolution/2); y < mapResolution/2; y++) {

                    vertices.Add(-.5f+x);
                    vertices.Add( .5f+CreateNewNoiseLayer(15, 2, .5f, (-.5f+x)/mapResolution, (.5f+y)/mapResolution, (float)seed));
                    vertices.Add( .5f+y);

                    vertices.Add(colors[0]);
                    vertices.Add(colors[1]);
                    vertices.Add(colors[2]);

                    vertices.Add( .5f+x);
                    vertices.Add( .5f+CreateNewNoiseLayer(15, 2, .5f, (.5f+x)/mapResolution, (.5f+y)/mapResolution, (float)seed));
                    vertices.Add( .5f+y);

                    vertices.Add(colors[0]);
                    vertices.Add(colors[1]);
                    vertices.Add(colors[2]);

                    vertices.Add( .5f+x);
                    vertices.Add( .5f+CreateNewNoiseLayer(15, 2, .5f, (.5f+x)/mapResolution, (-.5f+y)/mapResolution, (float)seed));
                    vertices.Add(-.5f+y);

                    vertices.Add(colors[0]);
                    vertices.Add(colors[1]);
                    vertices.Add(colors[2]);

                    vertices.Add(-.5f+x);
                    vertices.Add( .5f+CreateNewNoiseLayer(15, 2, .5f, (-.5f+x)/mapResolution, (-.5f+y)/mapResolution, (float)seed));
                    vertices.Add(-.5f+y);

                    vertices.Add(colors[0]);
                    vertices.Add(colors[1]);
                    vertices.Add(colors[2]);

                    indices.Add((uint)id*4);
                    indices.Add((uint)id*4+1);
                    indices.Add((uint)id*4+2);
                    indices.Add((uint)id*4);
                    indices.Add((uint)id*4+2);
                    indices.Add((uint)id*4+3);
                    id++;
                }
            }

            Run();
        }

        float CreateNewNoiseLayer(int octaves, float lac, float per, float x, float y, float seed) {

            float frequency = 2,
                  amplitude = 49;

            float noise = 0;

            for (int i = 0; i < octaves; i++) {
                noise += (float)ImprovedNoise.noise(x*frequency, y*frequency, seed)*amplitude;

                frequency *= lac;
                amplitude *= per;
            }
            return noise;
        }


        protected override void OnRenderFrame(FrameEventArgs args) {
            base.OnRenderFrame(args);
            tile.Render();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
        }

        protected override void OnLoad() {
            camera = new Camera(this);
            tile = new Tile(camera, vertices, indices);

            base.OnLoad();
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0,0,0,1.0f);
        }
    }
}
