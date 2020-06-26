using System;
using System.IO;
using System.Security.Authentication;
using System.Text;

namespace euler
{    
    class Program
    {
        // Задача сводится к подсчету простых множителей каждого целого числа от 1 до заданного.
        // Для этого построим массив, индексы которого будут наши числа, а значение элемента - количество простых множителей
        // этого числа. 2 множителя - число простое (1 и само число) - эти числа попадут в группу №2
        // Все остальные числа будут формировать группы с номером, равным количеству их простых множителей (включая 1),
        // т.к. внутри группы они не будут взаимно делиться.

        static string FindNumbers(string str)
        {
            StringBuilder numStr = new StringBuilder();

            char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            int pos = str.IndexOfAny(digits);
            if (pos < 0) return String.Empty;

            do
            {
                numStr.Append(str[pos++]);
            } while (char.IsDigit(str[pos]));

            return Convert.ToString(numStr);
        }

        static byte[] CountByEuler(int N)
        {
            byte[] prFacktNum = new byte[N + 1];                         // массив, содержащий количество простых множителей, включая 1, для каждого числа-инедекса
            prFacktNum[1] = 1;

            for (int i = 2; i < prFacktNum.Length; i++)
            {
                if (prFacktNum[i] == 0) prFacktNum[i] = 2;              //  i - простое число, у него 2 множителя
                
                for (int j = 2; j <= i; j++)
                {
                    if (j * (long)i > N) break;                         // если за пределами отрезка, бросаем это дело (теоретически, здесь возможно переполнение int)
                    if (prFacktNum[j] == 2)                             // если j - простое
                    {
                        prFacktNum[i * j] = Convert.ToByte(prFacktNum[i] + 1); // все кратные ему числа будут иметь + 1 простой множитель
                                                                        
                    }
                }

            }
            return prFacktNum;
        }

