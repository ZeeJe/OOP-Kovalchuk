using System;
using System.Collections.Generic;

namespace lab24
{
    // --- ПАТЕРН STRATEGY ---

    public interface INumericOperationStrategy
    {
        double Execute(double value);
    }

    public class SquareOperationStrategy : INumericOperationStrategy
    {
        public double Execute(double value) => value * value;
    }

    public class CubeOperationStrategy : INumericOperationStrategy
    {
        public double Execute(double value) => Math.Pow(value, 3);
    }

    public class SquareRootOperationStrategy : INumericOperationStrategy
    {
        public double Execute(double value) => Math.Sqrt(value);
    }

    public class NumericProcessor
    {
        private INumericOperationStrategy _strategy;

        public NumericProcessor(INumericOperationStrategy strategy)
        {
            _strategy = strategy;
        }

        public void SetStrategy(INumericOperationStrategy strategy)
        {
            _strategy = strategy;
        }

        public double Process(double input)
        {
            return _strategy.Execute(input);
        }
    }

    // --- ПАТЕРН OBSERVER ---

    public class ResultPublisher
    {
        // Подія, на яку будуть підписуватися спостерігачі
        public event Action<double, string> ResultCalculated;

        public void PublishResult(double result, string operationName)
        {
            ResultCalculated?.Invoke(result, operationName);
        }
    }

    public class ConsoleLoggerObserver
    {
        public void Log(double result, string operationName)
        {
            Console.WriteLine($"[ConsoleLogger]: Операція '{operationName}' виконана. Результат: {result}");
        }
    }

    public class HistoryLoggerObserver
    {
        public List<string> History { get; private set; } = new List<string>();

        public void AddToHistory(double result, string operationName)
        {
            string entry = $"Операція: {operationName} | Результат: {result}";
            History.Add(entry);
        }

        public void PrintHistory()
        {
            Console.WriteLine("\n--- Історія операцій ---");
            foreach (var item in History)
            {
                Console.WriteLine(item);
            }
        }
    }

    public class ThresholdNotifierObserver
    {
        private readonly double _threshold;

        public ThresholdNotifierObserver(double threshold)
        {
            _threshold = threshold;
        }

        public void CheckThreshold(double result, string operationName)
        {
            if (result > _threshold)
            {
                Console.WriteLine($"[ThresholdNotifier]: УВАГА! Результат операції '{operationName}' ({result}) перевищує поріг ({_threshold}).");
            }
        }
    }

    // --- ГОЛОВНИЙ КЛАС ---

    class Program
    {
        static void Main()
        {
            // Налаштування консолі для коректного відображення кирилиці
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("--- Демонстрація роботи Strategy та Observer ---\n");

            // 1. Ініціалізація видавця (Subject)
            ResultPublisher publisher = new ResultPublisher();

            // 2. Ініціалізація спостерігачів (Observers)
            ConsoleLoggerObserver consoleLogger = new ConsoleLoggerObserver();
            HistoryLoggerObserver historyLogger = new HistoryLoggerObserver();
            ThresholdNotifierObserver thresholdNotifier = new ThresholdNotifierObserver(50.0); // Поріг 50

            // 3. Підписка спостерігачів на подію
            publisher.ResultCalculated += consoleLogger.Log;
            publisher.ResultCalculated += historyLogger.AddToHistory;
            publisher.ResultCalculated += thresholdNotifier.CheckThreshold;

            // 4. Ініціалізація процесора зі стартовою стратегією
            NumericProcessor processor = new NumericProcessor(new SquareOperationStrategy());

            // 5. Виконання операцій та сповіщення
            double value1 = 8.0;
            double result1 = processor.Process(value1);
            publisher.PublishResult(result1, "Зведення в квадрат");
            Console.WriteLine("--------------------------------------------------");

            processor.SetStrategy(new CubeOperationStrategy());
            double value2 = 4.0;
            double result2 = processor.Process(value2);
            publisher.PublishResult(result2, "Зведення в куб");
            Console.WriteLine("--------------------------------------------------");

            processor.SetStrategy(new SquareRootOperationStrategy());
            double value3 = 144.0;
            double result3 = processor.Process(value3);
            publisher.PublishResult(result3, "Квадратний корінь");

            // 6. Виведення історії
            historyLogger.PrintHistory();
        }
    }
}