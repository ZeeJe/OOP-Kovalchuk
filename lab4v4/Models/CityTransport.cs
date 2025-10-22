namespace CityTransport
{
    // 🔹 Реалізація для міського транспорту
    public class CityTransport : Transport
    {
        public CityTransport(double ratePerKm)
        {
            Name = "Міський транспорт";
            BaseRate = ratePerKm;
        }

        // Розрахунок вартості без знижок
        public override double CalculateFare(int passengers, double distance)
        {
            return passengers * distance * BaseRate;
        }
    }
}
