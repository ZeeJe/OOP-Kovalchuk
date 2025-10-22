namespace IFareCalculator
{
    // 🔹 Інтерфейс для розрахунку вартості поїздки
    public interface IFareCalculator
    {
        // Метод, який реалізовують усі типи транспорту
        double CalculateFare(int passengers, double distance);
    }
}
