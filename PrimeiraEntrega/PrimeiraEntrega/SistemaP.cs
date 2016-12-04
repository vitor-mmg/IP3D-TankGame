using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PrimeiraEntrega
{
    public class SistemaP
    {
        //Propriedades do sistema de particulas
        //Lista de particulas
        List<Particulas> particulas;
        int nParticulas;
        int alturaInicialChuva;
        int raioNuvem;

        public SistemaP(Random random, int raioNuvem, int nParticulas, int alturaInicialChuva)
        {
            //Inicializar as propriedades
            this.nParticulas = nParticulas;
            this.alturaInicialChuva = alturaInicialChuva;
            this.raioNuvem = raioNuvem;
            particulas = new List<Particulas>();

            //Criar um determinado numero de particulas
            for (int i = 0; i < nParticulas; i++)
            {
                inserirNovaParticula(random);
            }
        }

        private void inserirNovaParticula(Random random)
        {
            //Centro da nuvem
            Vector3 centroNuvem = new Vector3(0, alturaInicialChuva, 0);

            Vector3 posicao = Vector3.Zero;

            //Inicializar propriedades da particula a criar
            float angulo = (float)random.NextDouble() * MathHelper.TwoPi;
            float magnitude = (float)random.NextDouble();
            float alturaInicial = (float)random.NextDouble();
            float velocidadeMedia = 0.03f;
            float perturbacao = 0.05f;

            //Posição X inicial da particula
            posicao.X = centroNuvem.X + raioNuvem * magnitude * (float)Math.Cos(angulo);

            if (particulas.Count < nParticulas - 10)
            {
                //Primeiras particulas, não geradas com uma altura aleatória
                posicao.Y = centroNuvem.Y - (alturaInicial * alturaInicialChuva);
            }
            else
            {
                //Já existe um numero grande de particulas, particula nasce no plano da nuvem
                posicao.Y = centroNuvem.Y;
            }

            //Posição Z inicial da particula
            posicao.Z = centroNuvem.X + raioNuvem * magnitude * (float)Math.Sin(angulo);

            //Adicionar nova particula à lista de particulas deste sistema
            particulas.Add(new Particulas(posicao, velocidadeMedia, perturbacao, random));
        }

        public void Update(Random random)
        {
            //Atualizar as particulas de chuva
            foreach (Particulas particula in particulas)
            {
                particula.Update();
            }
            //Verificar particulas que devem morrer e criar novas particulas para as substituir
            matarERenascerParticulas(random);
        }

        private void matarERenascerParticulas(Random random)
        {
            //Encontrar todas as particulas que estejam abaixo do plano
            List<Particulas> listaRemover = particulas.FindAll(x => x.posicao.Y < 0);

            //Remover todas as particulas que se encontram abaixo do plano
            foreach (Particulas particula in listaRemover)
            {
                particulas.Remove(particula);
            }

            //Inserir um numero de particulas igual ao numero de particulas que morreram
            for (int i = 0; i < listaRemover.Count; i++)
            {
                inserirNovaParticula(random);
            }

        }

        public void Draw(GraphicsDevice graphics, BasicEffect efeito)
        {
            //Desenhar as particulas geridas por este sistema
            foreach (Particulas particula in particulas)
            {
                particula.Draw(graphics, efeito);
            }
        }
    }
}