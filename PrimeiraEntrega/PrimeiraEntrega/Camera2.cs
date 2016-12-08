using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrimeiraEntrega
{
    class CameraVersao2
    {

        Vector3 posicao, direcao, target;
        float velocidade, time;
        float grausPorPixel = 20 / 100;
        float yaw, pitch, roll, strafe;
        Vector3 vetorBase;
        public Matrix view, projection, worldMatrix;
        Vector2 posicaoRato;
        Matrix rotacao;
        Vector3 vUpaux;
        Vector3 vUp;

        public CameraVersao2()
        {
            //vetorBase = new Vector3(1, 0, 0);
            //time = gameTime.ElapsedGameTime.Milliseconds;
            velocidade = 0.01f;
            vetorBase = new Vector3(0, 0, -1);
            posicao = new Vector3(20, 20, 100);
            direcao = vetorBase;
            worldMatrix = Matrix.Identity;

        }


        public void input(GameTime gameTime, GraphicsDeviceManager device)
        {
            KeyboardState kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.W))
            {
                this.frente(gameTime);
                view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
            }
            if (kb.IsKeyDown(Keys.S))
            {
                this.moverTras(gameTime);
                view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
            }
            if (kb.IsKeyDown(Keys.Q))
            {
                this.strafeEsquerda(gameTime, 0.08f);
                view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
            }
            if (kb.IsKeyDown(Keys.E))
            {
                this.strafeDireita(gameTime, 0.08f);
                view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
            }
            //teste
            if (kb.IsKeyDown(Keys.Right))
            {

                time = gameTime.ElapsedGameTime.Milliseconds;
                //yaw = yaw -velocidade * time / 100;
                yaw -= 1f;
                //rotacao = Matrix.CreateFromYawPitchRoll(yaw, 0, 0);
                rotacao = Matrix.CreateRotationY(MathHelper.ToRadians(yaw));
                Console.WriteLine(-yaw);
                worldMatrix = rotacao;
                direcao = Vector3.Transform(vetorBase, rotacao);

                target = posicao + direcao;
                view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
            }
            if (kb.IsKeyDown(Keys.Left))
            {

                time = gameTime.ElapsedGameTime.Milliseconds;
                //yaw = yaw +velocidade * time / 100;
                yaw += 1f;
                //rotacao = Matrix.CreateFromYawPitchRoll(yaw, 0, 0);
                rotacao = Matrix.CreateRotationY(MathHelper.ToRadians(yaw));

                Console.WriteLine(yaw);
                worldMatrix = rotacao;
                direcao = Vector3.Transform(vetorBase, rotacao);

                target = posicao + direcao;
                view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
            }
            if (kb.IsKeyDown(Keys.Up))
            {

                time = gameTime.ElapsedGameTime.Milliseconds;
                //yaw = yaw -velocidade * time / 100;
                pitch += 1f;
                //rotacao = Matrix.CreateFromYawPitchRoll(yaw, 0, 0);
                rotacao = Matrix.CreateRotationX(MathHelper.ToRadians(pitch));
                Console.WriteLine(yaw);
                worldMatrix = rotacao;
                direcao = Vector3.Transform(vetorBase, rotacao);

                target = posicao + direcao;
                view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
            }
            if (kb.IsKeyDown(Keys.Down))
            {

                time = gameTime.ElapsedGameTime.Milliseconds;
                //yaw = yaw +velocidade * time / 100;
                pitch -= 1f;
                //rotacao = Matrix.CreateFromYawPitchRoll(yaw, 0, 0);
                rotacao = Matrix.CreateRotationX(MathHelper.ToRadians(pitch));

                Console.WriteLine(yaw);
                worldMatrix = rotacao;
                direcao = Vector3.Transform(vetorBase, rotacao);

                target = posicao + direcao;
                view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
            }

            //rato
            MouseState mouseState = Mouse.GetState();
            //rotacao em x
            if (mouseState.X < posicaoRato.X)
            {

                time = gameTime.ElapsedGameTime.Milliseconds;
                //yaw = yaw -velocidade * time / 100;
                yaw += 0.01f;
                //rotacao = Matrix.CreateFromYawPitchRoll(yaw, 0, 0);
                rotacao = Matrix.CreateRotationY(yaw);
                Console.WriteLine(yaw);
                //worldMatrix = rotacao;
                vetorBase = Vector3.Transform(vetorBase, rotacao);

                target = posicao + vetorBase;
                view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
            }
            if (mouseState.X > posicaoRato.X)
            {

                time = gameTime.ElapsedGameTime.Milliseconds;
                //yaw = yaw +velocidade * time / 100;
                yaw -= 0.01f;
                //rotacao = Matrix.CreateFromYawPitchRoll(yaw, 0, 0);
                rotacao = Matrix.CreateRotationY(yaw);
                Console.WriteLine(-yaw);
                //worldMatrix = rotacao;
                direcao = Vector3.Transform(vetorBase, rotacao);

                target = posicao + direcao;
                view = Matrix.CreateLookAt(posicao, target, Vector3.Up);

            }
            //rotacao em y
            if (mouseState.Y > posicaoRato.Y)
            {

                view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
            }
            if (mouseState.Y < posicaoRato.Y)
            {

                view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
            }
            try
            {
                Mouse.SetPosition(device.GraphicsDevice.Viewport.Height / 2, device.GraphicsDevice.Viewport.Width / 2);
            }
            catch (Exception e) { }
            posicaoRato.X = mouseState.X;
            posicaoRato.Y = mouseState.Y;

        }


        public void strafeEsquerda(GameTime gameTime, float strafe)
        {
            time = gameTime.ElapsedGameTime.Milliseconds;
            this.strafe = strafe + velocidade * time;
            posicao = posicao - velocidade * Vector3.Cross(vetorBase, Vector3.Up);

            target = posicao + vetorBase;
            //view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
        }

        public void strafeDireita(GameTime gameTime, float strafe)
        {
            time = gameTime.ElapsedGameTime.Milliseconds;
            this.strafe = strafe + velocidade * time;
            posicao = posicao + velocidade * Vector3.Cross(vetorBase, Vector3.Up);

            target = posicao + vetorBase;
            //view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
        }

        public void frente(GameTime gameTime)
        {
            time = gameTime.ElapsedGameTime.Milliseconds;
            posicao = posicao + velocidade * vetorBase * time;
            target = posicao + vetorBase;//posicao + direcao;
            //Console.WriteLine(target);
            //view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
        }

        public void moverTras(GameTime gameTime)
        {
            time = gameTime.ElapsedGameTime.Milliseconds;
            posicao = posicao - velocidade * vetorBase * time;
            target = posicao + vetorBase;//posicao + direcao;
            //Console.WriteLine(target);
            //view = Matrix.CreateLookAt(posicao, target, Vector3.Up);
        }

    }
}