using System;
using Xunit;
using lab30v4;

namespace lab30v4.Tests;

public class ValidatorTests
{
    private readonly Validator _validator = new Validator();

    // --- Тести для IsValidEmail ---

    // 1. [Fact] - Успішний сценарій
    [Fact]
    public void IsValidEmail_ValidEmail_ReturnsTrue()
    {
        Assert.True(_validator.IsValidEmail("test@example.com"));
    }

    // 2. [Theory] - Некоректні формати email (крайові випадки)
    [Theory]
    [InlineData("plainaddress")]
    [InlineData("@missingusername.com")]
    [InlineData("missingdomain@.com")]
    [InlineData("spaces in@email.com")]
    [InlineData("")] // Порожній рядок
    public void IsValidEmail_InvalidEmail_ReturnsFalse(string invalidEmail)
    {
        Assert.False(_validator.IsValidEmail(invalidEmail));
    }

    // 3. [Fact] - Перевірка на null (помилка)
    [Fact]
    public void IsValidEmail_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _validator.IsValidEmail(null!));
    }

    // --- Тести для IsValidPhone ---

    // 4. [Fact] - Успішний сценарій
    [Fact]
    public void IsValidPhone_ValidFormat_ReturnsTrue()
    {
        Assert.True(_validator.IsValidPhone("+380123456789"));
    }

    // 5. [Theory] - Некоректні формати телефонів
    [Theory]
    [InlineData("0981234567")] // Без плюса
    [InlineData("+380abc45678")] // Містить літери
    [InlineData("+12")] // Занадто короткий
    [InlineData("")] // Порожній рядок
    public void IsValidPhone_InvalidFormat_ReturnsFalse(string invalidPhone)
    {
        Assert.False(_validator.IsValidPhone(invalidPhone));
    }

    // 6. [Fact] - Перевірка на null (помилка)
    [Fact]
    public void IsValidPhone_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _validator.IsValidPhone(null!));
    }

    // --- Тести для IsStrongPassword ---

    // 7. [Fact] - Успішний сценарій (надійний пароль)
    [Fact]
    public void IsStrongPassword_StrongPassword_ReturnsTrue()
    {
        Assert.True(_validator.IsStrongPassword("StrongPass1"));
    }

    // 8. [Theory] - Слабкі паролі (відсутні певні критерії)
    [Theory]
    [InlineData("weak")] // Занадто короткий
    [InlineData("alllowercase1")] // Немає великих літер
    [InlineData("ALLUPPERCASE1")] // Немає малих літер
    [InlineData("NoDigitsHere")] // Немає цифр
    public void IsStrongPassword_WeakPassword_ReturnsFalse(string weakPassword)
    {
         Assert.False(_validator.IsStrongPassword(weakPassword));
    }

    // 9. [Fact] - Edge case (пароль рівно 8 символів - мінімальна допустима межа)
    [Fact]
    public void IsStrongPassword_Exactly8CharsValid_ReturnsTrue()
    {
        Assert.True(_validator.IsStrongPassword("A1b2C3d4"));
    }

    // 10. [Fact] - Перевірка на null (помилка)
    [Fact]
    public void IsStrongPassword_Null_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _validator.IsStrongPassword(null!));
    }
}