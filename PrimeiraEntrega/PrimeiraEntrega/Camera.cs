
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
        static private Vector3 position;
        //Rotação horizontal
        static float leftrightRot = 0f;
        //Rotação vertical
        static float updownRot = 0f;
        //Velocidade da rotação
        const float rotationSpeed = 0.3f;
        //Velocidade do movimento com o rato
        const float moveSpeed = 5f;
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
            position = new Vector3(0, 1, 5);
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
            Vector3 cameraFinalTarget = position + cameraRotatedTarget;
            
            //Cálculo do vector Up
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);
           
            //Matriz View
            View = Matrix.CreateLookAt(position, cameraFinalTarget, cameraRotatedUpVector);
            
            frustum = new BoundingFrustum(View * Projection);
        }
        
        static float surfaceFollow()
        {
            //A e B - vertices superiores
            //C e D - vertices inferiores 
            //A-----------B
            //C-----------D
            int xA, zA, xB, zB, xC, zC, xD, zD;
            float yA = 0, yB = 0, yC = 0, yD = 0;
            xA = (int)position.X;
            zA = (int)position.Z;

            xB = xA + 1;
            zB = zA;
            xC = xA;
            zC = zA + 1;
            xD = xB;
            zD = zC;

            //encontrar valor de Y de cada vertice

            yA = vertices[xA * heightmap + zA].Position.Y;
            yB = vertices[xB * heightmap + zB].Position.Y;
            yC = vertices[xC * heightmap + zC].Position.Y;
            yD = vertices[xD * heightmap + zD].Position.Y;
            
            //calcular nova altura da camara
            float yAB, yCD, cameraY;

            yAB = (1 - (position.X - xA)) * yA + (position.X - xA) * yB;
            yCD = (1 - (position.X - xC)) * yC + (position.X - xC) * yD;
            cameraY = (1 - (position.Z - zA)) * yAB + (position.Z - zA) * yCD;
            return (cameraY + 1);
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
        }
        /// Atualiza a posição da camâra
     
        static private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            position += moveSpeed * rotatedVector;
            UpdateViewMatrix();
            position.Y = surfaceFollow(); // Linha que define camara como surface follow, se nao tiver aqui a camara e igual a uma livre.
        }
        /// Atualiza os parâmetros da camâra
    
        static public void Update(GameTime gameTime, GraphicsDevice graphics)
        {
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            ProcessInput(timeDifference, graphics);
        }
    }
}