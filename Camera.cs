using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;

using System;

namespace _3d {

    class Camera {

        float speed = 5;
        float xRotation, yRotation;
        public Vector3 position = new Vector3(0,20,0),
                       lookEye = new Vector3(0,0, -1),
                       up = new Vector3(0,1,0);

        public Game game;
        Vector2 lastPosition;
        public Vector2 mousePosition2D;
        bool canRotate = false;

        Matrix4 projection;
        Tile tile;

        public Camera(Game game) {
            this.game = game;
            game.UpdateFrame += Update;

            // mouse
            game.MouseMove += MouseMove;
            game.MouseDown += MouseDown;
            game.MouseUp   += MouseUp;
        }

        void Update(FrameEventArgs e) {

            xRotation = Clamp(xRotation, -89.9f, 89.9f);
            lookEye.X = (float)Math.Cos(MathHelper.DegreesToRadians(xRotation)) * (float)Math.Cos(MathHelper.DegreesToRadians(yRotation));
            lookEye.Y = (float)Math.Sin(MathHelper.DegreesToRadians(xRotation));
            lookEye.Z = (float)Math.Cos(MathHelper.DegreesToRadians(xRotation)) * (float)Math.Sin(MathHelper.DegreesToRadians(yRotation));

            lookEye = Vector3.Normalize(lookEye);

            if (game.IsKeyDown(Keys.W)) position += lookEye * speed;
            else if (game.IsKeyDown(Keys.S)) position -= lookEye * speed;

            Vector3 right = Vector3.Normalize(Vector3.Cross(lookEye, up));

            if (game.IsKeyDown(Keys.A)) position -= right * speed;
            if (game.IsKeyDown(Keys.D)) position += right * speed;
        }

        void MouseMove(MouseMoveEventArgs e) {
            if (canRotate)
            {
                xRotation += (lastPosition.Y - e.Y) * .5f;
                yRotation -= (lastPosition.X - e.X) * .5f;
            }
            lastPosition = new Vector2(e.X, e.Y);
            
            // converts mouse position to 2d screen position
            mousePosition2D = new Vector2(((e.Position.X/game.ClientSize.X)*2-.5f)*2, 
                                         -((e.Position.Y/game.ClientSize.Y)*2-.5f)*2);
        }

        public void GiveTerrain(Tile tile) {
            this.tile = tile;
        }

        void MouseDown(MouseButtonEventArgs e) {
            if (e.Button == MouseButton.Right) canRotate = true;
        }
        void MouseUp(MouseButtonEventArgs e) {
            if (e.Button == MouseButton.Right) canRotate = false;
        }

        float Clamp(float value, float min, float max) {

            if (value > max) value = max;
            if (value < min) value = min;
            return value;
        }

        public void calculateMouse(Matrix4 projection, Tile tile) {
            this.projection = projection;
            this.tile = tile;
        }
    }
}