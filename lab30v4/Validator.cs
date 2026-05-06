using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace lab30v4;

public class Validator
{
    // Перевірка формату Email
    public bool IsValidEmail(string email)
    {
        if (email == null) throw new ArgumentNullException(nameof(email));
        if (string.IsNullOrWhiteSpace(email)) return false;

        // Базовий патерн: текст@текст.текст
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }

    // Перевірка телефону (міжнародний формат, наприклад: +380123456789)
    public bool IsValidPhone(string phone)
    {
        if (phone == null) throw new ArgumentNullException(nameof(phone));
        if (string.IsNullOrWhiteSpace(phone)) return false;

        // Патерн: обов'язковий + і від 10 до 14 цифр
        string pattern = @"^\+\d{10,14}$";
        return Regex.IsMatch(phone, pattern);
    }

    // Перевірка надійності пароля (мінімум 8 символів, 1 велика, 1 мала літера, 1 цифра)
    public bool IsStrongPassword(string password)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        
        return password.Length >= 8 &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit);
    }
}