using System;

class Teacher
{
    private string name;
    private string subject;
    public int Experience { get; set; }
    public Teacher(string name, string subject, int experience)
    {
        this.name = name;
        this.subject = subject;
        this.Experience = experience;
        Console.WriteLine($"Викликано конструктор для {name}");
    }
    public void Introduce()
    {
        Console.WriteLine($"Мене звати {name}. Я викладаю {subject} і маю {Experience} років досвіду.");
    }
    ~Teacher()
    {
        Console.WriteLine($"Об'єкт {name} знищено.");
    }
}
class Program
{
    static void Main(string[] args)
    {
        Teacher t1 = new Teacher("Камії-Максим Теа", "OOP", 15);
        Teacher t2 = new Teacher("Ковальчук Євген Миколайович", "Нічого", 17);
        t1.Introduce();
        t2.Introduce();
    }
}