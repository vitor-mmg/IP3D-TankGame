using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PrimeiraEntrega
{
    class Boid
    {
        static float distancia;
        static float minidistancia;
        static Vector3 forca;
        static int cont;

        static public void Initialize()
        {
            minidistancia = 25f;
        }

        static public Vector3 Behaviour(List<Tanque> listatank, Tanque tank)
        {
            cont = 0;
            forca = Vector3.Zero;
            foreach (Tanque npc in listatank)
            {
                if (!npc.playerControl)
                {
                    distancia = Vector3.Distance(tank.position, npc.position);
                    if (distancia > 0 && distancia < minidistancia)
                    {
                        Vector3 dif = tank.position - npc.position;
                        Vector3.Normalize(dif);
                        forca += dif;

                        cont++;
                    }
                }
            }
            if (cont > 0)
            {
                forca = forca / cont;
            }

            return forca;
        }
    }
}
