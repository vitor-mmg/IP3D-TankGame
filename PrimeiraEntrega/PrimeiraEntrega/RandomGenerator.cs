using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrimeiraEntrega
{
    //Classe estatica que gera valores aleatorios.
    static class RandomGenerator
    {
        static Random random = new Random();
        static float min = 0;
        static float max = 1;

        public static void Initialize()
        {
            min = -1;
            max = 1;
        }
        //retorna o proximo valor double.
        public static float getRandomNextDouble()
        {
            return ((float)random.NextDouble());
        }
        //retorna o proximo valor inteiro.
        public static float getRandomNext()
        {
            return ((float)random.Next());
        }

        public static float getRandomMinMax()
        {
            return ((float)random.NextDouble() * (max - min) + min);
        }
    }
}