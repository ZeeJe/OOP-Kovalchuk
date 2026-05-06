using System;

namespace lab31v4;

// Модель даних
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

// Залежність 1: Робота з базою даних
public interface IProductRepository
{
    Product? GetById(int id);
    void Add(Product product);
    void Update(Product product);
}

// Залежність 2: Перевірка залишків на складі
public interface IStockService
{
    bool HasEnoughStock(int productId, int quantity);
    void DeductStock(int productId, int quantity);
}

// Основний сервіс
public class ProductService
{
    private readonly IProductRepository _repository;
    private readonly IStockService _stockService;

    // Dependency Injection через конструктор
    public ProductService(IProductRepository repository, IStockService stockService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
    }

    public Product? GetProduct(int id) => _repository.GetById(id);

    public void CreateProduct(Product product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        _repository.Add(product);
    }

    public void UpdatePrice(int id, decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentException("Ціна не може бути від'ємною");
        
        var product = _repository.GetById(id);
        if (product == null) throw new InvalidOperationException("Продукт не знайдено");

        product.Price = newPrice;
        _repository.Update(product);
    }

    public bool OrderProduct(int id, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Кількість має бути більше 0");

        var product = _repository.GetById(id);
        if (product == null) return false;

        // Перевіряємо склад і списуємо товар, якщо його достатньо
        if (_stockService.HasEnoughStock(id, quantity))
        {
            _stockService.DeductStock(id, quantity);
            return true;
        }
        
        return false;
    }
}