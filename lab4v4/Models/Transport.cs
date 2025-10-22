namespace Transport
{
    // 🔹 Абстрактний клас, який задає спільні властивості транспорту
    public abstract class Transport : IFareCalculator
    {
        public string Name { get; protected set; }
        public double BaseRate { get; protected set; }

        // Абстрактний метод, який обов’язково реалізують похідні класи
        public abstract double CalculateFare(int passengers, double distance);

        // Повертає короткий опис транспорту
        public override string ToString() => $"{Name} (тариф {BaseRate:F2} грн/км)";
    }
}