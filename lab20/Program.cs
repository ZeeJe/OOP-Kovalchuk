using System;
using System.Collections.Generic;

namespace lab20
{
    // --- МОДЕЛЬ ДАНИХ ---
    public enum OrderStatus
    {
        New,
        PendingValidation,
        Processed,
        Shipped,
        Delivered,
        Cancelled
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Order(int id, string customerName, decimal totalAmount)
        {
            Id = id;
            CustomerName = customerName;
            TotalAmount = totalAmount;
            Status = OrderStatus.New;
        }
    }

    // --- ПОЧАТКОВА РЕАЛІЗАЦІЯ (ПОРУШЕННЯ SRP) ---
    public class BadOrderProcessor
    {
        public void ProcessOrder(Order order)
        {
            // 1. Валідація
            if (order.TotalAmount <= 0)
            {
                Console.WriteLine($"[BadProcessor] Помилка: Замовлення {order.Id} невалідне.");
                return;
            }

            // 2. Збереження
            Console.WriteLine($"[BadProcessor] Замовлення {order.Id} збережено в БД.");

            // 3. Email
            Console.WriteLine($"[BadProcessor] Email відправлено клієнту {order.CustomerName}.");

            // 4. Оновлення статусу
            order.Status = OrderStatus.Processed;
            Console.WriteLine($"[BadProcessor] Статус замовлення {order.Id} оновлено на Processed.");
        }
    }

    // --- РЕФАКТОРИНГ: ІНТЕРФЕЙСИ ТА РЕАЛІЗАЦІЇ (SRP) ---

    public interface IOrderValidator
    {
        bool IsValid(Order order);
    }

    public interface IOrderRepository
    {
        void Save(Order order);
        Order GetById(int id);
    }

    public interface IEmailService
    {
        void SendOrderConfirmation(Order order);
    }

    // Реалізації-"заглушки"
    public class SimpleOrderValidator : IOrderValidator
    {
        public bool IsValid(Order order) => order.TotalAmount > 0;
    }

    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly Dictionary<int, Order> _database = new Dictionary<int, Order>();
        public void Save(Order order) 
        {
            _database[order.Id] = order;
            Console.WriteLine($"[Repository] Замовлення {order.Id} збережено в InMemory БД.");
        }
        public Order GetById(int id) => _database.ContainsKey(id) ? _database[id] : null;
    }

    public class ConsoleEmailService : IEmailService
    {
        public void SendOrderConfirmation(Order order)
        {
            Console.WriteLine($"[Email] Повідомлення відправлено на адресу {order.CustomerName}@example.com.");
        }
    }

    // --- СЕРВІС, ЩО КООРДИНУЄ РОБОТУ (Dependency Injection) ---
    public class OrderService
    {
        private readonly IOrderValidator _validator;
        private readonly IOrderRepository _repository;
        private readonly IEmailService _emailService;

        public OrderService(IOrderValidator validator, IOrderRepository repository, IEmailService emailService)
        {
            _validator = validator;
            _repository = repository;
            _emailService = emailService;
        }

        public void ProcessOrder(Order order)
        {
            if (!_validator.IsValid(order))
            {
                Console.WriteLine($"[OrderService] Валідація замовлення {order.Id} провалена.");
                return;
            }

            _repository.Save(order);
            _emailService.SendOrderConfirmation(order);
            
            order.Status = OrderStatus.Processed;
            Console.WriteLine($"[OrderService] Замовлення {order.Id} успішно оброблено.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Демонстрація BadOrderProcessor
            Console.WriteLine("=== ТЕСТ: ПОЧАТКОВИЙ PROCESSOR (ПОРУШЕННЯ SRP) ===");
            var badProcessor = new BadOrderProcessor();
            badProcessor.ProcessOrder(new Order(1, "Іван", 1500m));

            // 2. Демонстрація рефакторингу (Good Design)
            Console.WriteLine("\n=== ТЕСТ: РЕФАКТОРИНГОВИЙ ORDER SERVICE (SRP) ===");
            
            var service = new OrderService(
                new SimpleOrderValidator(),
                new InMemoryOrderRepository(),
                new ConsoleEmailService()
            );

            Console.WriteLine("\n--- Обробка валідного замовлення ---");
            var validOrder = new Order(2, "Марія", 2500m);
            service.ProcessOrder(validOrder);

            Console.WriteLine("\n--- Обробка невалідного замовлення ---");
            var invalidOrder = new Order(3, "Олег", -10m);
            service.ProcessOrder(invalidOrder);

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}