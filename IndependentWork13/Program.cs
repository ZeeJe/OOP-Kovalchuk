using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
    // =========================================================================
    // СЦЕНАРІЙ 1: API Timeout та Retry (PolicyWrap)
    // =========================================================================
    
    private static int _scenario1Attempts = 0;

    /// <summary>
    /// Імітація зовнішнього API, який тимчасово повільний і помилковий.
    /// </summary>
    public static string CallExternalApiWithDelay()
    {
        _scenario1Attempts++;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спроба {_scenario1Attempts}: Виклик зовнішнього API...");

        if (_scenario1Attempts == 1)
        {
            // Спроба 1: Імітуємо затримку + помилку
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]   - Імітація надмірної затримки (4с) + помилка.");
            Thread.Sleep(4000); 
            // Викидаємо помилку, щоб Retry Policy її перехопила.
            throw new HttpRequestException("Timeout/Service Failure on attempt 1."); 
        }
        else if (_scenario1Attempts == 2)
        {
            // Спроба 2: Імітуємо тимчасову помилку HTTP
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}]   - Імітація помилки HTTP 500...");
            throw new HttpRequestException("HTTP 500: Internal Server Error - Сервер тимчасово недоступний.");
        }

        // Спроба 3: Успіх
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] API успішно повернуло дані!");
        return "Scenario 1: Дані успішно отримано";
    }

    public static void Scenario1_ApiTimeoutAndRetry()
    {
        Console.WriteLine("\n==================================================");
        Console.WriteLine("--- 1. Сценарій: API Timeout та Retry (PolicyWrap) ---");
        Console.WriteLine("==================================================");

        // --- ЗВІТ / ОПИС ПРОБЛЕМИ ---
        // Проблема: Зовнішній API може бути повільним (Timeout) або повертати тимчасові помилки (HTTP 5xx).
        
        // --- ЗВІТ / ОБҐРУНТУВАННЯ ВИБОРУ ПОЛІТИКИ ---
        // Вибір: PolicyWrap, що містить Timeout Policy (внутрішня) та Retry Policy (зовнішня).

        // 1. Внутрішня політика: Timeout (2 секунди)
        var syncTimeoutPolicy = Policy.Timeout(2, TimeoutStrategy.Optimistic,
            onTimeout: (context, timespan, task) =>
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Таймаут: Операція перевищила {timespan.TotalSeconds}с. Час вичерпано!");
            });

        // 2. Зовнішня політика: Retry з експоненційною затримкою
        var retryPolicy = Policy
            .Handle<HttpRequestException>()     // Обробляємо HTTP помилки
            .Or<TimeoutRejectedException>()     // Обробляємо помилки таймауту
            .WaitAndRetry(3, // Максимум 3 повторні спроби
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Затримки: 2, 4, 8 секунд
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    // Логування повторної спроби
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Retry {retryCount} через {timeSpan.TotalSeconds}с. Причина: {exception.GetType().Name} - {exception.Message.Split('\n')[0]}");
                });

        // Комбінуємо політики (Timeout всередині Retry)
        var policyWrap = Policy.Wrap(retryPolicy, syncTimeoutPolicy); 

        try
        {
            string result = policyWrap.Execute(() => CallExternalApiWithDelay());
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Фінальний Результат: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Операція завершилася невдачею після всіх спроб: {ex.Message.Split('\n')[0]}");
        }
        Console.WriteLine("--------------------------------------------------");
    }


    // =========================================================================
    // СЦЕНАРІЙ 2: Доступ до Бази Даних з Моніторингом Стану (Circuit Breaker)
    // =========================================================================

    private static int _scenario2Attempts = 0;
    private static readonly TimeSpan BreakDuration = TimeSpan.FromSeconds(10); 

    // Створення єдиного екземпляру CircuitBreaker (інакше стан губиться між викликами)
    private static readonly CircuitBreakerPolicy CircuitBreakerPolicy = Policy
        .Handle<InvalidOperationException>()
        .CircuitBreaker(
            exceptionsAllowedBeforeBreaking: 3,      // Відкрити ланцюг після 3-х послідовних помилок
            durationOfBreak: BreakDuration,          // Використовуємо винесену змінну
            onBreak: (exception, breakDelay) => 
            {
                // Логування переходу в Open State
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] CIRCUIT OPEN: Ланцюг розірвано на {breakDelay.TotalSeconds}с. Причина: {exception.Message.Split('\n')[0]}");
            },
            onReset: () => 
            {
                // Логування переходу в Closed State
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] CIRCUIT CLOSED: Ланцюг відновлено (Reset)!");
            },
            onHalfOpen: () => 
            {
                // Логування переходу в Half-Open State
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] CIRCUIT HALF-OPEN: Пробна спроба...");
            }
        );

    /// <summary>
    /// Імітація доступу до БД, яка має тривалий збій.
    /// </summary>
    public static string AccessDatabase()
    {
        _scenario2Attempts++;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спроба {_scenario2Attempts}: Доступ до БД...");

        // Імітуємо 3 послідовні збої (щоб розірвати ланцюг)
        if (_scenario2Attempts <= 3) 
        {
            throw new InvalidOperationException("DB connection pool is exhausted - Пул з'єднань БД вичерпано.");
        }

        // 4-а спроба (пробна) та всі наступні будуть успішними, викликаючи Reset
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] БД успішно повернула дані!");
        return "Scenario 2: Дані БД отримано";
    }

    public static void Scenario2_DatabaseCircuitBreaker()
    {
        Console.WriteLine("\n==================================================");
        Console.WriteLine("--- 2. Сценарій: Доступ до БД з Circuit Breaker ---");
        Console.WriteLine("==================================================");

        // --- ЗВІТ / ОПИС ПРОБЛЕМИ ---
        // Проблема: Тривалий збій бази даних. 
        
        // --- ЗВІТ / ОБҐРУНТУВАННЯ ВИБОРУ ПОЛІТИКИ ---
        // Вибір: Circuit Breaker Policy.

        // Виконання 6 послідовних викликів для демонстрації
        for (int i = 1; i <= 6; i++)
        {
            try
            {
                string result = CircuitBreakerPolicy.Execute(() => AccessDatabase());
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Фінальний Результат: {result}");
            }
            catch (BrokenCircuitException ex)
            {
                // Логування, коли CB активно відхиляє виклик
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] BROKEN CIRCUIT: Виклик відхилено CB. Стан: {CircuitBreakerPolicy.CircuitState}");
            }
            catch (Exception ex)
            {
                // Логування, коли сталася помилка, що призвела до відкриття CB
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Помилка: {ex.Message.Split('\n')[0]}. Стан CB: {CircuitBreakerPolicy.CircuitState}");
            }

            Thread.Sleep(500); 
        }
        
        // --- Демонстрація переходу в Half-Open та Reset ---
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Очікування {BreakDuration.TotalSeconds}с для переходу в Half-Open...");
        Thread.Sleep(BreakDuration); 
        
        // Пробна спроба після очікування
        try
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Пробна спроба після очікування:");
            string result = CircuitBreakerPolicy.Execute(() => AccessDatabase());
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Фінальний Результат: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Помилка: {ex.Message.Split('\n')[0]}.");
        }
        Console.WriteLine("--------------------------------------------------");
    }


    // =========================================================================
    // MAIN METHOD
    // =========================================================================

    public static void Main(string[] args)
    {
        Scenario1_ApiTimeoutAndRetry();
        
        Console.WriteLine("\n\n##################################################\n");

        Scenario2_DatabaseCircuitBreaker();
    }
}