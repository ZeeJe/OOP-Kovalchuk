using System;
using System.Collections.Generic;

// 🔹 Базовий клас Athlete
public class Athlete
{
    public string Name { get; set; }
    public int Age { get; set; }

    public Athlete(string name, int age)
    {
        Name = name;
        Age = age;
    }

    // Віртуальний метод тренування
    public virtual void Train()
    {
        Console.WriteLine($"{Name} тренується загальним чином.");
    }

    // Віртуальний метод змагання
    public virtual double Compete()
    {
        Console.WriteLine($"{Name} бере участь у змаганнях.");
        return 0.0;
    }
}

// 🔹 Похідний клас Runner
public class Runner : Athlete
{
    public double Speed { get; set; } // км/год

    public Runner(string name, int age, double speed)
        : base(name, age)
    {
        Speed = speed;
    }

    public override void Train()
    {
        Console.WriteLine($"{Name} тренується бігом. Швидкість: {Speed} км/год.");
    }

    public override double Compete()
    {
        double time = 100 / Speed; // час на 100 км
        Console.WriteLine($"{Name} (бігун) пробіг дистанцію за {time:F2} год.");
        return time;
    }
}

// 🔹 Похідний клас Swimmer
public class Swimmer : Athlete
{
    public double StrokeRate { get; set; } // гребків/хв

    public Swimmer(string name, int age, double strokeRate)
        : base(name, age)
    {
        StrokeRate = strokeRate;
    }

    public override void Train()
    {
        Console.WriteLine($"{Name} тренується у басейні. Частота гребків: {StrokeRate} за хвилину.");
    }

    public override double Compete()
    {
        double time = 100 / (StrokeRate / 2); // умовна формула
        Console.WriteLine($"{Name} (плавець) проплив дистанцію за {time:F2} хв.");
        return time;
    }
}

// 🔹 Основна програма
public class Program
{
    public static void Main(string[] args)
    {
        // Створюємо колекцію спортсменів (демонстрація поліморфізму)
        List<Athlete> athletes = new List<Athlete>()
        {
            new Runner("Іван", 25, 20.0),
            new Swimmer("Петро", 22, 40.0),
            new Runner("Марко", 27, 18.5)
        };

        Console.WriteLine("=== Тренування спортсменів ===");
        foreach (var athlete in athletes)
        {
            athlete.Train(); // викликається перевизначений метод
        }

        Console.WriteLine("\n=== Змагання ===");
        double total = 0;
        foreach (var athlete in athletes)
        {
            total += athlete.Compete();
        }

        double average = total / athletes.Count;
        Console.WriteLine($"\nСередній час виконання вправи: {average:F2}");

        Console.WriteLine("Роботу виконано успішно!");
    }
}
