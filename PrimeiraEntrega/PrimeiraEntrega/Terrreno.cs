
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
     /*   
        static public VertexPositionNormalTexture[] vertices;

        static private int[] indexes;

        //array para os texels
        static Color[] txls;

        static private VertexBuffer vbuffer;
        static private IndexBuffer ibufer;


        static SamplerState sampler;

        static public int altura;

        static public void GerarTerreno(GraphicsDevice graphics, Texture2D heighmap)
        {
            //so it begins
            txls = new Color[heighmap.Width * heighmap.Height];
            heighmap.GetData<Color>(txls);

            altura = heighmap.Height;

            vertices = new VertexPositionNormalTexture[altura * altura];

            //Gerar vértices
            int x = 0, z = 0;
            for (int j = 0; j < altura / 2; j++) //Criamos duas colunas de vértices de cada vez
            {
                for (int i = 0; i < altura * 2; i++)
                {
                    //Calcular coordenadas da textura
                    int u, v;

                    u = (x % 2 == 0) ? 0 : 1;
                    v = (z % 2 == 0) ? 0 : 1;

                    //Escalas:
                    //bigaltura (512 * 512): 0.2f;
                    //altura (128 * 128): 0.04f;
                    float scale = 0.04f;
                    vertices[(2 * j * altura) + i] = new VertexPositionNormalTexture(
                        new Vector3(x, txls[(z * altura + x)].R * scale, z),
                        Vector3.Zero,
                        new Vector2(u, v));
                    z++;
                    if (z >= altura)
                    {
                        //Criámos uma faixa vertical de vértices, passar para a outra faixa
                        x++;
                        z = 0;
                    }
                }


            }



            //Gerar índices
            indexes = new int[(altura * 2) * (altura - 1)];

            for (int i = 0; i < indexes.Length / 2; i++)
            {
                indexes[2 * i] = (int)i;
                indexes[2 * i + 1] = (int)(i + altura);
            }

            //Calcular normais
            CalcularNormais();

            //Passar informação para o GPU
            vbuffer = new VertexBuffer(graphics,
                typeof(VertexPositionNormalTexture), vertices.Length, 
                BufferUsage.WriteOnly);
            vbuffer.SetData<VertexPositionNormalTexture>(vertices);

            ibufer = new IndexBuffer(graphics, typeof(int), indexes.Length, BufferUsage.WriteOnly);
            ibufer.SetData<int>(indexes);

            //Definir os buffers a utilizar
            graphics.SetVertexBuffer(vbuffer);
            graphics.Indices = ibufer;

            //Ativa o anisotropic filtering
            sampler = new SamplerState();
            sampler.Filter = TextureFilter.Anisotropic;
            sampler.MaxAnisotropy = 4;
        }
        static private void CalcularNormais()
        {
            //
        }
        
        */

        
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