        /// <summary>
        /// Подсчитывает количество простых множителей для каждого целого числа от 1 до N используя модифицированное решето Эйлера.
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        static byte[] OptimizedPrimeCount(int N)
        {
            ushort[] lp = new ushort[N + 1];          //массив наименьших делителей, изначально напротив каждого числа (индекса массива) стоит 0
            
            for (int i = 2; i <= N; i++)                                 
            {

                for (int p = 2; p <= i; p++)
                {
                    if (p * (long)i > N) break;                         // если за пределами отрезка, бросаем это дело (теоретически, здесь возможно переполнение int)
                    if (lp[p] == 0)                                     // если p  простое
                    {
                        lp[i * p] = Convert.ToByte(p);                  // все кратные ему числа будут иметь такой минимальный делитель (по идее, p не превысит
                                                                        // корень квадратный из N, т.е. 65535 должно хватить для отрезка до 4 294 836 225, а у нас
                                                                        // к тому времени вылетит за диапазон значений p * i
                                                                        // однако такая экономия памяти примерно в 3 раза замедляет работу алгоритма при N = 1 млрд !
                    }

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
                if (lp[i] == 0)
                    allNums[i] = 2;                                         // это простое число, у него 2 множителя: 1 и оно само
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
                            allNums[i] = Convert.ToByte(1 + allNums[q]);    // если разделилось - плюсуем единичку
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
            int count = Convert.ToByte(Math.Floor(Math.Log2(nums.GetUpperBound(0)))) + 1;

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
            //string myPath = Directory.GetCurrentDirectory();
            Console.WriteLine("Определение на заданном числовом отрезке групп целых чисел, которые не делятся друг на друга\n");
            Console.WriteLine("Текущая папка: " + Directory.GetCurrentDirectory());

            // ******************************************************************************************
            DateTime startTime = new DateTime();
            TimeSpan wasted = new TimeSpan();
            int bigNum; // = 2000000000;                           // длинна числового ряда от 1 до bigNum
            
            do
            {
                Console.Write("Введите имя файла с данными (и путь, если файл находится в другой папке):");
                string path = Console.ReadLine();
                if (!File.Exists(@path))
                {
                    Console.WriteLine("Файл не найден.");
                    break;
                }

                string data = File.ReadAllText(@path);
               
                if (!int.TryParse(FindNumbers(data), out bigNum))
                {
                    Console.WriteLine("Указанный файл содержит некорректные данные.");
                    break;
                }
                if (bigNum > 2_000_000_000)
                {
                    Console.WriteLine($"Верхняя граница {bigNum: ### ### ### ###} слишком велика. Максимальная граница числового отрезка: 2 000 000 000.");
                    break;
                }

                Console.WriteLine($"Данные получены. Задан числовой отрезок от 1 до {bigNum}.");
                Console.Write("Для подсчета количества групп нажмите \"1\", для записи групп в файл нажмите \"2\":");
                ConsoleKeyInfo ch = new ConsoleKeyInfo();
                do
                {
                    ch = Console.ReadKey(true);
                } while (ch.KeyChar != '1' && ch.KeyChar != '2');
               
                Console.WriteLine();
                switch (ch.KeyChar)
                {
                    case '1':
                        startTime = DateTime.Now;
                        Console.WriteLine($"Количество групп подсчитано: {Convert.ToInt32(Math.Floor(Math.Log2(bigNum))) + 1}");
                        wasted = DateTime.Now.Subtract(startTime);
                        Console.WriteLine($"Для посчета потребовалось {wasted.TotalSeconds} секунд машинного времени.");
                        break;

                    case '2':
                        Console.WriteLine("Каким алгоритмом Вы желаете воспользоваться для распределения чисел на группы?");
                        Console.Write("Решето Эратосфена - нажмите \"1\", Решето Эйлера - нажмите \"2\":");
                        ch = Console.ReadKey();
                        Console.WriteLine();
                       

                        if (ch.KeyChar == '1')
                        {
                            Console.WriteLine("Этот метод доступен только в зарегистрированной версии");
                            
                        }
                        else
                        {
                            Console.WriteLine("Производится распределение чисел на группы, это может занять некоторое время...");
                            startTime = DateTime.Now;
                            byte[] group = CountByEuler(bigNum);
                            wasted = DateTime.Now.Subtract(startTime);
                            Console.WriteLine($"Все числа распределены. Для этого потребовалось {wasted.TotalSeconds} секунд машинного времени.");
                            Console.Write($"Введите имя файла для записи и путь. По умолчанию файл будет создан в текущей папке программы: ");
                            path = Console.ReadLine();
                            if (!File.Exists(@path))
                                File.Delete(@path);
                            Console.WriteLine("Идет запись файла, пожалуйста, не отключайте компьютер...");
                            StreamWriter outputFile = new StreamWriter(path);
                            using (outputFile)
                            {
                                for (int j = 0; j < group.Length; j++)
                                {
                                    outputFile.WriteLine($"{j,5}: {group[j],3}");
                                }

                                for (byte i = 1; i <= Convert.ToByte(Math.Floor(Math.Log2(bigNum))) + 1; i++)
                                {
                                    outputFile.Write($"Группа {i}:");
                                    
                                    for (int j = 0; j < group.Length; j++)
                                    {
                                        if (group[j] == i)
                                            outputFile.Write($"\t{j}");
                                    }
                                    outputFile.WriteLine();
                                } 
                            }
                            wasted = DateTime.Now.Subtract(startTime);
                            Console.WriteLine($"Запись завершена. На всю процедуру потребовалось {wasted.TotalSeconds} секунд машинного времени.");

                        }

                        break;

                }

                break;
            } while (true);

            Console.ReadKey();






            //DateTime time = DateTime.Now;

            //Console.WriteLine($"Заданный числовой отрезок: от 1 до {bigNum : ### ### ### ###}.\n");

            //// ******************************************************************************************
            ////byte[] groups = OptimizedPrimeCount(bigNum);
            //byte[] groups = SuperFastPrimeCount(bigNum);

            ////PrintAllGroups(groups);

            ////PrintGroup(test, 0);

            //TimeSpan wasted = DateTime.Now.Subtract(time);
            //Console.WriteLine("Всё закончилось :(");
            //Console.WriteLine($"Было сформировано {Convert.ToInt32(Math.Floor(Math.Log2(bigNum))) + 1} групп.");
            //Console.WriteLine($"Для этого потребовалось {wasted.TotalSeconds} секунд машинного времени.");
            //Console.ReadKey();

        }
    }
}
