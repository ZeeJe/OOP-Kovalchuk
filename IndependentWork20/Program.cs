using System;

namespace IndependentWork20
{
    // ==========================================
    // ПАТЕРН STRATEGY (Стратегії форматування)
    // ==========================================

    // Інтерфейс для стратегій обробки даних
    public interface IDataProcessorStrategy
    {
        void Process(string data);
    }

    // Реалізація 1: Форматування в HTML
    public class HtmlFormatStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[Strategy] Результат HTML: <h1>{data}</h1>");
        }
    }

    // Реалізація 2: Форматування в Markdown
    public class MarkdownFormatStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[Strategy] Результат Markdown: # {data}");
        }
    }

    // Реалізація 3: Форматування в Plain Text (простий текст)
    public class PlainTextFormatStrategy : IDataProcessorStrategy
    {
        public void Process(string data)
        {
            Console.WriteLine($"[Strategy] Результат Plain Text: {data}");
        }
    }

    // Контекст, який використовує обрану стратегію
    public class DataContext
    {
        private IDataProcessorStrategy _strategy;

        // Конструктор приймає початкову стратегію
        public DataContext(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        // Метод для динамічної зміни стратегії під час виконання
        public void SetStrategy(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        // Делегування виконання поточної стратегії
        public void ExecuteProcessing(string data)
        {
            _strategy.Process(data);
        }
    }

    // ==========================================
    // ПАТЕРН OBSERVER (Спостерігачі та Події)
    // ==========================================

    // Суб'єкт (Джерело подій), що сповіщає про готовність звіту
    public class DataPublisher
    {
        // Подія С# для реалізації патерну Observer
        public event Action<string>? DataProcessed;

        // Метод для викликання події та сповіщення підписників
        public void PublishDataProcessed(string data)
        {
            Console.WriteLine($"\n[Publisher] Надсилання сповіщення підписникам про звіт: '{data}'");
            // Викликаємо подію (якщо є хоча б один підписник)
            DataProcessed?.Invoke(data);
        }
    }

    // Спостерігач 1: Вивід інформації на екран
    public class ConsoleOutputObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"[ConsoleOutputObserver] Отримано сигнал! Документ '{data}' успішно виведено на монітор.");
        }
    }

    // Спостерігач 2: Збереження інформації у файл
    public class FileSaverObserver
    {
        public void OnDataProcessed(string data)
        {
            Console.WriteLine($"[FileSaverObserver] Отримано сигнал! Імітація збереження файлу для звіту '{data}'.");
        }
    }

    // ==========================================
    // ДЕМОНСТРАЦІЯ РОБОТИ (Метод Main)
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== Демонстрація роботи програми (Варіант 4) ===");

            // 1. Створюємо екземпляри контексту (зі стартовою стратегією) та видавця подій
            DataContext context = new DataContext(new PlainTextFormatStrategy());
            DataPublisher publisher = new DataPublisher();

            // 2. Створюємо спостерігачів
            ConsoleOutputObserver consoleObserver = new ConsoleOutputObserver();
            FileSaverObserver fileObserver = new FileSaverObserver();

            // Підписуємо спостерігачів на подію (Pattern Observer)
            publisher.DataProcessed += consoleObserver.OnDataProcessed;
            publisher.DataProcessed += fileObserver.OnDataProcessed;

            string reportData = "Річний звіт з успішності студентів ООП";

            // 3. Тестуємо першу стратегію (Plain Text)
            Console.WriteLine("\n--- Крок 1: Текстовий формат ---");
            context.ExecuteProcessing(reportData);
            publisher.PublishDataProcessed("Звіт_ПростийТекст");

            // 4. Змінюємо стратегію на HTML
            Console.WriteLine("\n--- Крок 2: Зміна стратегії на HTML ---");
            context.SetStrategy(new HtmlFormatStrategy());
            context.ExecuteProcessing(reportData);
            publisher.PublishDataProcessed("Звіт_HTML");

            // 5. Змінюємо стратегію на Markdown
            Console.WriteLine("\n--- Крок 3: Зміна стратегії на Markdown ---");
            context.SetStrategy(new MarkdownFormatStrategy());
            context.ExecuteProcessing(reportData);
            publisher.PublishDataProcessed("Звіт_Markdown");

            Console.ReadLine();
        }
    }
}