using System;

namespace euler
{    
    class Program
    {
        /// <summary>
        /// Определяет, делится ли одно число на другое без остатка.
        /// </summary>
        /// <param name="a">делитель</param>
        /// <param name="b">делимое</param>
        /// <returns>true - если b делится на a без остатка, false - если нет</returns>
        static bool Aliquant(int a, int b)
        {
            return (b % a == 0);
        }

        static void Main(string[] args)
        {
            

        }
    }
}
