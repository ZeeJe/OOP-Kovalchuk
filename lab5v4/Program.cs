using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

// ----- 1. Власний виняток (згідно з варіантом) -----
public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// ----- 2. Моделі (Сутності) -----

// Сутність, що зберігається в репозиторії
public class StockItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int QuantityInStock { get; set; }

    public StockItem(int id, string name, int quantity)
    {
        // Валідація при створенні
        if (quantity < 0)
            throw new ArgumentException("Кількість на складі не може бути від'ємною");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Назва товару не може бути порожньою");

        Id = id;
        Name = name;
        QuantityInStock = quantity;
    }

    public override string ToString() =>
        $"| {Id,-3} | {Name,-18} | {QuantityInStock,-7} |";
}

// Частина замовлення (для композиції)
public class OrderLine
{
    public int StockItemId { get; set; } // Посилання на Id StockItem
    public int Quantity { get; set; }

    public OrderLine(int stockItemId, int quantity)
    {
        // Валідація згідно з варіантом: кидаємо власний виняток
        if (quantity <= 0)
            throw new InvalidQuantityException($"Замовлена кількість ({quantity}) має бути додатньою.");

        StockItemId = stockItemId;
        Quantity = quantity;
    }
}

// Головний клас (Композиція: Order "має" OrderLine)
public class Order
{
    public int OrderId { get; set; }
    // Композиція: список ліній є частиною замовлення
    private readonly List<OrderLine> _lines = new List<OrderLine>();

    public Order(int orderId)
    {
        OrderId = orderId;
    }

    public void AddLine(OrderLine line)
    {
        _lines.Add(line);
    }

    // Робимо список доступним лише для читання ззовні
    public IReadOnlyList<OrderLine> Lines => _lines;
}

// ----- 3. Узагальнений репозиторій (Generics) -----

public interface IRepository<T>
{
    void Add(T item);
    bool Remove(Predicate<T> match);
    IEnumerable<T> Where(Func<T, bool> predicate);
    T? FirstOrDefault(Func<T, bool> predicate);
    IReadOnlyList<T> All();
}

public class Repository<T> : IRepository<T>
{
    private readonly List<T> _data = new List<T>();

    public void Add(T item) => _data.Add(item);
    public bool Remove(Predicate<T> match) => _data.RemoveAll(match) > 0;
    public IEnumerable<T> Where(Func<T, bool> predicate) => _data.Where(predicate); // Використання LINQ
    public T? FirstOrDefault(Func<T, bool> predicate) => _data.FirstOrDefault(predicate); // Використання LINQ
    public IReadOnlyList<T> All() => _data;
}

// ----- 4. IComparer (Сортування) -----
// Згідно з варіантом: сортування IComparer<StockItem> за залишком
public class StockItemByQuantityComparer : IComparer<StockItem>
{
    public int Compare(StockItem? x, StockItem? y)
    {
        if (x == null || y == null) return 0;
        // Порівнюємо за QuantityInStock
        return x.QuantityInStock.CompareTo(y.QuantityInStock);
    }
}

// ----- 5. Логіка обчислень -----

// Клас для результатів обчислення
public class OrderFulfillmentResult
{
    public int TotalOrdered { get; set; }
    public int TotalReserved { get; set; }
    public int TotalDeficit { get; set; }
    public double FulfillmentPercentage { get; set; }
}

// Окремий клас-сервіс для розрахунків
public class OrderCalculator
{
    private readonly IRepository<StockItem> _stockRepo;

    public OrderCalculator(IRepository<StockItem> stockRepo)
    {
        _stockRepo = stockRepo;
    }

    // Метод для обчислень (вимога варіанту)
    public OrderFulfillmentResult Calculate(Order order)
    {
        // Використання методів LINQ для обчислень
        int totalOrdered = order.Lines.Sum(line => line.Quantity);
        int totalReserved = 0;
        int totalDeficit = 0;

        foreach (var line in order.Lines)
        {
            // Знаходимо товар на складі
            var stockItem = _stockRepo.FirstOrDefault(item => item.Id == line.StockItemId);
            int inStock = stockItem?.QuantityInStock ?? 0;

            // Розраховуємо резерв та дефіцит
            int reserved = Math.Min(inStock, line.Quantity);
            int deficit = line.Quantity - reserved;

            totalReserved += reserved;
            totalDeficit += deficit;
        }

        // Розрахунок відсотка
        double percentage = (totalOrdered > 0) ? ((double)totalReserved / totalOrdered) * 100 : 100.0;

        return new OrderFulfillmentResult
        {
            TotalOrdered = totalOrdered,
            TotalReserved = totalReserved,
            TotalDeficit = totalDeficit,
            FulfillmentPercentage = percentage
        };
    }
}


