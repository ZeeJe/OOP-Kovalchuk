using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        List<int> nums = new() { 5, 2, 8, 1, 7, 4, 3, 6 };

        Console.WriteLine("=== Lab6: Делегати, Predicate, Func, Comparison ===\n");

        // 1. Predicate — фільтрація парних чисел
        Predicate<int> isEven = n => n % 2 == 0;
        List<int> evens = nums.FindAll(isEven);

        Console.WriteLine("Парні числа:");
        evens.ForEach(n => Console.Write(n + " "));
        Console.WriteLine("\n");

        // 2. Comparison — сортування
        Comparison<int> cmp = delegate (int a, int b)
        {
            return a.CompareTo(b);
        };

        nums.Sort(cmp);

        Console.WriteLine("Після сортування:");
        nums.ForEach(n => Console.Write(n + " "));
        Console.WriteLine("\n");

        // 3. Func — підрахунок суми
        Func<int, int, int> add = (a, b) => a + b;

        // Aggregate з використанням Func
        int sum = nums.Aggregate(0, (acc, n) => add(acc, n));

        Console.WriteLine($"Сума елементів: {sum}\n");

        // 4. Action — друк результатів
        Action<string> print = s => Console.WriteLine("Action: " + s);
        print("Обробка завершена!");

        // 5. Бонус: комбінований делегат (multicast Action)
        Action bonus = () => Console.WriteLine("Перше повідомлення");
        bonus += () => Console.WriteLine("Друге повідомлення (multicast)");

        Console.WriteLine("\nКомбінований делегат:");
        bonus();
    }
}
