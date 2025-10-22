namespace IntercityTransport
{
    // 🔹 Реалізація для міжміського транспорту
    public class IntercityTransport : Transport
    {
        public IntercityTransport(double ratePerKm)
        {
            Name = "Міжміський транспорт";
            BaseRate = ratePerKm;
        }

        // Розрахунок з урахуванням знижки після 100 км
        public override double CalculateFare(int passengers, double distance)
        {
            double total = passengers * distance * BaseRate;
            if (distance > 100)
                total *= 0.9;
            return total;
        }
    }
}
