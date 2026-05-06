using System;
using Moq;
using Xunit;
using lab31v4;

namespace lab31v4.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly Mock<IStockService> _mockStock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        // Ініціалізуємо Mock-об'єкти
        _mockRepo = new Mock<IProductRepository>();
        _mockStock = new Mock<IStockService>();
        
        // Передаємо "фейкові" реалізації в наш сервіс
        _service = new ProductService(_mockRepo.Object, _mockStock.Object);
    }

    // 1. Тест: Отримання продукту
    [Fact]
    public void GetProduct_WhenCalled_ReturnsProductFromRepository()
    {
        var expectedProduct = new Product { Id = 1, Name = "Laptop", Price = 1000 };
        // Setup: налаштовуємо мок повернути конкретний об'єкт
        _mockRepo.Setup(r => r.GetById(1)).Returns(expectedProduct);

        var result = _service.GetProduct(1);

        Assert.NotNull(result);
        Assert.Equal("Laptop", result.Name);
        // Verify: перевіряємо, що метод GetById викликався рівно 1 раз
        _mockRepo.Verify(r => r.GetById(1), Times.Once);
    }

    // 2. Тест: Продукт не знайдено
    [Fact]
    public void GetProduct_NotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetById(It.IsAny<int>())).Returns((Product?)null);

        var result = _service.GetProduct(999);

        Assert.Null(result);
    }

    // 3. Тест: Створення продукту успішне
    [Fact]
    public void CreateProduct_ValidProduct_CallsAddOnRepository()
    {
        var product = new Product { Id = 2, Name = "Mouse" };

        _service.CreateProduct(product);

        // Verify: перевіряємо, чи викликав сервіс метод Add у репозиторії
        _mockRepo.Verify(r => r.Add(product), Times.Once);
    }

    // 4. Тест: Спроба створити null-продукт
    [Fact]
    public void CreateProduct_NullProduct_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _service.CreateProduct(null!));
        // Verify: переконуємося, що метод Add НІКОЛИ не викликався
        _mockRepo.Verify(r => r.Add(It.IsAny<Product>()), Times.Never);
    }

    // 5. Тест: Оновлення ціни успішне
    [Fact]
    public void UpdatePrice_ExistingProduct_UpdatesAndCallsRepository()
    {
        var product = new Product { Id = 1, Price = 100 };
        _mockRepo.Setup(r => r.GetById(1)).Returns(product);

        _service.UpdatePrice(1, 150);

        Assert.Equal(150, product.Price);
        _mockRepo.Verify(r => r.Update(product), Times.Once);
    }

    // 6. Тест: Оновлення ціни неіснуючого продукту
    [Fact]
    public void UpdatePrice_NonExistingProduct_ThrowsInvalidOperationException()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns((Product?)null);

        Assert.Throws<InvalidOperationException>(() => _service.UpdatePrice(1, 150));
        _mockRepo.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    // 7. Тест: Замовлення товару (достатньо на складі)
    [Fact]
    public void OrderProduct_WithEnoughStock_ReturnsTrueAndDeductsStock()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new Product { Id = 1 });
        // Setup: імітуємо, що на складі достатньо товару
        _mockStock.Setup(s => s.HasEnoughStock(1, 5)).Returns(true);

        var result = _service.OrderProduct(1, 5);

        Assert.True(result);
        // Verify: перевіряємо, чи відбулось списання зі складу
        _mockStock.Verify(s => s.DeductStock(1, 5), Times.Once);
    }

    // 8. Тест: Замовлення товару (недостатньо на складі)
    [Fact]
    public void OrderProduct_WithoutEnoughStock_ReturnsFalseAndDoesNotDeduct()
    {
        _mockRepo.Setup(r => r.GetById(1)).Returns(new Product { Id = 1 });
        // Setup: імітуємо нестачу товару
        _mockStock.Setup(s => s.HasEnoughStock(1, 5)).Returns(false);

        var result = _service.OrderProduct(1, 5);

        Assert.False(result);
        // Verify: переконуємося, що списання не відбулося
        _mockStock.Verify(s => s.DeductStock(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }
}