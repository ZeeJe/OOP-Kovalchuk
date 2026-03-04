using System;

namespace lab25
{
    // --- ПАТЕРНИ FACTORY METHOD ТА SINGLETON (Блок логування) ---

    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine($"[ConsoleLogger]: {message}");
    }

    public class FileLogger : ILogger
    {
        public void Log(string message) => Console.WriteLine($"[FileLogger (імітація запису у файл)]: {message}");
    }

    public abstract class LoggerFactory
    {
        public abstract ILogger CreateLogger();
    }

    public class ConsoleLoggerFactory : LoggerFactory
    {
        public override ILogger CreateLogger() => new ConsoleLogger();
    }

    public class FileLoggerFactory : LoggerFactory
    {
        public override ILogger CreateLogger() => new FileLogger();
    }

    public class LoggerManager
    {
        private static LoggerManager _instance;
        private LoggerFactory _factory;
        private ILogger _logger;

        private LoggerManager() { }

        public static LoggerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoggerManager();
                }
                return _instance;
            }
        }

        public void Initialize(LoggerFactory factory)
        {
            _factory = factory;
            _logger = _factory.CreateLogger();
        }

        public void LogMessage(string message)
        {
            if (_logger != null)
            {
                _logger.Log(message);
            }
            else
            {
                Console.WriteLine("Помилка: LoggerManager не ініціалізовано фабрикою.");
            }
        }
    }

    // --- ПАТЕРН STRATEGY (Блок обробки даних) ---

    public interface IDataProcessorStrategy
    {
        string Process(string data);
    }

    public class EncryptDataStrategy : IDataProcessorStrategy
    {
        public string Process(string data) => $"***ЗАШИФРОВАНО({data})***";
    }

    public class CompressDataStrategy : IDataProcessorStrategy
    {
        public string Process(string data) => $"[СТИСНУТО({data})]";
    }

    public class DataContext
    {
        private IDataProcessorStrategy _strategy;

        public DataContext(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(IDataProcessorStrategy strategy)
        {
            _strategy = strategy;
        }

        public string ExecuteProcessing(string data)
        {
            return _strategy.Process(data);
        }
    }

    // --- ПАТЕРН OBSERVER (Блок сповіщень) ---

    public class DataPublisher
    {
        // Знак питання запобігає попередженню CS8618 щодо можливого null
        public event Action<string>? DataProcessed;

        public void PublishDataProcessed(string processedData)
        {
            DataProcessed?.Invoke(processedData);
        }
    }

    public class ProcessingLoggerObserver
    {
        public void OnDataProcessed(string data)
        {
            // Використання Singleton для логування події
            LoggerManager.Instance.LogMessage($"Спостерігач отримав оброблені дані: {data}");
        }
    }

    // --- ГОЛОВНИЙ КЛАС ---

    class Program
    {
        static void Main()
        {
            // Налаштування консолі для української мови
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string rawData = "Конфіденційна інформація";

            // --- Сценарій 1: Повна інтеграція ---
            Console.WriteLine("==================================================");
            Console.WriteLine("Сценарій 1: Повна інтеграція");
            Console.WriteLine("==================================================");
            
            // 1. Ініціалізація Singleton через Factory
            LoggerManager.Instance.Initialize(new ConsoleLoggerFactory());
            
            // 2. Ініціалізація Strategy
            DataContext context = new DataContext(new EncryptDataStrategy());
            
            // 3. Ініціалізація Observer
            DataPublisher publisher = new DataPublisher();
            ProcessingLoggerObserver observer = new ProcessingLoggerObserver();
            
            // Підписка
            publisher.DataProcessed += observer.OnDataProcessed;
            
            // Виконання та публікація
            string encryptedData = context.ExecuteProcessing(rawData);
            publisher.PublishDataProcessed(encryptedData);

            // --- Сценарій 2: Динамічна зміна логера ---
            Console.WriteLine("\n==================================================");
            Console.WriteLine("Сценарій 2: Динамічна зміна логера");
            Console.WriteLine("==================================================");
            
            // Зміна фабрики в Singleton (тепер вивід піде через FileLogger)
            LoggerManager.Instance.Initialize(new FileLoggerFactory());
            
            // Публікація тих самих даних для перевірки нового логера
            publisher.PublishDataProcessed(encryptedData);

            // --- Сценарій 3: Динамічна зміна стратегії ---
            Console.WriteLine("\n==================================================");
            Console.WriteLine("Сценарій 3: Динамічна зміна стратегії");
            Console.WriteLine("==================================================");
            
            // Зміна стратегії в Context
            context.SetStrategy(new CompressDataStrategy());
            
            // Виконання за новою стратегією та публікація
            string compressedData = context.ExecuteProcessing(rawData);
            publisher.PublishDataProcessed(compressedData);
            
            Console.WriteLine("\nРоботу завершено.");
        }
    }
}