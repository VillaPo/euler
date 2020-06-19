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
            return (b % a != 0);
        }

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
        /// Программа нахождения групп некратных друг другу чисел в отрезке числового ряда
        /// от 1 до заданного целого числа.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            int bigNum = 1_000_000;                                                     // длинна числового ряда от 1 до bigNum
            DateTime time = DateTime.Now; 

            byte[] mask = new byte[bigNum+1];                                           // маска чисел, инициализируем единицами:
            
            for (int i = 2; i <= bigNum; i++)
            {
                mask[i] = 1;
            }
           
            int[] deb_group = new int[bigNum];                                          // временный массив для хранения группы, нужен только для отладки
            int mCount = 1;                                                             // счетчик групп чисел. Первая группа состоит из одного числа - 1
            Console.WriteLine($"Заданный числовой отрезок: от 1 до {bigNum}.\n");
            Console.WriteLine($"Группа 1:\t 1\n");
            int kGroup;                                                                 // индекс для обращения к элементу группы
            int iMask;                                                                  // индекс маски
            int lastPosFirst =1;                                                        // последний из найденных первых элементов группы
            
            while (true)
            {
                iMask = lastPosFirst + 1;
                for (kGroup = 0; kGroup < bigNum; kGroup++)
                    deb_group[kGroup] = 0;
                do                                                                      // находим первый элемент группы - первое число, которое не было ранее задействовано
                {
                    if (mask[iMask] == 0)
                    {
                        iMask++;
                        continue;
                    }    
                    deb_group[0] = iMask;
                    mask[iMask] = 0;                                                // и маскируем его, чтобы больше не использовать
                    lastPosFirst = iMask;
                    iMask++;
                }
                while (deb_group[0] == 0 && iMask <= bigNum);

                if (iMask > bigNum && deb_group[0] != 0)
                {
                    mCount++;
                    Console.Write($"Группа {mCount}:\t {deb_group[0]}");
                    Console.ReadKey();
                    break;
                }
                else if (iMask > bigNum) break;

                                                                 
                
                for (kGroup = 1; kGroup < bigNum; kGroup++)                              // для записи каждого нового числа в группе
                {
                    bool isOK;                                                          // флаг того, что число может быть записано в текущую группу
                    for (int j = iMask; j <= bigNum; j++)                                // прогоняем все немаскированные числа по каждому числу, которое уже есть в группе
                    {
                        if (mask[j] == 0) continue;
                        
                        isOK = true;                                                
                        
                        for (int k = 0; k < kGroup; k++)
                        {
                            if (!Aliquant(deb_group[k], j))
                            {
                                isOK = false;
                                break;
                            }
                        }
                        
                        if (isOK)
                        {
                            deb_group[kGroup] = j;
                            mask[j] = 0;
                            iMask = j + 1;
                            break;
                        }
                            
                    }
                    if (deb_group[kGroup] == 0) break;
                }
                

                mCount++;
                //PrintGroup(deb_group, mCount);
                //Console.ReadKey();
            }
            TimeSpan wasted = DateTime.Now.Subtract(time);
            Console.WriteLine("Всё закончилось :(");
            Console.WriteLine($"Было сформировано {mCount} групп.");
            Console.WriteLine($"Для этого потребовалось {wasted.TotalSeconds} секунд машинного времени.");
            Console.ReadKey();

        }
    }
}
