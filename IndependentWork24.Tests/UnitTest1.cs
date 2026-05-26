using System;
using Xunit;
using IndependentWork24;

namespace IndependentWork24.Tests
{
    public class PatternIntegrationTests
    {
        // Тест 1: Позитивний — Перевірка рекурсивного підрахунку Composite
        [Fact]
        public void ProductBundle_ShouldCalculateTotalSumOfNestedComponents()
        {
            // Arrange
            var p1 = new SingleProduct("Товар А", 300m);
            var p2 = new SingleProduct("Товар Б", 700m);
            var bundle = new ProductBundle("Набір");
            bundle.Add(p1);
            bundle.Add(p2);

            // Act
            decimal totalPrice = bundle.GetPrice();

            // Assert
            Assert.Equal(1000m, totalPrice);
        }

        // Тест 2: Позитивний — Перевірка накладання знижки Декоратором на Композит
        [Fact]
        public void DiscountDecorator_ShouldApplyDiscountOnCompositeCorrectly()
        {
            // Arrange
            var p = new SingleProduct("Смартфон", 10000m);
            var bundle = new ProductBundle("Комплект");
            bundle.Add(p);
            var decorated = new DiscountDecorator(bundle, 15); // 15% знижки

            // Act
            decimal finalPrice = decorated.GetPrice();

            // Assert
            Assert.Equal(8500m, finalPrice);
        }

        // Тест 3: Позитивний — Перевірка поведінки стану кешу в Proxy
        [Fact]
        public void CachedComponentProxy_ShouldCacheValueAfterFirstCall()
        {
            // Arrange
            var p = new SingleProduct("Продукт", 500m);
            var proxy = new CachedComponentProxy(p);

            // Act & Assert
            Assert.False(proxy.IsCacheValid()); // Спочатку кеш пустий
            
            decimal firstCall = proxy.GetPrice();
            Assert.True(proxy.IsCacheValid());  // Тепер кеш активовано
            
            decimal secondCall = proxy.GetPrice();
            Assert.Equal(firstCall, secondCall); // Значення ідентичні
        }

        // Тест 4: Негативний — Перевірка валідації некоректних даних (граничний випадок)
        [Fact]
        public void DiscountDecorator_ShouldThrowException_WhenDiscountIsInvalid()
        {
            // Arrange
            var p = new SingleProduct("Книга", 200m);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new DiscountDecorator(p, 150)); // Знижка > 100%
            Assert.Throws<ArgumentException>(() => new DiscountDecorator(p, -10)); // Знижка < 0%
        }

        // Тест 5: Граничний випадок — Порожній комплект Composite
        [Fact]
        public void EmptyBundle_ShouldReturnZeroPrice()
        {
            // Arrange
            var emptyBundle = new ProductBundle("Порожня коробка");

            // Act
            decimal price = emptyBundle.GetPrice();

            // Assert
            Assert.Equal(0m, price);
        }
    }
}