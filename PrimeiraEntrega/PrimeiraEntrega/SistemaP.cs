using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrimeiraEntrega
{
    class SistemaP
    {
        List<ParticulasTanque> listaParticulas;
        List<ParticulasTanque> listaParticulasAtiva;
        GraphicsDevice device;
        ParticulasTanque particulaTemp;
        public Vector3 posicaoCentro;
        public BasicEffect effect;
        public Matrix worldMatrix;
        int quantidadeParticulas;
        float alturaRetangulo, larguraRetangulo;
        public bool criarParticulas;
        public SistemaP(GraphicsDevice device, Vector3 centro, float largura, float altura, Matrix worldTank)
        {

            quantidadeParticulas = 3000;
            posicaoCentro = centro;
            this.device = device;

            listaParticulas = new List<ParticulasTanque>(quantidadeParticulas);
            listaParticulasAtiva = new List<ParticulasTanque>(quantidadeParticulas);

            effect = new BasicEffect(device);
            worldMatrix = Matrix.Identity;

            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;

            this.larguraRetangulo = largura;
            this.alturaRetangulo = altura;
            CriarParticulas(quantidadeParticulas);

            criarParticulas = true;
        }
        //a lista de particulas nao ativa é preenchida com a quantidade de particulas desejada
        public void CriarParticulas(int quantidadeParticulas)
        {
            for (int i = 0; i < quantidadeParticulas; i++)
            {
                listaParticulas.Add(new ParticulasTanque(device, larguraRetangulo, alturaRetangulo, posicaoCentro, this.worldMatrix));
            }
        }

        public void Update(GameTime gametime, Vector3 posicao, Vector3 novaDirecao, Tanque tank)
        {
            moverParaTraseiraTank(tank);

            //para cada Update retiram-se x particulas da lista nao ativa e colocam-se as mesmas na lista de particulas ativas.
            for (int i = 0; i < 5; i++)
            {
                if (listaParticulasAtiva.Count < quantidadeParticulas - 1000)
                {
                    //particula temporaria recebe a primeira particula da lista de nao ativas.
                    particulaTemp = listaParticulas.First();
                    //calcula posicao e direcao.
                    if (criarParticulas && tank.playerControl)
                        particulaTemp.CreateParticle(gametime, posicaoCentro, larguraRetangulo, alturaRetangulo, novaDirecao, tank, worldMatrix);
                    //adiciona particula a lista ativa.
                    listaParticulasAtiva.Add(particulaTemp);
                    //remove da lista nao ativa.
                    listaParticulas.Remove(particulaTemp);
                }
            }


            foreach (ParticulasTanque p in listaParticulasAtiva)
            {
                //Update de cada particula da lista ativa.
                p.Update(gametime);
                //se a particula ultrapassar a posicao em Y de -10...
                if (p.posicao.Y < -10f)
                {
                    //...é adicionada á lista nao ativa...
                    listaParticulas.Add(p);
                }

            }
            //... e é removida da lista ativa.
            listaParticulasAtiva.RemoveAll(particula => particula.posicao.Y < -10f);

        }

        public void Draw(Matrix view, Matrix proj)
        {
            //cada particula na lista ativa é desenhada.
            foreach (ParticulasTanque p in listaParticulasAtiva)
            {
                p.Draw(view, proj, worldMatrix, device);
            }
            //Create3DAxis.Draw(device, this.effect, view, proj, this.worldMatrix);
        }

        private void moverParaTraseiraTank(Tanque tank)
        {
            Vector3 offset = new Vector3(-0.6f, 0.2f, -1f);
            Matrix rotacao = Matrix.CreateTranslation(offset) * Matrix.CreateFromQuaternion(tank.rotacaoFinal.Rotation);
            Vector3 transformOffset = Vector3.Transform(offset, rotacao);
            this.posicaoCentro = transformOffset + tank.position;

            this.worldMatrix = rotacao;
            this.worldMatrix.Translation = transformOffset + tank.position;
        }
        public BasicEffect getEffect()
        {
            return effect;
        }


    }
}
