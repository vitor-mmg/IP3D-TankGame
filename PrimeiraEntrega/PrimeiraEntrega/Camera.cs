
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeiraEntrega
{
    static class Camera
    {
        //Matrizes World, View e Projection
        static public Matrix World, View, Projection;
        //Posição da camara
        static private Vector3 posicao, positionAnterior;

        //Rotação horizontal
        static float leftrightRot = 0f;
        //Rotação vertical
        static float updownRot = 0f;
        //Velocidade da rotação
        const float rotationSpeed = 0.3f;
        //Velocidade do movimento com o rato
        const float moveSpeed = 10f;
        //Estado do rato
        static private MouseState originalMouseState;
        //BoundingFrustum da camâra
        static public BoundingFrustum frustum;
        //Tamanho do "mundo"
        static public int worldSize = 700;
        //Near e far plane
        static public float nearPlane = 0.1f;
        static public float farPlane = worldSize ;

        static VertexPositionNormalTexture[] vertices;
        static int heightmap;

        /// Inicializa os componentes da camara

        static public void Initialize(GraphicsDevice graphics, VertexPositionNormalTexture[] vertexes, int alturaMapa)
        {
            heightmap = alturaMapa;
            //Posição inicial da camâra
            posicao = new Vector3(0, 1, 5);
            //Inicializar as matrizes world, view e projection
            World = Matrix.Identity;
            UpdateViewMatrix();
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                graphics.Viewport.AspectRatio,
                nearPlane,
                farPlane);

            vertices = vertexes;

            //Criar e definir o resterizerState a utilizar para desenhar a geometria
            RasterizerState rasterizerState = new RasterizerState();
            //Desenha todas as faces, independentemente da orientação
            rasterizerState.CullMode = CullMode.None;
            //rasterizerState.FillMode = FillMode.WireFrame;
            rasterizerState.MultiSampleAntiAlias = true;
            graphics.RasterizerState = rasterizerState;

            Mouse.SetPosition(graphics.Viewport.Width / 2, graphics.Viewport.Height / 2);

            originalMouseState = Mouse.GetState();
        }
    
        static private void UpdateViewMatrix()
        {
            //Cálculo da matriz de rotação
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            //Target
            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = posicao + cameraRotatedTarget;

            //Cálculo do vector Up
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);
           
            //Matriz View
            View = Matrix.CreateLookAt(posicao, cameraFinalTarget, cameraRotatedUpVector);
            
            frustum = new BoundingFrustum(View * Projection);
        }
        
        static float surfaceFollow()
        { 
            //Posição arredondada para baixo
            int xPos, zPos;
            xPos = (int)posicao.X;
            zPos = (int)posicao.Z;

            //Os 4 vértices que rodeiam a posição da camara
            Vector2 pontoA, pontoB, pontoC, pontoD;
            pontoA = new Vector2(xPos, zPos);
            pontoB = new Vector2(xPos + 1, zPos);
            pontoC = new Vector2(xPos, zPos + 1);
            pontoD = new Vector2(xPos + 1, zPos + 1);

            if (pontoA.X > 0 && pontoA.X < Terreno.altura
            && pontoA.Y > 0 && pontoA.Y < Terreno.altura
            && pontoB.X > 0 && pontoB.X < Terreno.altura
            && pontoB.Y > 0 && pontoB.Y < Terreno.altura
            && pontoC.X > 0 && pontoC.X < Terreno.altura
            && pontoC.Y > 0 && pontoC.Y < Terreno.altura
            && pontoD.X > 0 && pontoD.X < Terreno.altura
            && pontoD.Y > 0 && pontoD.Y < Terreno.altura)
            {
                //Recolher a altura de cada um dos 4 vértices à volta da câmara a partir do heightmap


                float Ya, Yb, Yc, Yd;
                Ya = Terreno.vertexes[(int)pontoA.X * Terreno.altura + (int)pontoA.Y].Position.Y;
                Yb = Terreno.vertexes[(int)pontoB.X * Terreno.altura + (int)pontoB.Y].Position.Y;
                Yc = Terreno.vertexes[(int)pontoC.X * Terreno.altura + (int)pontoC.Y].Position.Y;
                Yd = Terreno.vertexes[(int)pontoD.X * Terreno.altura + (int)pontoD.Y].Position.Y;
              

                //Interpolação bilenear (dada nas aulas)
                float Yab = (1 - (posicao.X - pontoA.X)) * Ya + (posicao.X - pontoA.X) * Yb;
                float Ycd = (1 - (posicao.X - pontoC.X)) * Yc + (posicao.X - pontoC.X) * Yd;
                float Y = (1 - (posicao.Z - pontoA.Y)) * Yab + (posicao.Z - pontoA.Y) * Ycd;
                
                //Devolver a altura
                return Y;
            }
            else
            {
                return -1;
            }
        }

        static private void ProcessInput(float amount, GraphicsDevice graphics)
        {
            //Movimento do rato
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != originalMouseState)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                leftrightRot -= rotationSpeed * xDifference * amount;
                updownRot -= rotationSpeed * yDifference * amount;
                try
                {
                    Mouse.SetPosition(graphics.Viewport.Width / 2, graphics.Viewport.Height / 2);
                }
                catch (Exception)
                {
                    //Impede de dar erro quando se sai do programa
                }
                UpdateViewMatrix();
            }

            //Controlos do teclado
            Vector3 moveVector = new Vector3(0, 0, 0);
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.NumPad8))
                moveVector += new Vector3(0, 0, -1);
            if (keyState.IsKeyDown(Keys.NumPad5))
                moveVector += new Vector3(0, 0, 1);
            if (keyState.IsKeyDown(Keys.NumPad6))
                moveVector += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.NumPad4))
                moveVector += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.NumPad7))
                moveVector += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.NumPad9))
                moveVector += new Vector3(0, -1, 0);
            AddToCameraPosition(moveVector * amount);
            positionAnterior = posicao;

        }
        /// Atualiza a posição da camâra
     
        static private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            posicao += moveSpeed * rotatedVector;
            UpdateViewMatrix();
            //posicao.Y = surfaceFollow(); // Linha que define camara como surface follow, se nao tiver aqui a camara e igual a uma livre.
        }
        /// Atualiza os parâmetros da camâra
    
        static public void Update(GameTime gameTime, GraphicsDevice graphics)
        {
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            ProcessInput(timeDifference, graphics);
        }
       
      
    }

}