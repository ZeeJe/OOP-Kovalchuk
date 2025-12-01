using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Lab7vN
{
    // Клас для роботи з файлами
    class FileProcessor
    {
        private int failCount = 0;

        public byte[] ReadImage(string path)
        {
            failCount++;
            if (failCount <= 3) // Імітуємо помилку FileNotFoundException 3 рази
            {
                Console.WriteLine($"[FileProcessor] Спроба {failCount}: Файл не знайдено.");
                throw new FileNotFoundException("Файл не знайдено: " + path);
            }

            Console.WriteLine("[FileProcessor] Файл успішно зчитано.");
            return new byte[] { 1, 2, 3 }; // Успішне зчитування (імітація)
        }
    }

    // Клас для мережевих запитів
    class NetworkClient
    {
        private int failCount = 0;

        public byte[] DownloadImage(string url)
        {
            failCount++;
            if (failCount <= 2) // Імітуємо помилку HttpRequestException 2 рази
            {
                Console.WriteLine($"[NetworkClient] Спроба {failCount}: Помилка HTTP-запиту.");
                throw new HttpRequestException("Помилка при завантаженні з " + url);
            }

            Console.WriteLine("[NetworkClient] Завантаження успішне.");
            return new byte[] { 4, 5, 6 }; // Успішне завантаження (імітація)
        }
    }

    // Узагальнений допоміжний клас для Retry
    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool> shouldRetry = null)
        {
            if (initialDelay == default)
                initialDelay = TimeSpan.FromSeconds(1);

            int attempt = 0;
            while (true)
            {
                try
                {
                    attempt++;
                    return operation();
                }
                catch (Exception ex)
                {
                    bool retry = shouldRetry?.Invoke(ex) ?? true;

                    if (!retry || attempt >= retryCount)
                    {
                        Console.WriteLine($"[RetryHelper] Операція завершилася невдачею: {ex.Message}");
                        throw;
                    }

                    TimeSpan delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                    Console.WriteLine($"[RetryHelper] Спроба {attempt} не вдалася: {ex.Message}. Повтор через {delay.TotalSeconds:F1} сек...");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var fileProcessor = new FileProcessor();
            var networkClient = new NetworkClient();

            Console.WriteLine("=== Демонстрація FileProcessor з Retry ===");
            try
            {
                byte[] fileData = RetryHelper.ExecuteWithRetry(
                    () => fileProcessor.ReadImage("image.jpg"),
                    retryCount: 5,
                    initialDelay: TimeSpan.FromSeconds(1),
                    shouldRetry: ex => ex is FileNotFoundException
                );
                Console.WriteLine($"Файл зчитано, довжина даних: {fileData.Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не вдалося зчитати файл: {ex.Message}");
            }

            Console.WriteLine("\n=== Демонстрація NetworkClient з Retry ===");
            try
            {
                byte[] imageData = RetryHelper.ExecuteWithRetry(
                    () => networkClient.DownloadImage("https://example.com/image.jpg"),
                    retryCount: 4,
                    initialDelay: TimeSpan.FromSeconds(1),
                    shouldRetry: ex => ex is HttpRequestException
                );
                Console.WriteLine($"Завантажено з мережі, довжина даних: {imageData.Length}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не вдалося завантажити зображення: {ex.Message}");
            }

            Console.WriteLine("\n=== Демонстрація завершена ===");
        }
    }
}
