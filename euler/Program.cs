﻿using System;
using System.Security.Authentication;

namespace euler
{    
    class Program
    {
        // Задача сводится к подсчету простых множителей каждого целого числа от 1 до заданного.
        // Для этого построим массив, индексы которого будут наши числа, а значение элемента - количество простых множителей
        // этого числа. 2 множителя - число простое (1 и само число) - эти числа попадут в группу №2
        // Все остальные числа будут формировать группы с номером, равным количеству их простых множителей (включая 1),
        // т.к. внутри группы они не будут взаимно делиться.

        /// <summary>
        /// Подсчитывает количество простых множителей для каждого целого числа от 1 до N используя модифицированное решето Эйлера.
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        static byte[] OptimizedPrimeCount(int N)
        {
            int[] lp = new int[N + 1];   //массив наименьших делителей, изначально напротив каждого числа (индекса массива) стоит 0

            for (int i = 2; i <= N; i++)
            {
                if (lp[i] == 0)
                   lp[i] = i;                                           // это простое число и мы записываем туда само число как наименьший делитель
                for (int p = 2; p <= lp[i]; p++)
                {
                    if (p * i > N) break;                               // если за пределами отрезка, бросаем это
                    if (p == lp[p])                                     // если p  простое   
                        lp[i * p] = p;                                  // все кратные ему числа будут иметь такой минимальный делитель.
                }

            }

            // после того, как сформировали массив с минимальными делителями всех чисел, перепишем его под наши нужды:
            // напротив каждого индекса должен стоять номер группы, в которую оно попадает, равный числу его простых множителей, включая 1

            byte[] allNums = new byte[N + 1];   

            // allNums[0] = 0 - ноль нас не интересует

            allNums[1] = 1;
            int q;              //частное от деления

            for (int i = 2; i < allNums.Length; i++)
            {
                if (lp[i] == i)
                    allNums[i] = 2;                                         // это простое число
                else
                {
                    q = i / lp[i];                                          // если не простое, находим частное от деления на наименьший множитель
                    allNums[i] = (byte)(1 + allNums[q]);                    // и подсчитываем число его простых множителей как 1 + к-во множителей частного
                }
            }
            return allNums;
        }


        /// <summary>
        /// Подсчитывает количество простых множителей для каждого целого числа от 1 до N
        /// </summary>
        /// <param name="N">Верхняя граница числового отрезка</param>
        /// <returns>Массив, содержащий количество простых множителей (включая 1) для каждого значения индекса</returns>
        static byte[] Factorized(int N)
        {
            byte[] allNums = new byte[N + 1];   //изначально напротив каждого числа (индекса массива) стоит 0

            // allNums[0] = 0 - ноль нас не интересует

            allNums[1] = 1;
            allNums[2] = 2;             // простое число
            allNums[3] = 2;
            int q;                      // частное от деления
            bool isPrime;

            for (int i = 4; i < allNums.Length; i++)
            {
                isPrime = true;                                             // начальное предположение
                for (int j = 2; j <= (int)Math.Sqrt(i); j++)                //нет смысла искать простые множители числа, большие его квадратного корня
                {
                    if (allNums[j] == 2)                                    //проверяем делимость только на простые числа!
                    {

                        if (i % j == 0) // нужно оптимизировать иф
                        {
                            q = i / j;
                            allNums[i] = (byte)(1 + allNums[q]);            // если разделилось - плюсуем единичку
                                                                            // и добавляем все делители полученного частного, которые уже известны
                            isPrime = false;                                // и оно точно не простое
                            break;                                          // для этого числа работа сделана
                        }
                    }
                }
                if (isPrime)
                    allNums[i] = 2;
            }
            return allNums;
        }

       /// <summary>
       ///  Печатает массив без нулей как группу с заданным номером. Нужна для отладки кода.
       /// </summary>
       /// <param name="group"></param>
       /// <param name="num"></param>
        static void PrintGroup(int[] group, int num)
        {
            Console.Write($"Группа {num}:\t");
            foreach (var item in group)
            {
                if (item > 0)
                Console.Write($" {item}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Анализирует полученный байтовый массив в котором записаны номера групп, в которые входит число,
        /// представленное индексом, и выводит на печать эти группы.
        /// </summary>
        /// <param name="nums"></param>
        static void PrintAllGroups(byte[] nums)
        {
            int count = Convert.ToInt32(Math.Floor(Math.Log2(nums.GetUpperBound(0)))) + 1;

            for (byte i = 1; i <= count; i++)
            {
                Console.Write($"Группа {i}:\t");
                for (int j = 0; j < nums.Length; j++)
                {
                    if (nums[j] == i)
                        Console.Write(j.ToString() + "  ");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Программа нахождения групп некратных друг другу чисел в отрезке числового ряда
        /// от 1 до заданного целого числа.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // ******************************************************************************************
            int bigNum = 1_000_000_000;                           // длинна числового ряда от 1 до bigNum
            DateTime time = DateTime.Now;

            Console.WriteLine($"Заданный числовой отрезок: от 1 до {bigNum : ### ### ### ###}.\n");

            // ******************************************************************************************
            byte[] groups = OptimizedPrimeCount(bigNum);
           
            //PrintAllGroups(groups);
            
            //PrintGroup(test, 0);

            TimeSpan wasted = DateTime.Now.Subtract(time);
            Console.WriteLine("Всё закончилось :(");
            Console.WriteLine($"Было сформировано {Convert.ToInt32(Math.Floor(Math.Log2(bigNum))) + 1} групп.");
            Console.WriteLine($"Для этого потребовалось {wasted.TotalSeconds} секунд машинного времени.");
            Console.ReadKey();

        }
    }
}
