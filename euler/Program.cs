using System;
using System.IO;
using System.IO.Compression;
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


        /// <summary>
        /// Находит в строке первые попавшиеся цифры, идущие подряд, и формирует из них подстроку.
        /// </summary>
        /// <param name="str">строка с цифрами</param>
        /// <returns>подстрока, состоящая только из цифр, или пустая строка</returns>
        static string FindNumbers(string str)
        {
            StringBuilder numStr = new StringBuilder();

            char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            int pos = str.IndexOfAny(digits);
            if (pos < 0) return String.Empty;

            do
            {
                numStr.Append(str[pos]);
            } while (pos < str.Length - 1 && char.IsDigit(str[++pos]));

            return Convert.ToString(numStr);
        }
        
        /// <summary>
        /// Выводит приглашение ввести один из двух симоволов и получает их с клавиатуры.
        /// </summary>
        /// <param name="msg">текст приглашения</param>
        /// <param name="a">первая опция</param>
        /// <param name="b">вторая опция</param>
        /// <returns>один из двух допустимых символов, который ввел пользователь - в верхнем регистре</returns>
        static char Choice(string msg, char a, char b)
        {
            Console.Write(msg);
            char ch; 
            do
            {
                ch = char.ToUpper(Console.ReadKey(true).KeyChar);
            } while (ch != char.ToUpper(a) && ch != char.ToUpper(b));
            Console.WriteLine();
            return ch;
        }   

        /// <summary>
        /// Подсчитывает количество простых множителей для каждого целого числа от 1 до N используя модифицированное решето Эйлера.
        /// </summary>
        /// <param name="N"></param>
        /// <returns>Массив, содержащий количество простых множителей (включая 1) для каждого значения индекса</returns>
        static byte[] CountByEuler(int N)
        {
            byte[] prNumber = new byte[N + 1];                              // массив, содержащий количество простых множителей, включая 1, для каждого числа-инедекса
            prNumber[1] = 1;

            for (int i = 2; i < prNumber.Length; i++)
            {
                if (prNumber[i] == 0) prNumber[i] = 2;                      //  i - простое число, у него 2 множителя
                
                for (int j = 2; j <= i; j++)
                {
                    if (j * (long)i > N) break;                             // приведение к long т.к. здесь возможно переполнение int и => некорректное сравнение c N 
                    if (prNumber[j] == 2)                             
                    {
                        prNumber[i * j] = Convert.ToByte(prNumber[i] + 1);  // все кратные ему числа будут иметь + 1 простой множитель.  
                    }
                }

            }
            return prNumber;
        }

        /// <summary>
        /// Подсчитывает количество простых множителей для каждого целого числа от 1 до N используя решето Эратосфена. 
        /// Быстрее Эйлера, но требует в два раза больше памяти, т.к. использует один и тот же массив и для записи
        /// наименьшего делителя числа, и для записи общего количества простых множителей (номера группы).
        /// </summary>
        /// <param name="N"></param>
        /// <returns>Массив, содержащий количество простых множителей (включая 1) для каждого значения индекса</returns>
        static ushort[] CountByEratosfen(int N)
        {
            ushort[] prNumber = new ushort[N + 1];                              // в целом, тот же подход, что  и выше
            prNumber[1] = 1;
            int q;

            for (int i = 2; i < prNumber.Length; i++)
            {
                if (prNumber[i] == 0)
                {
                    for (int j = i * 2; j < prNumber.Length; j += i)
                        if (prNumber[j] == 0)
                            prNumber[j] = Convert.ToUInt16(i);                  // предполагается, что в этом месте i никогда не превысит корня квадратного из N
                    prNumber[i] = 2;
                }
                else
                {
                    q = i / prNumber[i];
                    prNumber[i] = Convert.ToUInt16(prNumber[q] + 1);
                }

            }
            return prNumber;
        }

        /// <summary>
        /// Неспешно подсчитывает количество простых множителей для каждого целого числа от 1 до N
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
                for (int j = 2; j <= (int)Math.Floor(Math.Sqrt(i)); j++)    //нет смысла искать простые множители числа, большие его квадратного корня
                {
                    if (allNums[j] == 2)                                    //проверяем делимость только на простые числа!
                    {

                        if (i % j == 0) // можно оптимизировать иф
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
        /// Программа нахождения групп некратных друг другу чисел в отрезке числового ряда
        /// от 1 до заданного целого числа.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Определение на заданном числовом отрезке групп целых чисел, которые не делятся друг на друга\n");
            
            DateTime startTime;
            TimeSpan wasted;
            int bigNum;                                             // верхняя граница
            int maxNum = 1_000_000_000;                             // предельное значение
            string report = "log.txt";                              // лог-файл
            string output = "output.txt";                           // текстовый вывод
            string compressed = "output.gzip";                      // сжатый файл   
            string datatxt = "data.txt";                            // файл с данными по умолчанию

            string defPath = Directory.GetCurrentDirectory() + "\\" + datatxt;
            Console.WriteLine("Файл данных по умолчанию: " + defPath);
            do
            {
                Console.Write("Введите новое имя файла с данными и путь (Enter - оставить значение по умолчанию):");
                string path = Console.ReadLine();
                if (String.IsNullOrEmpty(path))
                    path = defPath;
                
                if (!File.Exists(@path))
                {
                    Console.WriteLine("Файл не найден.");
                    break;
                }

                string data = File.ReadAllText(@path);
               
                if (!int.TryParse(FindNumbers(data), out bigNum) || bigNum <= 0)
                {
                    Console.WriteLine("Указанный файл содержит некорректные данные.");
                    break;
                }
                if (bigNum > maxNum)
                {
                    Console.WriteLine($"Верхняя граница {bigNum: ### ### ### ###} слишком велика. Максимальная граница числового отрезка: {maxNum : ### ### ### ###}.");
                    break;
                }

                Console.WriteLine($"Данные получены. Задан числовой отрезок от 1 до {bigNum : ### ### ### ###}.");

                if (Choice("Для подсчета количества групп нажмите \"1\", для записи групп в файл нажмите \"2\":", '1', '2') == '1')
                {
                    startTime = DateTime.Now;
                    Console.WriteLine($"Количество возможных групп: {Convert.ToByte(Math.Floor(Math.Log2(bigNum))) + 1}");
                    wasted = DateTime.Now.Subtract(startTime);
                    Console.WriteLine($"Для подсчета потребовалось {wasted.TotalMilliseconds} милисекунд.");
                    break;
                }
                else
                {
                    Console.WriteLine($"Будет создан файл {Directory.GetCurrentDirectory()}\\output.txt");
                    if (Choice("Предыдущие результаты будут перезаписаны! Продолжить? (y/n)", 'y', 'n') == 'N')
                        break;
                }

                Console.WriteLine("Производится распределение чисел на группы, это может занять некоторое время...");
                startTime = DateTime.Now;
                //var group = CountByEuler(bigNum);                                                           // с решетом Эйлера (медленнее, меньше памяти, дольше пишет файл)                             
                var group = CountByEratosfen(bigNum);                                                       // с решетом Эратосфена
               
                wasted = DateTime.Now.Subtract(startTime);
                Console.WriteLine($"Все числа распределены. Для этого потребовалось {wasted.TotalSeconds} секунд.");
                
                Console.WriteLine("Идет запись файла, пожалуйста, не отключайте компьютер...");
                startTime = DateTime.Now;
                
                int count, megaCount = 0;                                       // счетчики чисел для отчета
                
                using (StreamWriter logFile = new StreamWriter(report, false), outputFile = new StreamWriter(output, false))
                {
                    logFile.WriteLine($"Время работы алгоритма: {wasted.TotalSeconds} секунд.");
                    
                    for (byte i = 1; i <= Convert.ToByte(Math.Floor(Math.Log2(bigNum))) + 1; i++)
                    {
                        outputFile.Write($"Группа {i}:");
                        logFile.Write($"Группа {i}:");
                        count = 0;

                        for (int j = 0; j < group.Length; j++)
                        {
                            if (group[j] == i)
                            {
                                outputFile.Write($"\t{j}");
                                count++;
                            }
                        }
                            
                        outputFile.WriteLine();
                        logFile.WriteLine($" всего \t{count, 11 : ### ### ###} чисел");
                        megaCount += count;
                    }
                    wasted = DateTime.Now.Subtract(startTime);
                    logFile.WriteLine($"Всего обработано \t{megaCount: ### ### ### ###} чисел.\nВремя на запись файла: {wasted.TotalSeconds} секунд");
                }

               
                var fileStat = new FileInfo(output);
                Console.WriteLine($"Запись завершена.  Размер файла: {Math.Ceiling(fileStat.Length / 1024f)} KB, для записи потребовалось {wasted.TotalSeconds} секунд.");
                Console.WriteLine("В текущей папке сформирован краткий отчет " + report);
                
                if (Choice("Заархивировать файл с данными? (y/n)", 'y', 'n') == 'N')
                    break;

                Console.WriteLine("Производится сжатие...");
                using (FileStream source = new FileStream(output, FileMode.Open), zipped = File.Create(compressed))
                {
                    using (GZipStream pipe = new GZipStream(zipped, CompressionMode.Compress))
                    {
                        source.CopyTo(pipe); 
                        Console.WriteLine("Сжатие файла {0} завершено. Было: {1} KB стало: {2} KB.",
                                            output,
                                            Math.Ceiling(source.Length / 1024f),
                                            Math.Ceiling(zipped.Length / 1024f));
                    }
                }

                break;
            } while (true);
            Console.Write("Нажмите любую клавишу...");
            Console.ReadKey();


        }
    }
}
