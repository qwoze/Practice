using System;
using System.Collections.Generic;
using System.Threading;

namespace practika1
{
    class Program
    {
        static int[] Numbers; // Массив для хранения чисел
        static int ThreadCount = 5; // Количество потоков по умолчанию
        static List<int> SquareNumbers = new List<int>(); // Лист для хранения квадратных чисел

        static void Main(string[] args)
        {
            ReadInput(); // Чтение входных данных

            if (args.Length > 0)
            {
                int.TryParse(args[0], out ThreadCount); // Чтение количества потоков из аргументов командной строки
                if (ThreadCount < 2 || ThreadCount > 20)
                {
                    Console.WriteLine("Указано некорректное число потоков");
                    return;
                }
            }

            Thread[] threads = new Thread[ThreadCount]; // Создание массива потоков

            // Разделение работы между потоками
            int range = Numbers.Length / ThreadCount;

            for (int i = 0; i < ThreadCount; i++)
            {
                int startIndex = i * range;
                int endIndex = (i == ThreadCount - 1) ? Numbers.Length : (i + 1) * range;
                threads[i] = new Thread(() => FindSquareNumbers(startIndex, endIndex));
                threads[i].Start();
            }

            // Ожидание завершения всех потоков
            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine("Окончательный результат:");

            foreach (int number in SquareNumbers)
            {
                Console.WriteLine(number);
            }
        }

        static void ReadInput()
        {
            Console.WriteLine("Введите числа, разделенные пробелами:");
            string input = Console.ReadLine();
            string[] numbers = input.Split(' ');
            Numbers = new int[numbers.Length];

            for (int i = 0; i < numbers.Length; i++)
            {
                if (!int.TryParse(numbers[i], out Numbers[i]))
                {
                    Console.WriteLine("Входная строка имеет неверный формат");
                    Environment.Exit(0);
                }
            }
        }

        static void FindSquareNumbers(int startIndex, int endIndex)
        {
            List<int> squareNumbers = new List<int>();

            for (int i = startIndex; i < endIndex; i++)
            {
                int squareRoot = (int)Math.Sqrt(Numbers[i]);

                if (squareRoot * squareRoot == Numbers[i])
                {
                    squareNumbers.Add(Numbers[i]);
                }
            }

            lock (SquareNumbers) // Блокируем доступ к SquareNumbers для синхронизации добавления элементов
            {
                SquareNumbers.AddRange(squareNumbers); // Добавляем найденные квадратные числа в общий список
            }

            // Отображение промежуточных результатов
            Console.WriteLine("Промежуточный результат:");
            foreach (int number in squareNumbers)
            {
                Console.WriteLine(number);
            }
        }
    }
}