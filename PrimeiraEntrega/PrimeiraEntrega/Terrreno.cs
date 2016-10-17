//exemplo
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PrimeiraEntrega
{
    class Terrreno
    {
        BasicEffect effect;
        Matrix worldMatrix;
        VertexPositionColorTexture[] vertices;
        Texture2D textura;

        public Terrreno(GraphicsDevice device, ContentManager content)
        {
            //
            worldMatrix = Matrix.Identity;
            textura = content.Load<Texture2D>("terreno");
            effect = new BasicEffect(device);
            float aspectRatio = (float)device.Viewport.Width /
                           device.Viewport.Height;
            effect.View = Matrix.CreateLookAt(
                                new Vector3(1.0f, 2.0f, 2.0f),
                                Vector3.Zero,
                                Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45.0f),
                               aspectRatio, 1.0f, 10.0f);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;

            //textura
            effect.TextureEnabled = true;
            effect.Texture = textura;

            CreateGeometry();
        }

        private void CreateGeometry()
        {
            //falta "endireitar" as coordenadas com o  prisma e 

            int vertexCount = 4;
            vertices = new VertexPositionColorTexture[vertexCount];
            vertices[0] = new VertexPositionColorTexture(
                new Vector3(-2, 0.0f, -2),
                Color.White, new Vector2(0.0f, 0.0f));
            vertices[1] = new VertexPositionColorTexture(
                new Vector3(2, 0.0f, -2),
                Color.White, new Vector2(1.0f, 0.0f));
            vertices[2] = new VertexPositionColorTexture(
                new Vector3(-2, 0.0f, 2),
                Color.White, new Vector2(0.0f, 1.0f));
            vertices[3] = new VertexPositionColorTexture(
                new Vector3(2, 0.0f, 2),
                Color.White, new Vector2(1.0f, 1.0f));
        }

        public void Draw(GraphicsDevice device)
        {
            effect.World = Camera.World;
            effect.View = Camera.View;
            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserPrimitives<VertexPositionColorTexture>(
                PrimitiveType.TriangleStrip,
                vertices,
                0,
                2);
        }


    }
}
