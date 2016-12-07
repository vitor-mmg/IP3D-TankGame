using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrimeiraEntrega
{
    class GeneradorBalas
    {

        static List<Balas> balasAtivas;
        static List<Balas> balasNaoAtivas;
        static List<Balas> copiaBalasAtivas;
        static int numeroDeBalas;
        static Tanque tank;
        static ContentManager content;
        static Balas balaTemp;
        static Vector3 posicaoBala, direcaoBala;


        static public void Initialize(Tanque tanqeAdisparar, ContentManager cont)
        {
            balasAtivas = new List<Balas>(500);
            balasNaoAtivas = new List<Balas>(500);
            tank = tanqeAdisparar;
            content = cont;
            numeroDeBalas = 500;
            copiaBalasAtivas = balasAtivas;

            for (int i = 0; i < numeroDeBalas; i++)
            {
                balasNaoAtivas.Add(new Balas(content));
            }
        }
        static public void PosicaoDirecaoBala()
        {

            Vector3 offset = new Vector3(0, 2, 3);
            Matrix rotacao = Matrix.CreateRotationX(tank.CannonRotation) * Matrix.CreateRotationY(tank.TurretRotation) * Matrix.CreateFromQuaternion(tank.posicao.Rotation);

            offset = Vector3.Transform(offset, rotacao);
            direcaoBala = Vector3.Transform(new Vector3(0, 0, 1), rotacao);
            posicaoBala = tank.posicao + offset;
        }



    }
}

