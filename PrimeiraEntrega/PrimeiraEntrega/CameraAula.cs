using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PrimeiraEntrega
{
    class CameraAula
    {
        Vector3 posicao, direcao, target;
        float velocidade, time;
        float grausPorPixel = MathHelper.ToRadians(20) / 100;
        float yaw, pitch, strafe;
        float diferencaX, diferencaY;
        Vector3 vetorBase;
        public Matrix view, projection, worldMatrix;
      
        Matrix rotacao;
   
        MouseState posicaoRatoInicial;
        public CameraAula(GraphicsDeviceManager graphics)
        {

            velocidade = 0.5f;
            vetorBase = new Vector3(1, -0.5f, 0);
            posicao = new Vector3(50, 50, 50);
            direcao = vetorBase;
            worldMatrix = Matrix.Identity;
            float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.1f, 1000.0f);
            Mouse.SetPosition(graphics.GraphicsDevice.Viewport.Height / 2, graphics.GraphicsDevice.Viewport.Width / 2);
            posicaoRatoInicial = Mouse.GetState();
            this.frente();
            updateCamera();
        }


        public void frente()
        {

            //time = gameTime.ElapsedGameTime.Milliseconds;
            posicao = posicao + velocidade * direcao;
            target = posicao + direcao;//posicao + direcao;

        }

        public void moverTras(GameTime gameTime)
        {

            time = gameTime.ElapsedGameTime.Milliseconds;
            posicao = posicao - velocidade * direcao;
            target = posicao + direcao;//posicao + direcao;
        }




        public void rodarDireitaEsquerda(GameTime gameTime)
        {
            time = gameTime.ElapsedGameTime.Milliseconds;
            //yaw = yaw - velocidade;//(yaw - velocidade);
            yaw -= diferencaX * grausPorPixel;
        }

        public void rodarCimaBaixo(GameTime gameTime)
        {
            time = gameTime.ElapsedGameTime.Milliseconds;
            //pitch = pitch + 0.01f;
            pitch -= diferencaY * grausPorPixel;
        }


        public void strafeEsquerda(GameTime gameTime, float strafe)
        {

            time = gameTime.ElapsedGameTime.Milliseconds;
            this.strafe = strafe + velocidade * time;
            posicao = posicao - velocidade * Vector3.Cross(direcao, Vector3.Up);

            target = posicao + direcao;

        }

        public void strafeDireita(GameTime gameTime, float strafe)
        {

            time = gameTime.ElapsedGameTime.Milliseconds;
            this.strafe = strafe + velocidade * time;
            posicao = posicao + velocidade * Vector3.Cross(direcao, Vector3.Up);

            target = posicao + direcao;

        }

        public void UpdateInput(GameTime gameTime, GraphicsDeviceManager graphics)
        {
            //verificarLimites();
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.NumPad8))
            {
                this.frente();

            }
            if (kb.IsKeyDown(Keys.NumPad5))
            {
                this.moverTras(gameTime);

            }
            if (kb.IsKeyDown(Keys.NumPad4))
            {
                this.strafeEsquerda(gameTime, 0.08f);

            }
            if (kb.IsKeyDown(Keys.NumPad6))
            {
                this.strafeDireita(gameTime, 0.08f);
            }

            MouseState mouseState = Mouse.GetState();
            if (mouseState != posicaoRatoInicial)
            {
                diferencaX = mouseState.Position.X - posicaoRatoInicial.Position.X;
                diferencaY = mouseState.Position.Y - posicaoRatoInicial.Position.Y;
                this.rodarDireitaEsquerda(gameTime);
                this.rodarCimaBaixo(gameTime);
                try
                {
                    Mouse.SetPosition(graphics.GraphicsDevice.Viewport.Height / 2, graphics.GraphicsDevice.Viewport.Width / 2);
                }
                catch (Exception e)
                { }

            }
            updateCamera();
        }

        public void updateCamera()
        {

            rotacao = Matrix.CreateFromYawPitchRoll(yaw, 0, pitch);
            worldMatrix = rotacao;
            direcao = Vector3.Transform(vetorBase, rotacao);
            target = posicao + direcao;
            view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
        }
        public void verificarLimites()
        {
            Vector3 posicaoAnterior = this.posicao;
            //verificar se esta fora do terreno
            if (this.posicao.X - 1 < 0)
            {
                this.posicao.X = posicaoAnterior.X;
            }
            if (this.posicao.Z - 1 < 0)
            {
                this.posicao.Z += 0.5f;
            }
            if (this.posicao.X + 1 > 127)
            {
                this.posicao.X -= 0.5f;
            }
            if (this.posicao.Z + 1 > 127)
            {
                this.posicao.Z -= 0.5f;
            }
        }
    }
}