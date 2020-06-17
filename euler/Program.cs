using System;
using System.Security.Authentication;

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

        /// <summary>
        /// Программа нахождения групп некратных друг другу чисел в отрезке числового ряда
        /// от 1 до заданного целого числа.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            int bigNum = 50;                                                    // длинна числового ряда от 1 до bigNum
            byte[] mask = new byte[bigNum+1];                                   // маска чисел, инициализируем единицами:
            
            for (int i = 1; i <= bigNum; i++)
            {
                mask[i] = 1;
            }

            int[] deb_group = new int[bigNum];                                  // временный массив для хранения группы, нужен только для отладки
            int mCount = 1;                                                     // счетчик групп чисел. Первая группа состоит из одного числа - 1
            Console.WriteLine($"Заданный числовой отрезок: от 1 до {bigNum}.\n");
            Console.WriteLine($"Группа { mCount}:\t\t{1: 6}\n");
            int kGroup;                                                        // индекс для обращения к элементу группы
            int iMask;                                                         // индекс маски
            
            while (true)
            {
                iMask = 1;
                kGroup = 0;
                do                                                              // находим первый элемент группы - первое число, которое не было ранее задействовано
                {
                    if (mask[iMask] != 0)
                    {
                        deb_group[0] = iMask;
                        mask[iMask] = 0;                                            // и маскируем его, чтобы больше не использовать
                        iMask++;
                    }
                }
                while (deb_group[0] == 0 || iMask <= bigNum);

                if (iMask > bigNum && deb_group[0] != 0)
                {
                    Console.Write($"Группа {mCount}:\t\t {deb_group[0]: 6}");
                    break;
                }
                else if (iMask > bigNum) break;

                for (int j = 0; j < length; j++)
                {

                }
            }
             



        }
    }
}
