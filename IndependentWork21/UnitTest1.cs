using System;
using System.Collections.Generic;
using Xunit;

namespace IndependentWork21
{
    // =========================================================================
    // БІЗНЕС-ЛОГІКА (Об'єднання 4 патернів для Варіанту №4: Форматування звітів)
    // =========================================================================

    // 1. Патерн STRATEGY
    public interface IReportStrategy 
    { 
        string Format(string text); 
    }

    public class HtmlStrategy : IReportStrategy 
    { 
        public string Format(string text) => $"<h1>{text}</h1>"; 
    }

    public class MarkdownStrategy : IReportStrategy 
    { 
        public string Format(string text) => $"# {text}"; 
    }

    // 2. Патерн FACTORY METHOD
    public abstract class StrategyFactory 
    { 
        public abstract IReportStrategy CreateStrategy(); 
    }

    public class HtmlStrategyFactory : StrategyFactory 
    { 
        public override IReportStrategy CreateStrategy() => new HtmlStrategy(); 
    }

    public class MarkdownStrategyFactory : StrategyFactory 
    { 
        public override IReportStrategy CreateStrategy() => new MarkdownStrategy(); 
    }

    // 3. Патерн OBSERVER
    public class ReportPublisher
    {
        public event Action<string>? OnReportGenerated;
        
        public void Notify(string message) 
        {
            OnReportGenerated?.Invoke(message);
        }
    }

    // 4. Патерн SINGLETON (Центральний менеджер системи)
    public class ReportManager
    {
        private static ReportManager? _instance;
        private IReportStrategy? _currentStrategy;

        // Глобальна точка доступу (Singleton)
        public static ReportManager Instance => _instance ??= new ReportManager();

        public ReportPublisher Publisher { get; } = new();
        public List<string> LogHistory { get; } = new();

        private ReportManager() { }

        // Метод очищення стану для ізоляції юніт-тестів
        public void Reset()
        {
            _currentStrategy = null;
            LogHistory.Clear();
        }

        // Встановлення стратегії через фабрику (Взаємодія Factory + Strategy)
        public void SetStrategyFromFactory(StrategyFactory factory)
        {
            _currentStrategy = factory.CreateStrategy();
        }

        // Основна логіка обробки (Взаємодія Singleton + Strategy + Observer)
        public string ProcessReport(string content)
        {
            if (_currentStrategy == null)
                throw new InvalidOperationException("Стратегію не встановлено!");

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Вміст звіту не може бути порожнім!");

            string formatted = _currentStrategy.Format(content);
            LogHistory.Add(formatted); // Зберігаємо стан у Singleton
            Publisher.Notify(formatted); // Сповіщаємо Observer
            
            return formatted;
        }
    }

    // =========================================================================
    // ІНТЕГРАЦІЙНІ ТЕСТИ (Перевірка взаємодії компонентів)
    // =========================================================================
    public class ReportIntegrationTests : IDisposable
    {
        private readonly ReportManager _manager;

        public ReportIntegrationTests()
        {
            // Перед кожним тестом беремо єдиний екземпляр та скидаємо його стан
            _manager = ReportManager.Instance;
            _manager.Reset();
        }

        public void Dispose()
        {
            // Очищення стану після завершення тесту
            _manager.Reset();
        }

        // --- ПОЗИТИВНІ СЦЕНАРІЇ ---

        [Fact]
        public void Test_Positive_FactoryAndStrategy_Integration()
        {
            // Сценарій 1: Коректне створення стратегії через Фабрику та її виконання
            // Arrange
            StrategyFactory factory = new HtmlStrategyFactory();

            // Act
            _manager.SetStrategyFromFactory(factory);
            string result = _manager.ProcessReport("Успішність");

            // Assert
            Assert.Equal("<h1>Успішність</h1>", result);
            Assert.Contains("<h1>Успішність</h1>", _manager.LogHistory);
        }

        [Fact]
        public void Test_Positive_ObserverNotification_Triggered()
        {
            // Сценарій 2: Перевірка автоматичного сповіщення підписників (Observer)
            // Arrange
            _manager.SetStrategyFromFactory(new MarkdownStrategyFactory());
            string receivedEventData = string.Empty;
            
            // Підписуємо анонімного спостерігача на подію
            _manager.Publisher.OnReportGenerated += (msg) => receivedEventData = msg;

            // Act
            _manager.ProcessReport("Звіт ООП");

            // Assert
            Assert.Equal("# Звіт ООП", receivedEventData);
        }

        [Fact]
        public void Test_Positive_RuntimeStrategySwitching_And_SingletonState()
        {
            // Сценарій 3: Динамічна зміна стратегій у рантаймі зі збереженням історії в Singleton
            // Arrange
            _manager.SetStrategyFromFactory(new HtmlStrategyFactory());
            _manager.ProcessReport("Перший");

            // Act: Змінюємо фабрику/стратегію на льоту
            _manager.SetStrategyFromFactory(new MarkdownStrategyFactory());
            string result2 = _manager.ProcessReport("Другий");

            // Assert
            Assert.Equal("# Другий", result2);
            Assert.Equal(2, _manager.LogHistory.Count); // Менеджер зберіг обидва звіти
            Assert.Equal("<h1>Перший</h1>", _manager.LogHistory[0]);
            Assert.Equal("# Другий", _manager.LogHistory[1]);
        }

        // --- НЕГАТИВНІ / ГРАНИЧНІ СЦЕНАРІЇ ---

        [Fact]
        public void Test_Negative_ProcessWithoutStrategy_ThrowsInvalidOperationException()
        {
            // Сценарій 4: Спроба обробки звіту, коли жодної стратегії не було задано
            // Arrange: стратегію навмисно не встановлено

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => _manager.ProcessReport("Тест тексту"));
            Assert.Equal("Стратегію не встановлено!", exception.Message);
        }

        [Fact]
        public void Test_Negative_ProcessEmptyContent_ThrowsArgumentException()
        {
            // Сценарій 5: Граничний випадок — передача порожнього тексту з чинною стратегією
            // Arrange
            _manager.SetStrategyFromFactory(new HtmlStrategyFactory());

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _manager.ProcessReport("   "));
            Assert.Equal("Вміст звіту не може бути порожнім!", exception.Message);
        }
    }
}
