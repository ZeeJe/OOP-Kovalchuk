using System.Diagnostics;
using System.Threading;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        // Експерименти з продуктивності
        var data1M = GenerateData(1_000_000);
        ComparePerformance(data1M);

        var data5M = GenerateData(5_000_000);
        ComparePerformance(data5M);

        var data10M = GenerateData(10_000_000);
        ComparePerformance(data10M);

        // Дослідження проблем безпеки
        SecurityStudy();
    }
    
    // Генерує велику колекцію випадкових чисел
    public static List<int> GenerateData(int size)
    {
        Console.WriteLine($"\nГенерація {size:N0} елементів...");
        var data = new List<int>(size);
        var random = new Random();
        for (int i = 0; i < size; i++)
        {
            data.Add(random.Next(10000, 500000)); 
        }
        return data;
    }

    // Обчислювально інтенсивна операція: перевірка на просте число
    public static bool IsPrime(int number)
    {
        if (number <= 1) return false;
        if (number <= 3) return true;
        if (number % 2 == 0 || number % 3 == 0) return false;
        
        for (int i = 5; i * i <= number; i = i + 6)
        {
            if (number % i == 0 || number % (i + 2) == 0)
                return false;
        }
        return true;
    }
    
    // Вимірює час виконання LINQ/PLINQ запиту
    public static TimeSpan MeasureTime<T>(string name, Func<IEnumerable<T>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        var results = operation().ToList(); // Примушує виконати запит
        stopwatch.Stop();

        Console.WriteLine($"[{name}]: Знайдено {results.Count} результатів. Час: {stopwatch.ElapsedMilliseconds} мс.");
        return stopwatch.Elapsed;
    }

    // Порівнює послідовне (LINQ) та паралельне (PLINQ) виконання
    public static void ComparePerformance(List<int> data)
    {
        Console.WriteLine($"\n--- Порівняння продуктивності для {data.Count:N0} елементів ---");

        var linqTime = MeasureTime("LINQ (Послідовно)", () => data.Where(IsPrime));

        // Ключова відмінність: .AsParallel()
        var plinqTime = MeasureTime("PLINQ (Паралельно)", () => data.AsParallel().Where(IsPrime));

        double speedup = linqTime.TotalMilliseconds / plinqTime.TotalMilliseconds;
        Console.WriteLine($"Прискорення PLINQ: x{speedup:F2}");
    }
    
    // Демонструє проблему побічних ефектів та її виправлення
    public static void SecurityStudy()
    {
        Console.WriteLine("\n\n--- Дослідження проблем безпеки PLINQ ---");

        var data = GenerateData(100_000); 
        int sharedCounter = 0;
        
        int expectedCount = data.Count(IsPrime);
        Console.WriteLine($"\nОчікувана кількість простих чисел (LINQ): {expectedCount}");

        // Сценарій 1: PLINQ з побічним ефектом (УМОВА ГОНКИ)
        Console.WriteLine("\nСценарій 1: PLINQ БЕЗ синхронізації (Некоректно)");
        sharedCounter = 0;
        data.AsParallel().ForAll(num => 
        {
            if (IsPrime(num))
            {
                sharedCounter++; // Не потокобезпечна операція
            }
        });
        Console.WriteLine($"Результат: {sharedCounter}");
        
        // Сценарій 2: Виправлення за допомогою lock
        Console.WriteLine("\nСценарій 2: Виправлення за допомогою lock");
        sharedCounter = 0;
        object lockObject = new object();

        data.AsParallel().ForAll(num =>
        {
            if (IsPrime(num))
            {
                lock (lockObject)
                {
                    sharedCounter++;
                }
            }
        });
        Console.WriteLine($"Результат: {sharedCounter}");
        
        // Сценарій 3: Виправлення за допомогою Interlocked
        Console.WriteLine("\nСценарій 3: Виправлення за допомогою Interlocked.Increment");
        sharedCounter = 0;
        data.AsParallel().ForAll(num =>
        {
            if (IsPrime(num))
            {
                Interlocked.Increment(ref sharedCounter); // Атомарна операція
            }
        });
        Console.WriteLine($"Результат: {sharedCounter}");
    }
}