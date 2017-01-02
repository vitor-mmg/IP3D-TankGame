using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrimeiraEntrega
{
    public class GestorCameras
    {
        CameraAula cameraAula;
        CameraSurfaceFollow cameraSurface;
        CameraTank cameraTank;
        int alturaMapa;
        VertexPositionNormalTexture[] vertices;
        Tanque tank;
        public Matrix view, projection, worldMatrix;
        
        enum ActivarCamara
        {
            fps,
            free,
            cameraTank
        };

        ActivarCamara activarcamara;

        public void Initialize(GraphicsDeviceManager graphics, VertexPositionNormalTexture[] vertices, int alturaMapa, Vector3 posicaoTank, Matrix worldTank, Matrix ViewTank, Tanque tank)
        {
            this.tank = tank;

            this.alturaMapa = alturaMapa;
            this.vertices = vertices;

            cameraAula = new CameraAula(graphics);
            cameraSurface = new CameraSurfaceFollow(graphics, vertices, alturaMapa);
            cameraTank = new CameraTank(graphics, vertices, alturaMapa, posicaoTank, worldTank, ViewTank);
        }
        
        public void UpdateInput()
        {
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.F1))
                activarcamara = ActivarCamara.fps;
            if (kb.IsKeyDown(Keys.F2))
                activarcamara = ActivarCamara.free;
            if (kb.IsKeyDown(Keys.F3))
                activarcamara = ActivarCamara.cameraTank;
        }
        
        public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            UpdateInput();
            if (activarcamara == ActivarCamara.fps)
            {
                cameraSurface.UpdateInput(gameTime, graphics);
                view = cameraSurface.view;
                projection = cameraSurface.projection;
                
                //tank.view = cameraSurfaceFollow.view;
                //tank.projection = cameraSurfaceFollow.projection;
            }
            else if (activarcamara == ActivarCamara.free)
            {
                cameraAula.UpdateInput(gameTime, graphics);
                view = cameraAula.view;
                projection = cameraAula.projection;
                worldMatrix = cameraAula.worldMatrix;
                //tank.view = camera.view;
                //tank.projection = camera.projection;
            }
            else
            {
                //cameraSurfaceFollow.updateCamera();
                //cameraTank.UpdateInput(gameTime, graphics,tank.getPosition());
                cameraTank.updateCamera(tank.getPosition(), tank.getWorldMAtrix(), tank.view, tank);
                view = cameraTank.view;
                projection = cameraTank.projection;
                worldMatrix = cameraTank.worldMatrix;
            }
        }
    }
}