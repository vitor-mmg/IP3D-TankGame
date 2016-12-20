using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrimeiraEntrega
{
    class ParticulasTanque
    {
        BasicEffect effect;
        VertexPositionColor[] vertices;
        public Vector3 posicaoInicial, direcao, velocidade;
        float posicaoX, posicaoY;
        public Vector3 posicao;
        Matrix worldMatrix;
        GraphicsDevice device;
        float magnitudeLargura, magnitudeAltura;//magnitude é  distancia entre centro e borda do disco
        float velocidadeQueda;
        public float direcaoDeEsguelha;
        Vector3 centro;
        float randomMagnitude, randomPosicao;
        float time;
        float larguraRetangulo, alturaRetangulo;
        public ParticulasTanque(GraphicsDevice device, float largura, float altura, Vector3 centro, Matrix sistemaWorld)
        {
            this.centro = centro;
            this.larguraRetangulo = largura;
            this.alturaRetangulo = altura;
            this.device = device;
            //vetices que compoem a particula.
            vertices = new VertexPositionColor[2];
            //effect
            effect = new BasicEffect(device);

            worldMatrix = Matrix.Identity;
            // direcaoDeEsguelha define o limite de inclinaçao que a direcao da particula pode obter.
            direcaoDeEsguelha = 0.2f;
            //define a velocidade a que a particula se desloca.
            velocidadeQueda = 0.03f;
            effect.VertexColorEnabled = true;
        }

        public void CreateParticle(GameTime gametime, Vector3 posicaoCentro, float larguraRetangulo, float alturaRetangulo, Vector3 novaDirecao, Tanque tank, Matrix worldSistema)
        {
            worldMatrix = worldSistema;
            centro = new Vector3(0, 0, 0);
            time += (float)gametime.ElapsedGameTime.TotalMilliseconds;
            //geracao de valores random para definir posicao e magnitude
            randomPosicao = RandomGenerator.getRandomNext();
            randomMagnitude = RandomGenerator.getRandomMinMax();

            magnitudeLargura = randomMagnitude;
            magnitudeAltura = randomMagnitude;

            //para definir a posicao soma-se ao centro o valor da largura do retangulo mais a magnitude para que encontre
            //em ponto intermiedio entre o centro e o limite exterior.
            posicaoX = centro.X + larguraRetangulo * magnitudeLargura;
            posicaoY = centro.Y /*+ alturaRetangulo * magnitudeAltura*/;
            this.posicao = new Vector3(posicaoX, posicaoY, centro.Z);

            //criaçao dos vertices que compoem a particula, um recebe a posicao calculada o outro é criado um pouco abaixo.
            vertices[0].Position = this.posicao;
            vertices[0].Color = Color.DarkGreen;
            vertices[1].Position = this.posicao + new Vector3(0, 1f, 0);
            vertices[1].Color = Color.Yellow;

            //a direcao da particula é calculada atraves do cross entre o vetor direcao do tanque e o vetor direita.
            Vector3 direcao = RandomGenerator.getRandomNextDouble() * novaDirecao + new Vector3(0, 1, 0);
            velocidade = direcao;
        }

        public void Update(GameTime gametime)
        {
            effect.World = this.worldMatrix;
            //worldMatrix = Matrix.Identity;
            time = (float)gametime.ElapsedGameTime.TotalSeconds;

            Vector3 acelaracao = new Vector3(0, -0.98f, 0);
            velocidade = velocidade + acelaracao * velocidadeQueda;
            posicao = posicao + velocidade * time;
            // actualiza-se a posicao, somando-lhe a direcao(velocidade) e multiplica-se pela velocidade de queda definida no construtor.

            vertices[0].Position = posicao;
            vertices[1].Position = posicao + new Vector3(0, 0.02f, 0);
        }

        public void Draw(Matrix Cview, Matrix Cproj, Matrix sistemaWorld, GraphicsDevice device)
        {
            effect.TextureEnabled = false;
            effect.VertexColorEnabled = true;
            this.effect.View = Cview;
            this.effect.Projection = Cproj;

            effect.CurrentTechnique.Passes[0].Apply();
            //cada instancia da partcula desenha os seus dois vertices
            device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
            //Create3DAxis.Draw(device, effect, Cview, Cproj, this.worldMatrix);
        }
    }
}