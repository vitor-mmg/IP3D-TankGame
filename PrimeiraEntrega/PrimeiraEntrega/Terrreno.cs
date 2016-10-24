using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PrimeiraEntrega
{
    class Terreno
    {
        //Array de vértices
        static public VertexPositionNormalTexture[] vertexes;

        //Array de índices
        static private int[] indexes;

        //Array de texels
        static Color[] texels;

        //Buffers
        static private VertexBuffer vertexBuffer;
        static private IndexBuffer indexBuffer;

        static SamplerState sampler;

        //Dimensões do terreno
        static public int altura;

        static public void GenerateTerrain(GraphicsDevice graphics, Texture2D heightmap)
        {
            //Gerar texels a partir do heightmap
            texels = new Color[heightmap.Width * heightmap.Width];
            heightmap.GetData<Color>(texels);

            altura = heightmap.Height;
            vertexes = new VertexPositionNormalTexture[altura * altura];
             //altera isto 
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
                    vertexes[(2 * j * altura) + i] = new VertexPositionNormalTexture(
                        new Vector3(x, texels[(z * altura + x)].R * scale, z),
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
            //ate aki
            //Calcular normais

            //Passar informação para o GPU
            vertexBuffer = new VertexBuffer(graphics,
                typeof(VertexPositionNormalTexture), vertexes.Length,
                BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertexes);

            indexBuffer = new IndexBuffer(graphics, typeof(int), indexes.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData<int>(indexes);

            //Definir os buffers a utilizar
            graphics.SetVertexBuffer(vertexBuffer);
            graphics.Indices = indexBuffer;

            //Ativa o anisotropic filtering
            sampler = new SamplerState();
            sampler.Filter = TextureFilter.Anisotropic;
            sampler.MaxAnisotropy = 4;
        }
        
        static public VertexPositionNormalTexture[] getVertexes()
        {
            return (vertexes);
        }

        static public void Draw(GraphicsDevice graphics, BasicEffect efeito)
        {
            //World, View, Projection
            efeito.World = Matrix.Identity; 
            efeito.View = Camera.View;
            efeito.Projection = Camera.Projection;
            
            graphics.SamplerStates[0] = sampler;
            
            efeito.CurrentTechnique.Passes[0].Apply();

            //Desenhar o terreno, uma strip de cada vez
            for (int i = 0; i < altura - 1; i++)
            {
                graphics.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip,
                    vertexes,
                    i * altura,
                    altura * 2,
                    indexes,
                    0,
                    altura * 2 - 2);
            }
        }
    }
}
