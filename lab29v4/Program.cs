using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace lab29v4;

class Program
{
    static async Task Main(string[] args)
    {
        // Налаштування кодування для коректного відображення тексту в консолі
        Console.OutputEncoding = Encoding.UTF8;
        
        string inputFile = "users_data.csv";
        string asyncOutputFile = "filtered_users_async.csv";
        string syncOutputFile = "filtered_users_sync.csv";
        
        // 1 мільйон рядків для тестування
        int rowsCount = 1_000_000; 

        Console.WriteLine($"=== Етап 1: Генерація великого файлу ({rowsCount} рядків) ===");
        await GenerateLargeFileAsync(inputFile, rowsCount);
        Console.WriteLine("Генерацію завершено. Файл створено.\n");

        Console.WriteLine("=== Етап 2: Асинхронне читання, статистика та фільтрація ===");
        var stopwatch = Stopwatch.StartNew();
        await ProcessFileAsync(inputFile, asyncOutputFile);
        stopwatch.Stop();
        long asyncTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Асинхронна обробка зайняла: {asyncTime} мс\n");

        Console.WriteLine("=== Етап 3: Синхронне читання (для порівняння) ===");
        stopwatch.Restart();
        ProcessFileSync(inputFile, syncOutputFile);
        stopwatch.Stop();
        long syncTime = stopwatch.ElapsedMilliseconds;
        Console.WriteLine($"Синхронна обробка зайняла: {syncTime} мс\n");

        Console.WriteLine("=== Висновок ===");
        Console.WriteLine($"- Час виконання асинхронного методу: {asyncTime} мс");
        Console.WriteLine($"- Час виконання синхронного методу: {syncTime} мс");
        Console.WriteLine("\nПримітка: На сучасних SSD синхронне читання може бути трохи швидшим " +
                          "через відсутність накладних витрат на перемикання контексту потоків. " +
                          "Проте асинхронне читання НЕ блокує основний потік програми, " +
                          "що критично важливо для веб-серверів та застосунків з інтерфейсом.");
    }

    // 1. Генератор великого файлу
    static async Task GenerateLargeFileAsync(string path, int count)
    {
        var random = new Random();
        // Використовуємо StreamWriter для запису
        await using var writer = new StreamWriter(path, false, Encoding.UTF8);
        await writer.WriteLineAsync("Id,Name,Age"); // Заголовок CSV

        for (int i = 1; i <= count; i++)
        {
            int age = random.Next(10, 81); // Генеруємо вік від 10 до 80
            await writer.WriteLineAsync($"{i},User_{i},{age}");
        }
    }

    // 2. Асинхронний читач, обробка, статистика та запис результату
    static async Task ProcessFileAsync(string inputPath, string outputPath)
    {
        using var reader = new StreamReader(inputPath, Encoding.UTF8);
        await using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);

        // Читаємо і відразу записуємо заголовок
        string? header = await reader.ReadLineAsync();
        if (header != null) await writer.WriteLineAsync(header);

        int totalUsers = 0;
        int adultUsers = 0;
        long totalAge = 0;

        string? line;
        // Порядкове асинхронне читання
        while ((line = await reader.ReadLineAsync()) != null)
        {
            totalUsers++;
            var parts = line.Split(',');
            
            if (parts.Length == 3 && int.TryParse(parts[2], out int age))
            {
                totalAge += age;
                
                // Фільтр: вік > 18
                if (age > 18)
                {
                    adultUsers++;
                    await writer.WriteLineAsync(line);
                }
            }
        }

        double avgAge = totalUsers > 0 ? (double)totalAge / totalUsers : 0;
        Console.WriteLine($"[Async] Загальна кількість рядків: {totalUsers}");
        Console.WriteLine($"[Async] Користувачів старше 18: {adultUsers}");
        Console.WriteLine($"[Async] Середній вік усіх користувачів: {avgAge:F1}");
    }

    // 3. Синхронна обробка для порівняння (ідентична логіка, але без async/await)
    static void ProcessFileSync(string inputPath, string outputPath)
    {
        using var reader = new StreamReader(inputPath, Encoding.UTF8);
        using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);

        string? header = reader.ReadLine();
        if (header != null) writer.WriteLine(header);

        int totalUsers = 0;
        int adultUsers = 0;
        long totalAge = 0;

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            totalUsers++;
            var parts = line.Split(',');
            
            if (parts.Length == 3 && int.TryParse(parts[2], out int age))
            {
                totalAge += age;
                if (age > 18)
                {
                    adultUsers++;
                    writer.WriteLine(line);
                }
            }
        }

        double avgAge = totalUsers > 0 ? (double)totalAge / totalUsers : 0;
        Console.WriteLine($"[Sync] Загальна кількість рядків: {totalUsers}");
        Console.WriteLine($"[Sync] Користувачів старше 18: {adultUsers}");
        Console.WriteLine($"[Sync] Середній вік усіх користувачів: {avgAge:F1}");
    }
}