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
        static private short[] indice;

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
             
            //Gerar vértices
            int x = 0, z = 0;
            for (int j = 0; j < altura / 2; j++) 
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
                        x++;
                        z = 0;
                    }
                }
            }
            
            //alteraçoes
            //Gerar índices
            indice = new short[(altura * 2) * (altura - 1)];

            for (short i = 0; i < indice.Length / 2; i++)
            {
                indice[2 * i] = (short)i;
                indice[2 * i + 1] = (short)(i + altura);
            }

            //Calcular normais
            CalcularNormais();

            //Passar informação para o GPU
            vertexBuffer = new VertexBuffer(graphics,
                typeof(VertexPositionNormalTexture), vertexes.Length,
                BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertexes);

            indexBuffer = new IndexBuffer(graphics, typeof(int), indice.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData<short>(indice);

            //Definir os buffers a utilizar
            graphics.SetVertexBuffer(vertexBuffer);
            graphics.Indices = indexBuffer;

            //Ativa o anisotropic filtering
            sampler = new SamplerState();
            sampler.Filter = TextureFilter.Anisotropic;
            sampler.MaxAnisotropy = 2;
        }
       static private void CalcularNormais()
        {
            for (int x = 0; x <= altura - 1; x++)
            {
                for (int z = 0; z <= altura - 1; z++)
                {
                    VertexPositionNormalTexture vertice = vertexes[x * altura + z];

                    // centro
                    if (x > 0 && x < altura - 1 && z > 0 && z < altura - 1)
                    {

                        //com 8 cross's
                        VertexPositionNormalTexture verticeCima = vertexes[(x * altura + z) - 1];
                        VertexPositionNormalTexture verticeCimaDireita = vertexes[(x * altura + z) + altura - 1];
                        VertexPositionNormalTexture verticeDireita = vertexes[(x * altura + z) + altura];
                        VertexPositionNormalTexture verticeBaixoDireita = vertexes[(x * altura + z) + altura + 1];
                        VertexPositionNormalTexture verticeBaixo = vertexes[(x * altura + z) + 1];
                        VertexPositionNormalTexture verticeBaixoEsquerda = vertexes[(x * altura + z) + 1 - altura];
                        VertexPositionNormalTexture verticeEsquerda = vertexes[(x * altura + z) - altura];
                        VertexPositionNormalTexture verticeCimaEsquerda = vertexes[(x * altura + z) - altura - 1];


                        Vector3 vectorCima = verticeCima.Position - vertice.Position;
                        Vector3 vectorEsquerda = verticeEsquerda.Position - vertice.Position;
                        Vector3 vectorBaixo = verticeBaixo.Position - vertice.Position;
                        Vector3 vectorDireita = verticeDireita.Position - vertice.Position;

                        Vector3 vectorCimaDireita = verticeCimaDireita.Position - vertice.Position;
                        Vector3 vectorCimaEsquerda = verticeCimaEsquerda.Position - vertice.Position;
                        Vector3 vectorBaixoDireita = verticeBaixoDireita.Position - vertice.Position;
                        Vector3 vectorBaixoEsquerda = verticeBaixoEsquerda.Position - vertice.Position;

                        Vector3 normal1 = Vector3.Cross(vectorCima, vectorCimaEsquerda);
                        Vector3 normal2 = Vector3.Cross(vectorCimaEsquerda, vectorEsquerda);
                        Vector3 normal3 = Vector3.Cross(vectorEsquerda, vectorBaixoEsquerda);
                        Vector3 normal4 = Vector3.Cross(vectorBaixoEsquerda, vectorBaixo);
                        Vector3 normal5 = Vector3.Cross(vectorBaixo, vectorBaixoDireita);
                        Vector3 normal6 = Vector3.Cross(vectorBaixoDireita, vectorDireita);
                        Vector3 normal7 = Vector3.Cross(vectorDireita, vectorCimaDireita);
                        Vector3 normal8 = Vector3.Cross(vectorCimaDireita, vectorCima);


                        Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2) + Vector3.Normalize(normal3) + Vector3.Normalize(normal4) +
                        Vector3.Normalize(normal5) + Vector3.Normalize(normal6) + Vector3.Normalize(normal7) + Vector3.Normalize(normal8)) / 8;

                        vertexes[x * altura + z].Normal = Normal;
                    }

                    // lado esquerdo
                    if (x == 0 && z != 0 && z != altura - 1)
                    {
                        VertexPositionNormalTexture verticeDireita = vertexes[(x * altura + z) + altura];
                        VertexPositionNormalTexture verticeCima = vertexes[(x * altura + z) - 1];
                        VertexPositionNormalTexture verticeBaixo = vertexes[(x * altura + z) + 1];
                        VertexPositionNormalTexture verticeCimaDireita = vertexes[(x * altura + z) + altura - 1];
                        VertexPositionNormalTexture verticeBaixoDireita = vertexes[(x * altura + z) + altura + 1];

                        Vector3 vectorBaixo = verticeBaixo.Position - vertice.Position;
                        Vector3 vectorDireita = verticeDireita.Position - vertice.Position;
                        Vector3 vectorCima = verticeCima.Position - vertice.Position;
                        Vector3 vectorCimaDireita = verticeCimaDireita.Position - vertice.Position;
                        Vector3 vectorBaixoDireita = verticeBaixoDireita.Position - vertice.Position;



                        Vector3 normal1 = Vector3.Cross(vectorBaixo, vectorBaixoDireita);
                        Vector3 normal2 = Vector3.Cross(vectorBaixoDireita, vectorDireita);
                        Vector3 normal3 = Vector3.Cross(vectorDireita, vectorCimaDireita);
                        Vector3 normal4 = Vector3.Cross(vectorCimaDireita, vectorCima);


                        Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2) + Vector3.Normalize(normal3) + Vector3.Normalize(normal4)) / 4;

                        vertexes[x * altura + z].Normal = Normal;



                    }
                    // lado direito
                    if (x == altura - 1 && z != 0 && z != altura - 1)
                    {
                        VertexPositionNormalTexture verticeEsquerda = vertexes[(x * altura + z) - altura];
                        VertexPositionNormalTexture verticeCima = vertexes[(x * altura + z) - 1];
                        VertexPositionNormalTexture verticeBaixo = vertexes[(x * altura + z) + 1];
                        VertexPositionNormalTexture verticeBaixoEsquerda = vertexes[(x * altura + z) + 1 - altura];
                        VertexPositionNormalTexture verticeCimaEsquerda = vertexes[(x * altura + z) - altura - 1];

                        Vector3 vectorBaixo = verticeBaixo.Position - vertice.Position;
                        Vector3 vectorEsquerda = verticeEsquerda.Position - vertice.Position;
                        Vector3 vectorCima = verticeCima.Position - vertice.Position;
                        Vector3 vectorBaixoEsquerda = verticeBaixoEsquerda.Position - vertice.Position;
                        Vector3 vectorCimaEsquerda = verticeCimaEsquerda.Position - vertice.Position;



                        Vector3 normal1 = Vector3.Cross(vectorCima, vectorCimaEsquerda);
                        Vector3 normal2 = Vector3.Cross(vectorCimaEsquerda, vectorEsquerda);
                        Vector3 normal3 = Vector3.Cross(vectorEsquerda, vectorBaixoEsquerda);
                        Vector3 normal4 = Vector3.Cross(vectorBaixoEsquerda, vectorBaixo);

                        Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2) + Vector3.Normalize(normal3) + Vector3.Normalize(normal4)) / 4;

                        vertexes[x * altura + z].Normal = Normal;



                    }
                    // lado superior
                    if (x != 0 && z == 0 && x != altura - 1)
                    {
                        VertexPositionNormalTexture verticeDireita = vertexes[(x * altura + z) + altura];
                        VertexPositionNormalTexture verticeEsquerda = vertexes[(x * altura + z) - altura];
                        VertexPositionNormalTexture verticeBaixo = vertexes[(x * altura + z) + 1];
                        VertexPositionNormalTexture verticeBaixoEsquerda = vertexes[(x * altura + z) + 1 - altura];
                        VertexPositionNormalTexture verticeBaixoDireita = vertexes[(x * altura + z) + altura + 1];

                        Vector3 vectorBaixo = verticeBaixo.Position - vertice.Position;
                        Vector3 vectorEsquerda = verticeEsquerda.Position - vertice.Position;
                        Vector3 vectorDireita = verticeDireita.Position - vertice.Position;
                        Vector3 vectorBaixoEsquerda = verticeBaixoEsquerda.Position - vertice.Position;
                        Vector3 vectorBaixoDireita = verticeBaixoDireita.Position - vertice.Position;


                        Vector3 normal1 = Vector3.Cross(vectorEsquerda, vectorBaixoEsquerda);
                        Vector3 normal2 = Vector3.Cross(vectorBaixoEsquerda, vectorBaixo);
                        Vector3 normal3 = Vector3.Cross(vectorBaixo, vectorBaixoDireita);
                        Vector3 normal4 = Vector3.Cross(vectorBaixoDireita, vectorDireita);

                        Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2) + Vector3.Normalize(normal3) + Vector3.Normalize(normal4)) / 4;

                        vertexes[x * altura + z].Normal = Normal;



                    }

                    // lado inferior
                    if (x != 0 && z == altura - 1 && x != altura - 1)
                    {
                        VertexPositionNormalTexture verticeDireita = vertexes[(x * altura + z) + altura];
                        VertexPositionNormalTexture verticeEsquerda = vertexes[(x * altura + z) - altura];
                        VertexPositionNormalTexture verticeCima = vertexes[(x * altura + z) - 1];
                        VertexPositionNormalTexture verticeCimaEsquerda = vertexes[(x * altura + z) - altura - 1];
                        VertexPositionNormalTexture verticeCimaDireita = vertexes[(x * altura + z) + altura - 1];

                        Vector3 vectorCima = verticeCima.Position - vertice.Position;
                        Vector3 vectorEsquerda = verticeEsquerda.Position - vertice.Position;
                        Vector3 vectorDireita = verticeDireita.Position - vertice.Position;
                        Vector3 vectorCimaEsquerda = verticeCimaEsquerda.Position - vertice.Position;
                        Vector3 vectorCimaDireita = verticeCimaDireita.Position - vertice.Position;


                        Vector3 normal1 = Vector3.Cross(vectorDireita, vectorCimaDireita);
                        Vector3 normal2 = Vector3.Cross(vectorCimaDireita, vectorCima);
                        Vector3 normal3 = Vector3.Cross(vectorCima, vectorCimaEsquerda);
                        Vector3 normal4 = Vector3.Cross(vectorCimaEsquerda, vectorEsquerda);

                        Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2) + Vector3.Normalize(normal3) + Vector3.Normalize(normal4)) / 4;

                        vertexes[x * altura + z].Normal = Normal;


                    }
                    if (x == 0 && z == 0)
                    {
                        VertexPositionNormalTexture verticeDireita = vertexes[(x * altura + z) + altura];
                        VertexPositionNormalTexture verticeBaixo = vertexes[(x * altura + z) + 1];
                        VertexPositionNormalTexture verticeBaixoDireita = vertexes[(x * altura + z) + altura + 1];

                        Vector3 vectorDireita = verticeDireita.Position - vertice.Position;
                        Vector3 vectorBaixo = verticeBaixo.Position - vertice.Position;
                        Vector3 vectorBaixoDireita = verticeBaixoDireita.Position - vertice.Position;

                        Vector3 normal1 = Vector3.Cross(vectorBaixo, vectorBaixoDireita);
                        Vector3 normal2 = Vector3.Cross(vectorBaixoDireita, vectorDireita);
                        Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2)) / 2;
                        vertexes[x * altura + z].Normal = Normal;

                    }
                    if (x == 0 && z == altura - 1)
                    {
                        VertexPositionNormalTexture verticeCima = vertexes[(x * altura + z) - 1];
                        VertexPositionNormalTexture verticeDireita = vertexes[(x * altura + z) + altura];
                        VertexPositionNormalTexture verticeCimaDireita = vertexes[(x * altura + z) + altura - 1];

                        Vector3 vectorDireita = verticeDireita.Position - vertice.Position;
                        Vector3 vectorCima = verticeCima.Position - vertice.Position;
                        Vector3 vectorCimaDireita = verticeCimaDireita.Position - vertice.Position;

                        Vector3 normal1 = Vector3.Cross(vectorDireita, vectorCimaDireita);
                        Vector3 normal2 = Vector3.Cross(vectorCimaDireita, vectorCima);
                        Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2)) / 2;
                        vertexes[x * altura + z].Normal = Normal;

                    }

                    if (x == altura - 1 && z == 0)
                    {
                        VertexPositionNormalTexture verticeBaixo = vertexes[(x * altura + z) + 1];
                        VertexPositionNormalTexture verticeEsquerda = vertexes[(x * altura + z) - altura];
                        VertexPositionNormalTexture verticeBaixoEsquerda = vertexes[(x * altura + z) + 1 - altura];

                        Vector3 vectorEsquerda = verticeEsquerda.Position - vertice.Position;
                        Vector3 vectorBaixo = verticeBaixo.Position - vertice.Position;
                        Vector3 vectorBaixoEsquerda = verticeBaixoEsquerda.Position - vertice.Position;

                        Vector3 normal1 = Vector3.Cross(vectorEsquerda, vectorBaixoEsquerda);
                        Vector3 normal2 = Vector3.Cross(vectorBaixoEsquerda, vectorBaixo);
                        Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2)) / 2;
                        vertexes[x * altura + z].Normal = Normal;
                    }

                    if (x == altura - 1 && z == altura - 1)
                    {
                        VertexPositionNormalTexture verticeCima = vertexes[(x * altura + z) - 1];
                        VertexPositionNormalTexture verticeEsquerda = vertexes[(x * altura + z) - altura];
                        VertexPositionNormalTexture verticeCimaEsquerda = vertexes[(x * altura + z) - altura - 1];

                        Vector3 vectorEsquerda = verticeEsquerda.Position - vertice.Position;
                        Vector3 vectorCima = verticeCima.Position - vertice.Position;
                        Vector3 vectorCimaEsquerda = verticeCimaEsquerda.Position - vertice.Position;

                        Vector3 normal1 = Vector3.Cross(vectorCima, vectorCimaEsquerda);
                        Vector3 normal2 = Vector3.Cross(vectorCimaEsquerda, vectorEsquerda);
                        Vector3 Normal = (Vector3.Normalize(normal1) + Vector3.Normalize(normal2)) / 2;
                        vertexes[x * altura + z].Normal = Normal;
                    }


                }
            }
        }
       
        static public Vector3 NormalHeighmap(Vector3 posicao)
       {
           //Posição arredondada para baixo da camara
           int xTank, zTank;
           xTank = (int)posicao.X;
           zTank = (int)posicao.Z;

           //Os 4 vértices que rodeiam a posição da camara
           Vector2 pontoA, pontoB, pontoC, pontoD;
           pontoA = new Vector2(xTank, zTank);
           pontoB = new Vector2(xTank + 1, zTank);
           pontoC = new Vector2(xTank, zTank + 1);
           pontoD = new Vector2(xTank + 1, zTank + 1);


           Vector3 Ya, Yb, Yc, Yd;
           Ya = Terreno.vertexes[(int)pontoA.X * Terreno.altura + (int)pontoA.Y].Normal;
           Yb = Terreno.vertexes[(int)pontoB.X * Terreno.altura + (int)pontoB.Y].Normal;
           Yc = Terreno.vertexes[(int)pontoC.X * Terreno.altura + (int)pontoC.Y].Normal;
           Yd = Terreno.vertexes[(int)pontoD.X * Terreno.altura + (int)pontoD.Y].Normal;

           //Interpolação bilenear (dada nas aulas)
           Vector3 Yab = (1 - (posicao.X - pontoA.X)) * Ya + (posicao.X - pontoA.X) * Yb;
           Vector3 Ycd = (1 - (posicao.X - pontoC.X)) * Yc + (posicao.X - pontoC.X) * Yd;
           Vector3 Y = (1 - (posicao.Z - pontoA.Y)) * Yab + (posicao.Z - pontoA.Y) * Ycd;

           //Devolver normal
           return Y;
       }
        static public float AlturaHeighmap(Vector3 positiçao)
        {
            //Posição arredondada para baixo da camara
            int xTank, zTank;
            xTank = (int)positiçao.X;
            zTank = (int)positiçao.Z;

            //Os 4 vértices que rodeiam a posição da camara
            Vector2 pontoA, pontoB, pontoC, pontoD;
            pontoA = new Vector2(xTank, zTank);
            pontoB = new Vector2(xTank + 1, zTank);
            pontoC = new Vector2(xTank, zTank + 1);
            pontoD = new Vector2(xTank + 1, zTank + 1);

            if (positiçao.X > 0 && positiçao.X < Terreno.altura
                        && positiçao.Z > 0 && positiçao.Z < Terreno.altura)
            {

                //Recolher a altura de cada um dos 4 vértices à volta do tanque a partir do heightmap
                float Ya, Yb, Yc, Yd;
                Ya = Terreno.vertexes[(int)pontoA.X * Terreno.altura + (int)pontoA.Y].Position.Y;
                Yb = Terreno.vertexes[(int)pontoB.X * Terreno.altura + (int)pontoB.Y].Position.Y;
                Yc = Terreno.vertexes[(int)pontoC.X * Terreno.altura + (int)pontoC.Y].Position.Y;
                Yd = Terreno.vertexes[(int)pontoD.X * Terreno.altura + (int)pontoD.Y].Position.Y;

                //Interpolação bilenear 
                float Yab = (1 - (positiçao.X - pontoA.X)) * Ya + (positiçao.X - pontoA.X) * Yb;
                float Ycd = (1 - (positiçao.X - pontoC.X)) * Yc + (positiçao.X - pontoC.X) * Yd;
                float Y = (1 - (positiçao.Z - pontoA.Y)) * Yab + (positiçao.Z - pontoA.Y) * Ycd;

                //Devolver a altura + um offset
                return Y + 0.01f;
            }
            else
            {
                return -1;
            }
        }
        static public VertexPositionNormalTexture[] getVertices()
        {
            return (vertices);
        }
        public static VertexPositionNormalTexture[] vertices { get; set; }

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
                    indice,
                    0,
                    altura * 2 - 2);
            }
        }

        
    }
}
