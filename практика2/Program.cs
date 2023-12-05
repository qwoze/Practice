using System;
using System.Threading;

namespace practika2
{
    class Program
    {
        static void Main(string[] args)
        {
            GalleryVisitor[] visitors = new GalleryVisitor[100];

            for (int i = 0; i < visitors.Length; i++)
            {
                visitors[i] = new GalleryVisitor($"Посетитель {i + 1}");
                ThreadPool.QueueUserWorkItem(state =>
                {
                    GalleryVisitor visitor = (GalleryVisitor)state;
                    visitor.VisitGallery();
                }, visitors[i]);
            }

            Console.ReadLine();
        }
    }

    class GalleryVisitor
    {
        private static Semaphore semaphore = new Semaphore(10, 10);
        private static readonly object lockObject = new object();
        private static int currentVisitors = 0;

        private string name;

        public GalleryVisitor(string name)
        {
            this.name = name;
        }

        public void VisitGallery()
        {
            Console.WriteLine($"{name} зашел в галерею.");

            lock (lockObject)
            {
                currentVisitors++;
                Console.WriteLine($"Текущее количество посетителей: {currentVisitors}");
            }

            for (int i = 0; i < 5; i++)
            {
                semaphore.WaitOne();
                Console.WriteLine($"{name} смотрит картину {i + 1}.");

                // Имитация времени просмотра картин
                Thread.Sleep(2000);

                if (new Random().NextDouble() <= 0.2)
                {
                    Console.WriteLine($"{name} покинул галерею.");
                    break;
                }

                semaphore.Release();
            }

            lock (lockObject)
            {
                currentVisitors--;
                Console.WriteLine($"{name} вышел из галереи.");
                Console.WriteLine($"Текущее количество посетителей: {currentVisitors}");
            }
        }
    }
}