// ----- 6. Демонстрація в Program.cs -----

public class Program
{
    public static void Main()
    {
        // Встановіть кодування для коректного відображення в консолі
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("--- Лабораторна робота №5. Варіант 4: Склад і Замовлення ---");

        // 1. Створюємо узагальнений репозиторій (вимога Generics)
        IRepository<StockItem> stockRepository = new Repository<StockItem>();

        // 2. Додаємо дані
        stockRepository.Add(new StockItem(1, "Молоко", 10));
        stockRepository.Add(new StockItem(2, "Хліб", 30));
        stockRepository.Add(new StockItem(3, "Яйця (десяток)", 5)); // Дефіцитний товар
        stockRepository.Add(new StockItem(4, "Вода 1л", 50));

        Console.WriteLine("\n--- Стан складу (до сортування) ---");
        PrintStock(stockRepository.All());

        // 3. Демонстрація IComparer (Сортування)
        Console.WriteLine("\n--- Стан складу (сортування за залишком) ---");
        var allStock = stockRepository.All().ToList();
        allStock.Sort(new StockItemByQuantityComparer()); // Використання IComparer
        PrintStock(allStock);

        // 4. Демонстрація обробки винятків (Вимога)
        Console.WriteLine("\n--- Демонстрація винятку (InvalidQuantityException) ---");
        try
        {
            Console.WriteLine("Спроба створити замовлення на -5 шт. хліба...");
            // Цей код згенерує виняток
            var invalidLine = new OrderLine(stockItemId: 2, quantity: -5);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("ПОМИЛКА: Виняток не було згенеровано!");
            Console.ResetColor();
        }
        catch (InvalidQuantityException ex) // Обробка власного винятку
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"УСПІХ! Спіймано власний виняток: {ex.Message}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Спіймано інший виняток: {ex.Message}");
        }

        // 5. Демонстрація обчислень (Композиція + LINQ/Обчислення)
        Console.WriteLine("\n--- Розрахунок виконання замовлення ---");

        // Створюємо замовлення (Композиція)
        var order = new Order(101);
        order.AddLine(new OrderLine(1, 5));  // 5 молока (є 10)
        order.AddLine(new OrderLine(2, 20)); // 20 хліба (є 30)
        order.AddLine(new OrderLine(3, 10)); // 10 яєць (є 5) -> дефіцит 5
        order.AddLine(new OrderLine(99, 1)); // Неіснуючий товар -> дефіцит 1

        // Розраховуємо
        OrderCalculator calculator = new OrderCalculator(stockRepository);
        OrderFulfillmentResult result = calculator.Calculate(order);

        Console.WriteLine($"Замовлення ID: {order.OrderId}");
        Console.WriteLine($"Всього замовлено: {result.TotalOrdered} шт.");
        Console.WriteLine($"Зарезервовано: {result.TotalReserved} шт.");
        Console.WriteLine($"Дефіцит: {result.TotalDeficit} шт.");
        Console.WriteLine($"Відсоток виконання: {result.FulfillmentPercentage:F2}%");

        // 6. Демонстрація LINQ .Where (додатково)
        var lowStockItems = stockRepository.Where(item => item.QuantityInStock < 15);
        Console.WriteLine("\n--- Товари, що закінчуються (LINQ .Where < 15) ---");
        PrintStock(lowStockItems);
    }

    // Допоміжний метод для друку (має бути static)
    public static void PrintStock(IEnumerable<StockItem> stockItems)
    {
        Console.WriteLine("| ID  | Назва              | Залишок |");
        Console.WriteLine("|-----|--------------------|---------|");
        foreach (var item in stockItems)
        {
            Console.WriteLine(item);
        }
    }
}